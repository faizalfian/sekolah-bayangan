using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent)), RequireComponent(typeof(Health))]
public class EnemyAI : MonoBehaviour
{
    [Header("AI Settings")]
    public float detectionRadius = 4f;
    public float attackRadius = 2f;
    public float patrolRadius = 7f;
    public Transform patrolGlobalPoint;
    public float waypointTolerance = 1f; // Jarak minimal untuk mencapai waypoint

    [Header("Combat Settings")]
    public Animator animator;
    public int maxHealth = 100;
    public int attackDamage = 2;
    public float attackCooldown = 1f;
    public int scoreValue = 50; // Nilai score yang diberikan saat musuh mati
    public float mass = 2f;

    [Header("Attack Settings")]
    public float attackAnimationDelay = 0.5f; // Waktu delay sebelum mengurangi HP
    protected bool isAttacking = false;
    protected Quaternion attackRotation;


    [Header("UI")]
    public HealthBar healthBar;
    public Vector3 healthBarOffset = new Vector3(0, 2f, 0);


    //[Header("Others")]
    //public GameObject fighter;
    //[SerializeField] private UnityEvent<int> onEnemyDeath;

    [SerializeField]
    public GameManager gm;

    protected NavMeshAgent agent;
    protected GameObject player;
    protected Health health;
    protected Transform playerTransform;
    protected float lastAttackTime;

    protected void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.avoidancePriority = Random.Range(50, 100);
        player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player.transform;
        health = GetComponent<Health>();
        // SetNextPatrolPoint();
        gm.addEnemy();
    }

    void Update()
    {
        //Debug.Log(gm.isPlaying);
        if (!gm.isPlaying)
        {
            if (!agent.isStopped)
            {
                Debug.Log("Stop?");
                agent.ResetPath();
                agent.velocity = Vector3.zero;
                agent.isStopped = true;
            }
            return;
        }

        if (health.isDeath() || player == null) return;
        UpdateHealthBarPosition();
        //fighter.transform.position = transform.position;
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= attackRadius && !player.GetComponent<Health>().isDeath())
        {
            AttackPlayer();
        }
        else if (distanceToPlayer <= detectionRadius && distanceToPlayer > attackRadius && agent.enabled && !player.GetComponent<Health>().isDeath())
        {
            ChasePlayer();
        }
        else if (patrolGlobalPoint != null)
        {
            Patrol();
        }


        UpdateAnimations();
    }

    protected virtual void ChasePlayer()
    {
        transform.LookAt(playerTransform.position);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        agent.SetDestination(player.transform.position);
        if (agent.isStopped) agent.isStopped = false;
    }

    protected virtual void AttackPlayer()
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

    protected virtual void Patrol()
    {
        // Cek jika sudah mencapai titik patroli
        if (agent.enabled && !agent.pathPending && agent.remainingDistance <= waypointTolerance)
        {
            SetNextPatrolPoint();
        }
        if (agent.isStopped) agent.isStopped = false;
    }

    protected void SetNextPatrolPoint()
    {
        Vector3 randomPoint = patrolGlobalPoint.transform.position + Random.insideUnitSphere * patrolRadius;
        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    public virtual void Die(int _)
    {
        animator.SetTrigger("Dead");
        gm.removeEnemy();
        StartCoroutine(disableAfterDelay());
    }

    protected virtual void UpdateAnimations()
    {
        animator.SetBool("Walking", agent.velocity.magnitude > 0.1f);
    }

    protected void UpdateHealthBarPosition()
    {
        healthBar.transform.position = transform.position + healthBarOffset;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        if (patrolGlobalPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(patrolGlobalPoint.position, patrolRadius);
        }
    }
    protected IEnumerator LockAttackRotation()
    {
        // Tunggu sampai animasi serangan selesai
        float attackTime = 0f;
        float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;

        while (attackTime < animationLength)
        {
            transform.rotation = attackRotation; // Pertahankan rotasi serangan
            attackTime += Time.deltaTime;
            yield return null;
        }

        isAttacking = false;
    }

    protected IEnumerator ApplyDamageAfterDelay()
    {
        yield return new WaitForSeconds(attackAnimationDelay);

        if (player != null && Vector3.Distance(transform.position, player.transform.position) <= attackRadius * 1.2f)
        {
            Health playerH = player.GetComponent<Health>();
            Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
            if (playerH != null && angleToPlayer < 60f)
            {
                playerH.TakeDamage(attackDamage);
            }

        }
    }

    protected IEnumerator disableAfterDelay()
    {
        yield return new WaitForSeconds(0.65f);
        gameObject.SetActive(false);
    }
}