using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health Parameters")]
    [SerializeField] public float maxHealth = 100.0f;
    public float currentHealth;
    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damageToTake)
    {
        currentHealth -= damageToTake;

        Debug.Log(this.gameObject + "has the follow health: " + currentHealth);

        if (currentHealth <= 0)
            Die();
    }

    public void Die()
    {
        Debug.Log($"{gameObject.name} died!");
        Destroy(gameObject);
    }
}
