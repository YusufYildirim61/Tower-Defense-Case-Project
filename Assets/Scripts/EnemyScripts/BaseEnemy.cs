using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BaseEnemy : MonoBehaviour
{
    public enum BuffType { Damage, Protection }
    [SerializeField] EnemySO enemyData;
    [Header("Health Bar Settings")]
    [SerializeField] Slider healthBarSlider;
    [SerializeField] Gradient gradient;
    [SerializeField] Image fill;
    [Header("Icons Settings")]
    [SerializeField] GameObject buffIcon;
    [SerializeField] GameObject protectIcon;

    [Header("Flash Effect Settings")]
    [ColorUsage(true,true)]
    [SerializeField] Color flashColor = Color.white;
    [SerializeField] float flashTime = 0.25f;

    Animator enemyAnimator;
    Collider2D enemyCollider;
    SpriteRenderer spriteRenderer;
    Material material;
    Waypoints waypoints;
    Transform target;
    Vector3 targetPos;
    
    

    float initialHealth,initialMoveSpeed,initialDamage;
    float damageReduction = 1;
    bool isAlive = true;
    int waypointIndex = 0;
    bool isBuffed = false;
    bool isProtected = false;

    protected EnemySO GetEnemyData()
    {
        return enemyData;
    }
    protected Animator GetEnemyAnimator()
    {
        return enemyAnimator;
    }
    protected float GetInitialDamage()
    {
        return initialDamage;
    }

    protected virtual void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        enemyCollider = GetComponent<Collider2D>();
        enemyAnimator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        material = spriteRenderer.material;

        spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);

        enemyAnimator.SetBool("Walk", true);

        initialHealth = enemyData.health;
        initialMoveSpeed = enemyData.moveSpeed;
        initialDamage = enemyData.attackPower;

        SetMaxHealth(initialHealth);

        waypoints = GetComponentInParent<Waypoints>();
        target = waypoints.GetPoints()[0];
        targetPos = GetRandomPosition(target.position, target.localScale);
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
    
    #region "Movement Methods"
    protected virtual void Movement()
    {
        if(isAlive && !GameManager.Instance.GameOver)
        {
            enemyAnimator.SetBool("Walk",true);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, initialMoveSpeed * Time.deltaTime);

            if(Vector3.Distance(transform.position, targetPos) <= 0.2f)
            {
                GetNextWaypoint();
            }
        }
        else
        {
            enemyAnimator.SetBool("Walk",false);
            return;
        }
    }
    void GetNextWaypoint()
    {
        if(waypointIndex >= waypoints.GetPoints().Length -1)
        {
            return;
        }
        waypointIndex++;
        target = waypoints.GetPoints()[waypointIndex];
        targetPos = GetRandomPosition(target.position,target.localScale);
    }
    Vector2 GetRandomPosition(Vector3 center, Vector3 size)
    {
        float x = Random.Range(-0.5f, 0.5f) * size.x;
        float y = Random.Range(-0.5f, 0.5f) * size.y;
        return new Vector2(center.x + x, center.y + y);
    }
    #endregion

    #region "Attack and Death Methods"
    protected virtual void Attack(Collider2D[] hits)
    {
        if(!isAlive) return;
        enemyAnimator.SetBool("Walk",false);
        enemyAnimator.SetTrigger("Attack");
    }
    public virtual void TakeDamage(float damage)
    {
        initialHealth -= damage * damageReduction;

        StartCoroutine(DamageFlasher());
        enemyAnimator.SetTrigger("Hurt");
        SetHealth(initialHealth);

        if(initialHealth<=0)
        {
            Die();
        }
    }
    protected virtual void Die()
    {
        isAlive = false;
        GameEvents.OnEnemyKill?.Invoke();

        healthBarSlider.gameObject.SetActive(false);
        enemyCollider.enabled = false;

        enemyAnimator.SetTrigger("Death");

        StartCoroutine(DestroyEnemy());
    }
    public void SetMaxHealth(float health)
    {
        healthBarSlider.maxValue = health;
        healthBarSlider.value = health;
        
        fill.color = gradient.Evaluate(1f);
    }
    public void SetHealth(float health)
    {
        healthBarSlider.value = health;

        fill.color = gradient.Evaluate(healthBarSlider.normalizedValue);
    }
    IEnumerator DestroyEnemy()
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }
    #endregion

    #region "Buff Methods"
    public virtual void ApplyBuff(BuffType type,float multiplier,float duration)
    {
        switch (type)
        {
            case BuffType.Damage:
                if(isBuffed)
                {
                    return;    
                } 
                initialDamage *= multiplier;
                isBuffed = true;
                StartCoroutine(EndBuff(type,duration));
                break;

            case BuffType.Protection:
                if(isProtected)
                {
                    return;
                }
                damageReduction = multiplier;
                isProtected = true;
                StartCoroutine(EndBuff(type, duration));
                break;
        }

        UpdateVisuals(type, true);
    }
    IEnumerator EndBuff(BuffType type, float duration)
    {
        yield return new WaitForSeconds(duration);

        switch (type)
        {
            case BuffType.Damage:
                initialDamage = enemyData.attackPower;
                isBuffed = false;
                break;

            case BuffType.Protection:
                damageReduction = 1;
                isProtected = false;
                break;
        }

        UpdateVisuals(type, false);
    }
    void UpdateVisuals(BuffType type, bool active)
    {
        if (type == BuffType.Damage)
        {
            buffIcon.SetActive(active || isBuffed);
        }
        else if (type == BuffType.Protection)
        {
            protectIcon.SetActive(active || isProtected);
        }
    }
    #endregion
}
