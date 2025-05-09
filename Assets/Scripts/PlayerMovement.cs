using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float jumpHeight = 3f;
    public float gravity = -20f;
    public float rotationSpeed = 10f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public float groundCheckOffset = 0.1f;

    [Header("Slope Handling")]
    public float slopeForce = 5f;
    public float slopeForceRayLength = 2f;

    [Header("Other")]
    public Animator animator;
    public GameObject fighter;
    public GameObject AtkPoint;
    public Vector3 atkPointOffset;

    // Input System
    public PlayerInputAction playerInput;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction punchAction;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float currentSpeed;
    private Vector2 moveInput;
    private Vector3 moveDir;
    //private PlayerCombat playerCombat;

    void Awake()
    {
        // Initialize Input System
        playerInput = new PlayerInputAction();
        moveAction = playerInput.Player.Move;
        //sprintAction = playerInput.actions["Sprint"];

        controller = GetComponent<CharacterController>();
        currentSpeed = walkSpeed;
    }

    void OnEnable()
    {
        // Enable input actions
        moveAction.Enable();
        //punchAction.Enable();
        //sprintAction.Enable();

        // Setup input callbacks
        //sprintAction.canceled += OnSprintEnd;
    }

    void OnDisable()
    {
        // Disable input actions
        moveAction.Disable();
        //sprintAction.Disable();

        // Remove callbacks
        //sprintAction.started -= OnSprintStart;
        //sprintAction.canceled -= OnSprintEnd;
    }

    void Update()
    {
        
        HandleGroundCheck();
        HandleGravity();
        HandleMovement();
        HandleAnimations();
        //HandleSlope();
    }

    void HandleAnimations()
    {
        // player bergerak
        if((moveInput.x > 0 || moveInput.x < 0) || (moveInput.y > 0 || moveInput.y < 0))
        {
            //if (moveInput.x > 0)
            //{
            //    fighter.transform.rotation = Quaternion
            //}
            float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;

            // Snap ke kelipatan 45 derajat terdekat
            targetAngle = Mathf.Round(targetAngle / 45) * 45;

            // Terapkan rotasi
            AtkPoint.transform.position = transform.position + atkPointOffset;
            fighter.transform.rotation = Quaternion.Euler(0, targetAngle, 0);
            Vector3 dir = AtkPoint.transform.position - fighter.transform.position; // 1. Relative position
            dir = fighter.transform.rotation * dir; // 2. Rotate
            Vector3 point = dir + fighter.transform.position; // 3. Final position
            AtkPoint.transform.position = point;
            // animasi berjalan
            animator.SetBool("Walk Forward", true);
        // player diam
        } else {
            animator.SetBool("Walk Forward", false);
        }
        
    }

    private void HandleGroundCheck()
    {
        isGrounded = Physics.CheckSphere(
            groundCheck.position + (Vector3.down * groundCheckOffset),
            groundDistance,
            groundMask
        );

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = 0;
        }
    }

    private void HandleMovement()
    {
        // Get input from Input System
        moveInput = moveAction.ReadValue<Vector2>();
        fighter.transform.position = groundCheck.transform.position;
        moveDir = new Vector3(-moveInput.y, velocity.y, moveInput.x).normalized;

        
        // Move character
        controller.Move(moveDir * currentSpeed * Time.deltaTime);
    }

    private void HandleGravity()
    {
        velocity.y += gravity * Time.deltaTime;
    }

    private void HandleSlope()
    {
        if ((moveInput.x != 0 || moveInput.y != 0) && OnSlope())
        {
            controller.Move(Vector3.down * controller.height / 2 * slopeForce * Time.deltaTime);
        }
    }

    private bool OnSlope()
    {
        if (!isGrounded) return false;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, controller.height / 2 * slopeForceRayLength))
        {
            return hit.normal != Vector3.up;
        }
        return false;
    }

    //private void OnSprintStart(InputAction.CallbackContext context)
    //{
    //    isSprinting = true;
    //    currentSpeed = sprintSpeed;
    //}

    //private void OnSprintEnd(InputAction.CallbackContext context)
    //{
    //    isSprinting = false;
    //    currentSpeed = walkSpeed;
    //}

    // Debug Visualization
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position + (Vector3.down * groundCheckOffset), groundDistance);
    }
}