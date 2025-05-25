using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Enemy")]
public class EnemySO : ScriptableObject
{
    public string enemyName;
    public int health;
    public float moveSpeed;
    public int attackPower;
    public float attackRate;
}
