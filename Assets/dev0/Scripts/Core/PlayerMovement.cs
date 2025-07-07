using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float jumpHeight = 12f;
    public float jumpCooldown = 1f;
    public float gravity = -8f;
    public float rotationSpeed = 10f;

    [Header("Other")]
    public Animator animator;

    // Input System
    public PlayerInputAction playerInput;
    private InputAction moveAction;
    private InputAction jumpAction;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float currentSpeed;
    private Vector2 moveInput;
    private Vector3 moveDir;
    private bool isJumping = false;

    void Awake()
    {
        // Initialize Input System
        playerInput = new PlayerInputAction();
        moveAction = playerInput.Player.Move;
        jumpAction = playerInput.Player.Jump;

        controller = GetComponent<CharacterController>();
        currentSpeed = walkSpeed;
    }

    void OnEnable()
    {
        // Enable input actions
        moveAction.Enable();
        jumpAction.Enable();
        jumpAction.performed += JumpAction_performed;
    }


    void OnDisable()
    {
        // Disable input actions
        moveAction.Disable();
        jumpAction.Disable();
        jumpAction.performed -= JumpAction_performed;
    }

    void FixedUpdate()
    {
        if(GetComponent<Health>().isDeath()) return;
        HandleGravity();
        HandleMovement();
    }

    void Update()
    {
        isGrounded = controller.isGrounded;
        if(GetComponent<Health>().isDeath()) return;
        HandleAnimations();
    }
    private void JumpAction_performed(InputAction.CallbackContext obj)
    {
        if(isJumping || !isGrounded || !GameManager.Instance.isPlaying) return;
        isJumping = true;
        animator.SetTrigger("jump");
        velocity.y += jumpHeight;
        StartCoroutine(resetJump());
    }

    void HandleAnimations()
    {
        // player bergerak
        if((moveInput.x > 0 || moveInput.x < 0) || (moveInput.y > 0 || moveInput.y < 0))
        {
            float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;

            // Snap ke kelipatan 45 derajat terdekat
            targetAngle = Mathf.Round(targetAngle / 15) * 15;

            transform.eulerAngles = new Vector3(0, targetAngle, 0);
            animator.SetBool("running", true);
        // player diam
        } else {
            animator.SetBool("running", false);
        }
        
    }

    private void HandleMovement()
    {
        // Get input from Input System
        moveInput = moveAction.ReadValue<Vector2>();
        moveDir = new Vector3(moveInput.x, velocity.y, moveInput.y).normalized;

        // Move character
        //Debug.Log(currentSpeed);
        controller.Move(currentSpeed * Time.deltaTime * moveDir);
    }

    private void HandleGravity()
    {
        if (isGrounded && velocity.y < 0) velocity.y = 0;
        if (transform.position.y < -20) GetComponent<Health>().TakeDamage(1000);
        if(!isGrounded) velocity.y += gravity * Time.deltaTime;
    }

    private IEnumerator resetJump()
    {
        yield return new WaitForSeconds(jumpCooldown);

        isJumping = false;
    }


    public void resetMovement()
    {
        moveDir = Vector3.zero;
        moveInput = Vector2.zero;
        velocity = Vector3.zero;
    }

    public void Move(Vector3 dir)
    {
        controller.Move(dir);
    }
    public void LockMovement(bool shouldLock)
    {
        if (shouldLock)
        {
            moveAction.Disable();   
        }
        else
        {
            moveAction.Enable();
        }
    }

    public void enableMovement(bool enable)
    {
        if(!enable)
        {
            animator.ResetTrigger("jump");
            animator.SetBool("running", false);
        }
        controller.enabled = enable;
        enabled = enable;
        LockMovement(!enable);
    }

}