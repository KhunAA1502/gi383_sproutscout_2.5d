using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Enemy System/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string enemyName = "Enemy";
    public float attackRange = 2.0f;
    public float attackCooldown = 1.5f;
    public int damage = 10;
}