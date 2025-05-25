using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Character")]
public class CharacterSO : ScriptableObject
{
   public Sprite icon;
   public int health;
   public int damage;
   public float moveSpeed;
   public float attackCooldown;
   public string characterName;
   public string attackRateInfo;
}
