using UnityEngine;



public class EnemyBossAI : EnemyAI
{


    protected override void Patrol()
    {
        // Cek jika sudah mencapai titik patroli
        if (patrolRadius <= 1 && Vector3.Distance(transform.position, player.transform.position) > 0.2)
        {
            agent.SetDestination(patrolGlobalPoint.transform.position);
            return;
        }
        if (agent.enabled && !agent.pathPending && agent.remainingDistance <= waypointTolerance)
        {
            SetNextPatrolPoint();
        }
    }
}