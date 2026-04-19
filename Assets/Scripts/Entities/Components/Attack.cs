using UnityEngine;

public class Attack : MonoBehaviour
{
    [Header("Basic Attack Parameters")]
    [SerializeField] public float attackDamage = 10.0f;
    [SerializeField] public float attackDelay = 0.5f;
    [SerializeField] public float attackRange = 50.0f;


    public void DealDamage(Health target, float damageToDeal)
    {
        target.TakeDamage(damageToDeal);
    }
}
