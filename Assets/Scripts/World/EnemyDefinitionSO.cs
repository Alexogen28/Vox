using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDefinitionSO", menuName = "Vox Obscura/World Generation/Enemy Definition")]
public class EnemyDefinitionSO : ScriptableObject
{
    public GameObject prefab;
    public float aggroRange;
    public float attackRange;
    public float roamRadius;
    public float roamWaitTime;
    public float attackDamage;
    public float attackCooldown;
    public float maxHealth;
}
