using UnityEngine;

public class BasicSwordSkeleton : EnemyController
{
    protected override void UpdateRoam(float distanceToPlayer)
    {
        if (distanceToPlayer < aggroRange)
        {
            currentState = EnemyState.Aggro;
            return;
        }

        if (isWaiting)
        {
            roamWaitTimer -= Time.deltaTime;
            if (roamWaitTimer <= 0f)
                isWaiting = false;
            return;
        }

        if (!navMeshAgent.hasPath || navMeshAgent.remainingDistance < 0.5f)
        {
            Vector3 roamTarget = GetRandomRoamPosition();
            navMeshAgent.SetDestination(roamTarget);
            isWaiting = true;
            roamWaitTimer = roamWaitTime;
        }
    }

    protected override void UpdateAggro(float distanceToPlayer)
    {
        if (distanceToPlayer > aggroRange)
        {
            currentState = EnemyState.Roam;
            navMeshAgent.ResetPath();
            return;
        }

        if (distanceToPlayer < attackRange)
        {
            currentState = EnemyState.Attack;
            navMeshAgent.ResetPath();
            return;
        }

        navMeshAgent.SetDestination(playerPosition);
    }

    protected override void UpdateAttack(float distanceToPlayer)
    {
        transform.LookAt(playerPosition);

        if (distanceToPlayer > attackRange)
        {
            currentState = EnemyState.Roam;
            return;
        }

        if (Time.time - lastAttackTime >= attack.attackCooldown)
        {
            lastAttackTime = Time.time;
            animator.SetTrigger("Attack");
            attack.DealDamage(playerController.GetComponent<Health>(), attack.attackDamage);
        }
    }
}
