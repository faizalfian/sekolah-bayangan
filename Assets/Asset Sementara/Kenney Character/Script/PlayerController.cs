using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
<<<<<<< Updated upstream
    public bool isFightingMode = false;
    public bool doingSkill = false;
=======
>>>>>>> Stashed changes

    private Rigidbody rb;
    private Animator animator;
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
<<<<<<< Updated upstream
        HandleFighting();
    }

    void HandleFighting()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
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
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            isFightingMode = true;
            animator.SetBool("isFightIdle", true);
            animator.SetTrigger("blast");
            animator.SetBool("isDoAction", true);
            StartCoroutine(ResetIsDoAction());
        }
=======
>>>>>>> Stashed changes
    }

    void HandleMovement()
    {
<<<<<<< Updated upstream
        if (animator.GetBool("isDoAction")) return;
=======
>>>>>>> Stashed changes
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(h, 0f, v).normalized;

        if (movement.magnitude > 0)
        {
<<<<<<< Updated upstream
            isFightingMode = false;
            animator.SetBool("isFightIdle", isFightingMode);
=======
>>>>>>> Stashed changes
            transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);
            transform.forward = movement; // Menghadap ke arah gerakan
        }

        animator.SetBool("isMoving", movement.magnitude > 0);
<<<<<<< Updated upstream

=======
>>>>>>> Stashed changes
    }

    void OnCollisionEnter(Collision collision)
    {
        // Cek jika menyentuh tanah
        if (collision.contacts[0].point.y < transform.position.y)
        {
            isGrounded = true;
        }
    }
<<<<<<< Updated upstream

    IEnumerator ResetIsDoAction()
    {
        yield return new WaitForSeconds(2f); // sesuaikan durasi animasi kick
        animator.SetBool("isDoAction", false);
    }
=======
>>>>>>> Stashed changes
}
