using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    public Transform attackPoint;
    public float attackRange = 1f;
    public LayerMask enemyLayers;
    public int attackDamage = 20;
    public float attackRate = 2f;

    [Header("Effects")]
    public ParticleSystem attackEffect;

    private float nextAttackTime = 0f;
    private Animator animator;
    private PlayerInputAction playerInput;
    private InputAction punch;


    void Awake()
    {

        playerInput = new PlayerInputAction();
        punch = playerInput.Player.Punch;
    }

    void Start()
    {
        animator = GetComponent<PlayerMovement>().animator;
    }

    private void OnEnable()
    {
        punch.Enable();
        punch.started += Attack;
    }

    private void OnDisable()
    {
        
        punch.started -= Attack;
        punch.Disable();
    }

    //void Update()
    //{
    //    if (Time.time >= nextAttackTime)
    //    {
    //        if (Input.GetMouseButtonDown(0))
    //        {
    //            Attack();
    //            nextAttackTime = Time.time + 1f / attackRate;
    //        }
    //    }
    //}

    void Attack(InputAction.CallbackContext context)
    {
        if (Time.time >= nextAttackTime)
        {

            animator.SetTrigger("PunchTrigger");

            if (attackEffect != null)
                attackEffect.Play();

            Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

            foreach (Collider enemy in hitEnemies)
            {
                enemy.GetComponent<EnemyAI>().TakeDamage(attackDamage);
            }
            nextAttackTime = Time.time + 1f / attackRate;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}