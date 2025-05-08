using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

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
    }

    void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(h, 0f, v).normalized;

        if (movement.magnitude > 0)
        {
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
}
