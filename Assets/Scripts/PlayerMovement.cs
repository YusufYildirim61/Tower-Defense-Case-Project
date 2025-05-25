using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Stats")]
    [SerializeField] float iFrameDuration = 3f;
    [SerializeField] CharacterSO playerData;
    [Header("Attack Settings")]
    [SerializeField] int numberOfAttacks = 3;
    [SerializeField] Transform attackArea;
    [SerializeField] LayerMask attackLayers;
    [Header("Flash Effect Settings")]
    [ColorUsage(true,true)]
    [SerializeField] Color flashColor = Color.white;
    [SerializeField] float flashTime = 0.25f;

    Vector2 moveInput;
    Rigidbody2D rb;
    Animator animator;
    Collider2D playerCollider;
    SpriteRenderer spriteRenderer;
    Material material;

    bool canTakeDamage = true;
    float attackTimer;
    bool isAlive = true;
    float initialHealth;

    public CharacterSO GetCharacterSO()
    {
        return playerData;
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        material = spriteRenderer.material;

        initialHealth = playerData.health;
        
        GameEvents.OnSetMaxHealth?.Invoke(initialHealth,playerData.icon);
    }

    void Update()
    {
        if(isAlive && !GameManager.Instance.GameOver)
        {
            CheckAttack();
            AnimatePlayer();
        }
    }

    #region "Attack Methods"
    void CheckAttack()
    {
        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f && IsEnemyInRange())
        {
            animator.SetBool("Walk", false);
            Attack();
            attackTimer = playerData.attackCooldown;
        }
    }
    void Attack()
    {
        int randomAttackIndex = Random.Range(0, numberOfAttacks);
        animator.SetInteger("AttackIndex", randomAttackIndex);
    }
    public void SetSendAttack() // Animation Event 
    {
        DealDamage();
    }
    void DealDamage()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(attackArea.position, attackArea.localScale * 0.5f, 0f,attackLayers);

        foreach (Collider2D enemyCollider in hits)
        {
            BaseEnemy targetEnemy = enemyCollider.GetComponent<BaseEnemy>();
            if (targetEnemy != null)
            {
                targetEnemy.TakeDamage(playerData.damage);
            }
        }
    }
    bool IsEnemyInRange()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(attackArea.position, attackArea.localScale * 0.5f, 0f,attackLayers);

        foreach (Collider2D enemyCollider in hits)
        {
            if (enemyCollider.GetComponent<BaseEnemy>() != null)
            {
                return true;
            }
        }
        return false;
    }
    public void ResetAttackAnimation()
    {
        animator.SetInteger("AttackIndex", -1);
    }
    #endregion

    #region "Movement Methods"
    void OnMove(InputValue value)
    {
        if(isAlive && !GameManager.Instance.GameOver)
        {
            moveInput = value.Get<Vector2>();
            MovePlayer();
        }

    }
    void MovePlayer()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x * playerData.moveSpeed, moveInput.y * playerData.moveSpeed);
        rb.linearVelocity = playerVelocity; 
        spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
        FlipSprite();
    }
    void FlipSprite() 
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(rb.linearVelocity.x)>Mathf.Epsilon;

        if(playerHasHorizontalSpeed)
        {
            float direction = rb.linearVelocity.x > 0 ? 0f : 180f;
            transform.rotation = Quaternion.Euler(0f, direction, 0f);
        }
    }
    void AnimatePlayer()
    {
        bool isMoving = moveInput.sqrMagnitude > 0.01f; 
        animator.SetBool("Walk", isMoving);
    }
    #endregion
    
    #region "Damage, Death and Respawn Methods"
    public void TakeDamage(float damage)
    {
        if(canTakeDamage && isAlive)
        {
            animator.SetTrigger("Hurt");
            StartCoroutine(DamageFlasher());
            initialHealth -= damage;
            GameEvents.OnHealthChanged?.Invoke(initialHealth);
            if(initialHealth<=0)
            {
                Die();
                return;
            }
            StartCoroutine(IFrameHandler());
        }
        
    }
    IEnumerator DamageFlasher()
    {
        material.SetColor("_FlashColor",flashColor);

        float currentFlashAmount = 0;
        float elapsedTime = 0;

        while(elapsedTime < flashTime)
        {
            elapsedTime += Time.deltaTime;
            currentFlashAmount = Mathf.Lerp(1,0, elapsedTime / flashTime);
            material.SetFloat("_FlashAmount",currentFlashAmount);
            yield return null;
        }
    }
    void Die()
    {
        StopPlayer();
        animator.SetTrigger("Death");
        GameEvents.OnPlayerDied?.Invoke();
        
        isAlive = false;
    }
    public void StopPlayer()
    {
        ResetAttackAnimation();
        animator.SetBool("Walk",false);
        rb.linearVelocity = Vector3.zero;
        playerCollider.enabled = false;
    }
    IEnumerator IFrameHandler()
    {
        if(isAlive)
        {
            canTakeDamage = false;
            animator.SetBool("IFrame",true);

            yield return new WaitForSeconds(iFrameDuration);

            canTakeDamage = true;
            animator.SetBool("IFrame",false);
        }
        
    }
    public void RespawnPlayer(Transform spawnPoint) 
    {
        animator.SetTrigger("Respawn");
        animator.SetBool("IFrame",true);
        GameEvents.OnSetMaxHealth?.Invoke(playerData.health,playerData.icon);
        transform.position = spawnPoint.position;
        rb.linearVelocity = Vector3.zero;
        initialHealth = playerData.health;

        StartCoroutine(RespawnIFrame());
    }
    public void EnableMovement() // Animation Event
    {
        playerCollider.enabled = true;
        isAlive = true;
    }
    IEnumerator RespawnIFrame()
    {
        canTakeDamage = false;
        animator.SetBool("IFrame",true);
        yield return new WaitForSeconds(3f);
        canTakeDamage = true;
        animator.SetBool("IFrame",false);
    }
    #endregion
    void OnDrawGizmos()
    {
        Vector3 size = attackArea.localScale;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(attackArea.position, size); 
    }
}
