using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

[RequireComponent(typeof(Health)), RequireComponent(typeof(PlayerMovement))]
public class PlayerCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    public Transform attackPoint;
    public float attackRange = 1f;
    public LayerMask enemyLayers;
    public int attackDamage = 20;
    public float damageDelay = 0.5f;

    [Header("Movement Skills")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.3f;
    public float pushForce = 10f;
    public float pushRange = 2f;

    [Header("Combo System")]
    [SerializeField] private float comboWindow = 0.25f;
    [SerializeField] private Combo[] combos;

    [Header("Effects")]
    public ParticleSystem attackEffect;
    public ParticleSystem dashEffect;
    public ParticleSystem pushEffect;

    [System.Serializable]
    public class Combo
    {
        public string name;
        public InputAction[] sequence;
        public System.Action<PlayerCombat> execute;
        public float cooldown;
    }

    private PlayerInputAction inputActions;
    private Queue<InputAction> inputBuffer = new Queue<InputAction>();
    private Coroutine comboCheckRoutine;
    private PlayerMovement movement;
    private Animator animator;
    private float currentCooldown;

    private InputAction punchAction;
    private InputAction dashAction;
    private InputAction pushAction;


    void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        animator = GetComponent<PlayerMovement>().animator;
        inputActions = new PlayerInputAction();
        punchAction = inputActions.Player.Punch;
        dashAction = inputActions.Player.Dash;
        pushAction = inputActions.Player.Push;

        InitializeCombos();
    }

    void InitializeCombos()
    {
        combos = new Combo[]
        {
            new Combo
            {
                name = "DashPunch",
                sequence = new InputAction[] { inputActions.Player.Dash, inputActions.Player.Punch },
                execute = pc => pc.StartCoroutine(pc.DashPunchCombo()),
                cooldown = 1f
            },
            new Combo
            {
                name = "PushPunch",
                sequence = new InputAction[] { inputActions.Player.Push, inputActions.Player.Punch },
                execute = pc => pc.StartCoroutine(pc.PushPunchCombo()),
                cooldown = 0.75f
            },
            new Combo
            {
                name = "DashPush",
                sequence = new InputAction[] { inputActions.Player.Dash, inputActions.Player.Push },
                execute = pc => pc.StartCoroutine(pc.DashPushCombo()),
                cooldown = 0.5f
            }
        };
    }

    void OnEnable()
    {
        punchAction.performed += ctx => BufferInput(punchAction);
        dashAction.performed += ctx => BufferInput(dashAction);
        pushAction.performed += ctx => BufferInput(pushAction);
        punchAction.Enable();
        dashAction.Enable();
        pushAction.Enable();
    }

    void OnDisable()
    {
        punchAction.performed -= ctx => BufferInput(punchAction);
        dashAction.performed -= ctx => BufferInput(dashAction);
        pushAction.performed -= ctx => BufferInput(pushAction);
        punchAction.Disable();
        dashAction.Disable();
        pushAction.Disable();
    }

    void Update()
    {
        if (currentCooldown > 0) currentCooldown -= Time.deltaTime;
    }

    void BufferInput(InputAction action)
    {
        if (currentCooldown > 0) return;

        inputBuffer.Enqueue(action);
        if (comboCheckRoutine == null)
            comboCheckRoutine = StartCoroutine(CheckComboWindow());
    }

    IEnumerator CheckComboWindow()
    {
        float timer = 0;
        List<InputAction> currentSequence = new List<InputAction>();

        while (timer < comboWindow)
        {
            if (inputBuffer.Count > 0)
            {
                currentSequence.Add(inputBuffer.Dequeue());
                CheckCombo(currentSequence);
                timer = 0; // Reset timer on new input
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // Execute single action if no combo found
        if (currentSequence.Count > 0)
            ExecuteAction(currentSequence[0]);

        comboCheckRoutine = null;
    }

    void CheckCombo(List<InputAction> sequence)
    {
        foreach (var combo in combos)
        {
            if (SequenceMatch(combo.sequence, sequence))
            {
                combo.execute(this);
                currentCooldown = combo.cooldown;
                inputBuffer.Clear();
                return;
            }
        }
    }

    bool SequenceMatch(InputAction[] comboSequence, List<InputAction> inputSequence)
    {
        if (inputSequence.Count != comboSequence.Length) return false;

        for (int i = 0; i < comboSequence.Length; i++)
            if (comboSequence[i] != inputSequence[i])
                return false;

        return true;
    }

    void ExecuteAction(InputAction action)
    {
        if (action == inputActions.Player.Punch) StartCoroutine(PerformAttack());
        else if (action == inputActions.Player.Dash) StartCoroutine(PerformDash());
        else if (action == inputActions.Player.Push) StartCoroutine(PerformPush());
    }

    IEnumerator PerformAttack()
    {
        animator.SetTrigger("PunchTrigger");
        //attackEffect?.Play();
        yield return new WaitForSeconds(damageDelay);

        var hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);
        foreach (var enemy in hitEnemies)
            enemy.GetComponent<Health>().TakeDamage(attackDamage);
    }

    IEnumerator PerformDash()
    {
        movement.LockMovement(true);
        animator.SetTrigger("PunchTrigger");
        //animator.SetTrigger("Dash");
        //dashEffect?.Play();

        Vector3 direction = transform.forward;
        float timer = 0;

        while (timer < dashDuration)
        {
            movement.Move(direction * dashSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        movement.LockMovement(false);
    }

    IEnumerator PerformPush()
    {
        movement.LockMovement(true);
        animator.SetTrigger("PunchTrigger");
        //animator.SetTrigger("Push");
        //pushEffect?.Play();

        var hitEnemies = Physics.OverlapSphere(attackPoint.position, pushRange, enemyLayers);
        foreach (var enemy in hitEnemies)
        {
            var agent = enemy.GetComponent<NavMeshAgent>();
            if (agent) StartCoroutine(PushEnemy(agent));
        }

        yield return new WaitForSeconds(0.5f);
        movement.LockMovement(false);
    }

    IEnumerator PushEnemy(NavMeshAgent agent)
    {
        var rb = agent.gameObject.AddComponent<Rigidbody>();
        rb.AddForce(transform.forward * pushForce, ForceMode.Impulse);

        yield return new WaitForSeconds(0.5f);

        Destroy(rb);
        agent.enabled = true;
    }

    // Combo Implementations
    IEnumerator DashPunchCombo()
    {
        movement.LockMovement(true);
        animator.SetTrigger("PunchTrigger");
        //animator.SetTrigger("DashPunch");
        //dashEffect?.Play();

        float timer = 0;
        Vector3 direction = transform.forward;
        bool damageApplied = false;

        while (timer < dashDuration)
        {
            movement.Move(direction * dashSpeed * 1.7f * Time.deltaTime);
            timer += Time.deltaTime;

            if (timer >= dashDuration / 2 && !damageApplied)
            {
                ApplyDashDamage(direction, Mathf.RoundToInt(attackDamage * 0.75f));
                damageApplied = true;
            }

            yield return null;
        }

        movement.LockMovement(false);
    }

    IEnumerator DashPushCombo()
    {
        movement.LockMovement(true);
        animator.SetTrigger("PunchTrigger");
        //dashEffect?.Play();
        //pushEffect?.Play();

        float timer = 0;
        Vector3 direction = transform.forward;
        bool effectApplied = false;

        while (timer < dashDuration * 1.2f) // Durasi lebih panjang untuk combo
        {
            movement.Move(direction * dashSpeed * 0.8f * Time.deltaTime);
            timer += Time.deltaTime;

            if (timer >= dashDuration / 2 && !effectApplied)
            {
                ApplyDashPushEffect(direction);
                effectApplied = true;
            }

            yield return null;
        }

        movement.LockMovement(false);
    }

    IEnumerator PushPunchCombo()
    {
        movement.LockMovement(true);
        animator.SetTrigger("PunchTrigger");
        //animator.SetTrigger("PushPunch");
        //pushEffect.Play();

        var hitEnemies = Physics.OverlapSphere(attackPoint.position, pushRange * 1.5f, enemyLayers);
        foreach (var enemy in hitEnemies)
        {
            enemy.GetComponent<Health>().TakeDamage(Mathf.FloorToInt(attackDamage * 1.5f));
            var agent = enemy.GetComponent<NavMeshAgent>();
            if (agent) StartCoroutine(PushEnemy(agent));
        }

        yield return new WaitForSeconds(0.5f);
        movement.LockMovement(false);
    }

    void ApplyDashDamage(Vector3 direction, int damage)
    {
        var hitEnemies = Physics.OverlapSphere(transform.position + direction * 2f, 3f, enemyLayers);
        foreach (var enemy in hitEnemies)
            enemy.GetComponent<Health>().TakeDamage(damage);
    }

    void ApplyDashPushEffect(Vector3 direction)
    {
        var hitEnemies = Physics.OverlapSphere(transform.position + direction * 2f, 3f, enemyLayers);
        foreach (var enemy in hitEnemies)
        {
            // Damage kecil untuk combo ini
            Health enemyHealth = enemy.GetComponent<Health>();
            enemyHealth.TakeDamage(Mathf.RoundToInt(attackDamage * 0.5f));

            // Efek knockback kuat
            var agent = enemy.GetComponent<NavMeshAgent>();
            if (agent) StartCoroutine(PushEnemy2(agent, pushForce * 0.5f));
        }
    }

    IEnumerator PushEnemy2(NavMeshAgent agent, float force)
    {
        var rb = agent.gameObject.AddComponent<Rigidbody>();
        rb.AddForce(transform.forward * force, ForceMode.Impulse);

        yield return new WaitForSeconds(0.8f); // Durasi knockback lebih lama

        if (rb != null) Destroy(rb);
        if (agent != null) agent.enabled = true;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + transform.forward * 2f, 3f);
    }
}