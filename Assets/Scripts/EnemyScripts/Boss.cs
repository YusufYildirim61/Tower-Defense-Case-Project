using UnityEngine;

public class Boss : BaseEnemy
{
    
    [Header("Attack Settings")]
    [SerializeField] Transform attackArea;
    [SerializeField] LayerMask attackLayers;
    [Header("Skills Settings")]
    [SerializeField] float skillCooldown;
    [SerializeField] Transform skillPoint;
    [SerializeField] float skillRange = 0.5f;
    [SerializeField] LayerMask skillLayers;
    [Header("Buff Settings")]
    [SerializeField] float damageBuffMultiplier;
    [SerializeField] float buffDuration;
    [Header("Protection Settings")]
    [SerializeField] float protectionMultiplier;
    [SerializeField] float protectionDuration;

    EnemySO enemySO;
    Collider2D[] hits;
    Collider2D[] enemyColliders;

    float nextAttackTime = 0f;
    bool canTakeDamage = true;
    bool canMove = true;
    float initialSkillCooldown;
    protected override void Start()
    {
        base.Start();
        enemySO = GetEnemyData();
        initialSkillCooldown = skillCooldown;
    }

    void Update()
    {
        Movement();
        HandleSkill();
    }
    protected override void Movement()
    {
        hits = Physics2D.OverlapBoxAll(attackArea.position, attackArea.localScale * 0.5f, 0f,attackLayers);
        if(hits.Length>0)
        {
            GetEnemyAnimator().SetBool("Walk",false);
            if (Time.time > nextAttackTime)
            {
                base.Attack(hits);
                nextAttackTime = Time.time + 1f / enemySO.attackRate;
            }
            
        }
        else
        {
            if(canMove)
            {
                base.Movement();
            }
        }
        
    }
    public override void TakeDamage(float damage)
    {
        if(canTakeDamage)
        {
            base.TakeDamage(damage);
        }
    }
    public void ActivateTakeDamage() // Animation Event for the end of Buff and Protection
    {
        canTakeDamage = true;
        canMove = true;
    }
    void PerformSkill()
    {
        int randomAnimationIndex = Random.Range(0,2);
        if(randomAnimationIndex == 0)
        {
            GetEnemyAnimator().SetTrigger("Buff");
            canTakeDamage = false;
            canMove = false;
        }
        else
        {
            GetEnemyAnimator().SetTrigger("Protect");
            canTakeDamage = false;
            canMove = false;
        }
    }
    void HandleSkill()
    {
        if(GameManager.Instance.GameOver || !GetComponent<Collider2D>().enabled)
        {
            return;
        }
        else
        {
            skillCooldown -= Time.deltaTime;
            if(skillCooldown <=0)
            {
                PerformSkill();
                skillCooldown = initialSkillCooldown;
            }
        }
        
    }
    public void ActivateBuff() // Animation Event
    {
        enemyColliders = Physics2D.OverlapCircleAll(skillPoint.position,skillRange,skillLayers);

        foreach (Collider2D enemy in enemyColliders)
        {
            BaseEnemy buffedEnemy = enemy.GetComponent<BaseEnemy>();
            buffedEnemy.ApplyBuff(BuffType.Damage,damageBuffMultiplier,buffDuration);
        }
    }
    public void ActivateProtection() // Animation Event
    {
        enemyColliders = Physics2D.OverlapCircleAll(skillPoint.position,skillRange,skillLayers);

        foreach (Collider2D enemy in enemyColliders)
        {
            BaseEnemy buffedEnemy = enemy.GetComponent<BaseEnemy>();
            buffedEnemy.ApplyBuff(BuffType.Protection,protectionMultiplier,protectionDuration);
        }
    }
    public void SetSendAttack() // Animation Event for Attack
    {
        DealDamage();
    }

    void DealDamage() // Boss can damage The Player and The Castle at the same time
    {
        foreach (Collider2D col in hits)
        {
            PlayerMovement player = col.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.TakeDamage(enemySO.attackPower);
                continue;
            }
            Castle castle = col.GetComponent<Castle>();
            if (castle != null)
            {
                castle.TakeDamage(enemySO.attackPower);
                continue;
            }
        }
    }

    void OnDrawGizmos()
    {
        Vector3 size = attackArea.localScale;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(attackArea.position, size); 
        Gizmos.DrawWireSphere(skillPoint.position,skillRange);
    }
}
