using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    Roam,
    Aggro,
    Attack
}


public abstract class EnemyController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected GameManager gameManager;
    [SerializeField] protected Health health;
    [SerializeField] protected Attack attack;
    [SerializeField] protected Animator animator;
    [SerializeField] protected PlayerController playerController;
    [SerializeField] protected Vector3 playerPosition;
    [SerializeField] protected NavMeshAgent navMeshAgent;

    [Header("State Control")]
    [SerializeField] protected float aggroRange = 10.0f;
    [SerializeField] protected float attackRange = 2.0f;

    [Header("Roam Control")]
    [SerializeField] protected float roamRadius = 10.0f;
    [SerializeField] protected float roamWaitTime = 2.0f;

    protected float lastAttackTime = 0f;
    protected float roamWaitTimer = 0f;
    protected bool isWaiting = false;

    float distanceToPlayer = 0.0f;
    protected EnemyState currentState;

    private void Update()
    {
        if (health.currentHealth <= 0) return;

        playerPosition = playerController.transform.position;
        distanceToPlayer = Vector3.Distance(transform.position, playerPosition);

        switch(currentState)
        {
            case EnemyState.Roam: UpdateRoam(distanceToPlayer); break;
            case EnemyState.Aggro: UpdateAggro(distanceToPlayer); break;
            case EnemyState.Attack: UpdateAttack(distanceToPlayer); break;
        }

        animator.SetFloat("Speed", navMeshAgent.velocity.magnitude);
    }

    public void Initialise(EnemyDefinitionSO enemyDef)
    {
        playerController = FindFirstObjectByType<PlayerController>();
        playerPosition = playerController.transform.position;
        health = GetComponent<Health>();
        attack = GetComponent<Attack>();
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        currentState = EnemyState.Roam;

        aggroRange = enemyDef.aggroRange;
        attackRange = enemyDef.attackRange;
        roamRadius = enemyDef.roamRadius;
        roamWaitTime = enemyDef.roamWaitTime;
        attack.attackDamage = enemyDef.attackDamage;
        attack.attackCooldown = enemyDef.attackCooldown;
        health.maxHealth = enemyDef.maxHealth;
        health.currentHealth = enemyDef.maxHealth;
    }

    protected abstract void UpdateRoam(float distanceToPlayer);
    protected abstract void UpdateAggro(float distanceToPlayer);
    protected abstract void UpdateAttack(float distanceToPlayer);

    protected Vector3 GetRandomRoamPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, roamRadius, NavMesh.AllAreas);
        return hit.position;
    }
}
