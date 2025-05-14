using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class BimaMvController : MonoBehaviour
{
    [SerializeField] public float moveSpeed = 1.5f;
    public float jumpForce = 5f;
    public bool isFightingMode = false;
    private bool isRolling = false;
    public bool doingSkill = false;

    private Rigidbody rb;
    private Animator animator;
    public GameObject attackHitbox;
    private bool isGrounded = true;

    private InputAction moveAction;
    public PlayerInputAction playerInput;

    private CharacterController controller;
    private float currentSpeed;
    private Vector2 moveInput;


    void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = new PlayerInputAction();
        moveAction = playerInput.Player.Move;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
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

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        Vector2 move = moveAction.ReadValue<Vector2>();
        float h = move.x;
        float v = move.y;

        Vector3 movement = new Vector3(h, 0f, v).normalized;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            moveSpeed = 2;
            isRolling = true;
            // animator.SetBool("isFightIdle", true);
            animator.SetTrigger("tRoll");
            StartCoroutine(ResetIsRolling());
        }

        if (movement.magnitude > 0)
        {
            // isFightingMode = false;
            // animator.SetBool("isFightIdle", isFightingMode);
            // transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);
            controller.Move(movement * moveSpeed * Time.deltaTime);
            transform.forward = movement; // Menghadap ke arah gerakan
        }

        animator.SetBool("isMoving", movement.magnitude > 0);
    }

    public void Move(Vector3 dir)
    {
        controller.Move(dir);
    }



    void OnCollisionEnter(Collision collision)
    {
        // Cek jika menyentuh tanah
        if (collision.contacts[0].point.y < transform.position.y)
        {
            isGrounded = true;
        }
    }

    IEnumerator ResetIsDoAction()
    {
        yield return new WaitForSeconds(2f); // sesuaikan durasi animasi kick
        animator.SetBool("isDoAction", false);
    }

    IEnumerator ResetIsRolling()
    {
        yield return new WaitForSeconds(2f);
        isRolling = false;
        moveSpeed = 1.5f;
    }

    IEnumerator ActivateHitbox()
    {
        Debug.Log("Attack from player controller");
        attackHitbox.SetActive(true);
        yield return new WaitForSeconds(0.2f); // durasi aktif hitbox
        attackHitbox.SetActive(false);
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
}
