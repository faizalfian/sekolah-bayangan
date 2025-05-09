using TMPro;
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
    public float damageDelay = 0.5f;

    [Header("Effects")]
    public ParticleSystem attackEffect;

    [Header("Others")]
    public TextMeshProUGUI scoreText;

    private float nextAttackTime = 0f;
    private Animator animator;
    private PlayerInputAction playerInput;
    private InputAction punch;
    private bool isAttacking = false;


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

    private void Update()
    {
        scoreText.text = $"Score: {GameManager.Instance.GetCurrentScore()}\nHighscore: {GameManager.Instance.GetHighScore()}";
    }

    private void OnDisable()
    {
        
        punch.started -= Attack;
        punch.Disable();
    }

    void Attack(InputAction.CallbackContext context)
    {
        if (Time.time >= nextAttackTime && !isAttacking)
        {
            isAttacking = true;
            animator.SetTrigger("PunchTrigger");

            if (attackEffect != null) attackEffect.Play();

            Invoke("ApplyDamage", damageDelay);

            nextAttackTime = Time.time + 1f / attackRate;
        }
    }

    void ApplyDamage()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyAI>().TakeDamage(attackDamage);
        }
        isAttacking = false;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    public void onEnemyDead(int scoreVal)
    {
        GameManager.Instance.AddScore(scoreVal);
    }
}