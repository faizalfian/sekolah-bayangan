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
        if (agent.isStopped) agent.isStopped = false;
    }


    public override void Die(int _)
    {
        animator.SetTrigger("Dead");
        StartCoroutine(disableAfterDelay());
    }

    protected override void UpdateAnimations()
    {
        animator.SetBool("Walking", agent.velocity.magnitude > 0.2f);
        //animator.SetBool("Running", agent.velocity.magnitude > 2f && isChasingPlayer);
    }

    protected override void AttackPlayer()
    {
        if (isAttacking) return;
        if (!agent.isStopped)
        {
            Debug.Log("Stop?");
            agent.ResetPath();
            agent.velocity = Vector3.zero;
            agent.isStopped = true;
        }

        Quaternion lastRot = transform.rotation;
        transform.LookAt(playerTransform.position);
        transform.rotation = Quaternion.Lerp(lastRot, Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0), 0.5f);
        attackRotation = transform.rotation; // Simpan rotasi saat mulai serangan

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            isAttacking = true;
            //animator.ResetTrigger("Attack");
            animator.SetTrigger("Attack");

            // Lock rotation during attack
            StartCoroutine(LockAttackRotation());

            // Apply damage after animation delay
            StartCoroutine(ApplyDamageAfterDelay());

            lastAttackTime = Time.time;
        }
    }

}