using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public float moveSpeed = 0.001f;
    public float jumpForce = 5f;
    public bool isFightingMode = false;
    public bool doingSkill = false;

    private Rigidbody rb;
    private Animator animator;
    public GameObject attackHitbox;
    private bool isGrounded = true;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleFighting();
    }

    void HandleFighting()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("Fighting Idle");
            Debug.Log(isFightingMode);
            isFightingMode = !isFightingMode;
            animator.SetBool("isFightIdle", isFightingMode);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            isFightingMode = true;
            animator.SetBool("isFightIdle", true);
            animator.SetTrigger("kick");
            animator.SetBool("isDoAction", true);
            StartCoroutine(ResetIsDoAction());
            StartCoroutine(ActivateHitbox());
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            isFightingMode = true;
            animator.SetBool("isFightIdle", true);
            animator.SetTrigger("blast");
            animator.SetBool("isDoAction", true);
            StartCoroutine(ResetIsDoAction());
        }
    }

    void HandleMovement()
    {
        if (animator.GetBool("isDoAction")) return;
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(h, 0f, v).normalized;

        if (movement.magnitude > 0)
        {
            isFightingMode = false;
            animator.SetBool("isFightIdle", isFightingMode);
            transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);
            transform.forward = movement; // Menghadap ke arah gerakan
        }

        animator.SetBool("isMoving", movement.magnitude > 0);
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

    IEnumerator ActivateHitbox()
    {
        Debug.Log("Attack from player controller");
        attackHitbox.SetActive(true);
        yield return new WaitForSeconds(0.2f); // durasi aktif hitbox
        attackHitbox.SetActive(false);
    }
}
