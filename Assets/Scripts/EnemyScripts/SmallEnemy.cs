using UnityEngine;

public class SmallEnemy : BaseEnemy
{
    [Header("Attack Settings")]
    [SerializeField] Transform attackArea;
    [SerializeField] LayerMask attackLayers;

    EnemySO enemySO;
    Collider2D[] hits;

    float nextAttackTime = 0f;
    protected override void Start()
    {
        base.Start();
        enemySO = GetEnemyData();
    }

    void Update()
    {
        if(!GameManager.Instance.GameOver)
        {
            Movement();
        }
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
            base.Movement();
        }
        
    }

    public void SetSendAttack() // Animation Event 
    {
        DealDamage();
    }

    void DealDamage()
    {
        foreach (Collider2D col in hits)
        {
            Castle castle = col.GetComponent<Castle>();
            if (castle != null)
            {
                castle.TakeDamage(GetInitialDamage());
                break;
            }
        }
    }

    void OnDrawGizmos()
    {
        Vector3 size = attackArea.localScale;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(attackArea.position, size); 
    }
}
