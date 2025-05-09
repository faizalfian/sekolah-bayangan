using UnityEngine;


// BUG: animasi menyerang belum selesai tapi bos sudah menyerang (hp berkurang)

public class EnemyBossAI : EnemyAI
{
    

    void Update()
    {

        if (death || player == null) return;
        UpdateHealthBarPosition();
        fighter.transform.position = transform.position;
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= attackRadius)
        {
            AttackPlayer();
        }
        else if (distanceToPlayer <= detectionRadius && agent.enabled)
        {
            ChasePlayer();
        }
        else if (patrolGlobalPoint != null)
        {
            Patrol();
        }
        

        UpdateAnimations();
    }
}