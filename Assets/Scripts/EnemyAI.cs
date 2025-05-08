using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Threading.Tasks;

public class EnemyAI : MonoBehaviour
{
    [Header("AI Settings")]
    public float detectionRadius = 10f;
    public float attackRadius = 2f;
    public float patrolRadius = 15f;
    public float patrolCooldown = 3f;
    public Transform patrolGlobalPoint;
    public float waypointTolerance = 1f; // Jarak minimal untuk mencapai waypoint

    [Header("Combat Settings")]
    public Animator animator;
    public int maxHealth = 100;
    public int attackDamage = 10;
    public float attackCooldown = 1f;


    [Header("UI")]
    public HealthBar healthBar;
    public Vector3 healthBarOffset = new Vector3(0, 2f, 0);

    

    [Header("Others")]
    public GameObject fighter;


    private NavMeshAgent agent;
    private GameObject player;
    private int currentHealth;
    private float lastAttackTime;
    private bool death;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        healthBar.GetComponent<LookAt>().target = player.transform;
        Debug.Log(player);
        currentHealth = maxHealth;

        healthBar.maxHP = maxHealth;
        healthBar.currHP = currentHealth;
        SetNextPatrolPoint();
    }

    void Update()
    {
        UpdateHealthBarPosition();
        fighter.transform.position = transform.position;
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= attackRadius)
        {
            AttackPlayer();
        }
        else if (distanceToPlayer <= detectionRadius && agent.enabled)
        {
            ChasePlayer();
        }
        else if(!death)
        {
            Patrol();
        }
        

        UpdateAnimations();
    }

    void ChasePlayer()
    {
        agent.SetDestination(player.transform.position);
    }

    void AttackPlayer()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            animator.SetTrigger("PunchTrigger");
            PlayerHealth playerH = player.GetComponent<PlayerHealth>();
            lastAttackTime = Time.time;
        }
    }

    void Patrol()
    {
        // Cek jika sudah mencapai titik patroli
        if (agent.enabled && !agent.pathPending && agent.remainingDistance <= waypointTolerance)
        {
            SetNextPatrolPoint();
        }
    }

    void SetNextPatrolPoint()
    {
        Vector3 randomPoint = patrolGlobalPoint.transform.position + Random.insideUnitSphere * patrolRadius;
        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
        healthBar.currHP = currentHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            ChasePlayer(); // Mengejar player ketika diserang
        }
    }

    void Die()
    {
        death = true;
        agent.enabled = false;
        GetComponent<Collider>().enabled = false;
        animator.enabled = false;
        //healthBar.enabled = false;
        StartCoroutine(MoveToPosition(transform.position + new Vector3(0f, -3f, 0f), 1.75f));
        Destroy(gameObject, 2f);
    }

    void UpdateAnimations()
    {
        animator.SetBool("Walk Forward", agent.velocity.magnitude > 0.1f);
    }

    void UpdateHealthBarPosition()
    {
        healthBar.transform.position = transform.position + healthBarOffset;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
    IEnumerator MoveToPosition(Vector3 targetPos, float duration)
    {
        Vector3 startPos = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
    }
}