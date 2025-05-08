using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterController : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;
    public float moveSpeed = 5f;
    public float attackDelay = 0.5f;
    private bool canAttack = true;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Move();
        if (Input.GetKeyDown(KeyCode.J) && canAttack)
        {
            StartCoroutine(Attack());
            Debug.Log("Attack");
        }
        if (Input.GetKeyDown(KeyCode.K) && canAttack)
        {
            StartCoroutine(HeavyAttack());
        }
    }

    void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        Vector2 moveDirection = new Vector2(horizontal, vertical).normalized;
        rb.velocity = moveDirection * moveSpeed;

        animator.SetFloat("Speed", moveDirection.sqrMagnitude);
    }

    IEnumerator Attack()
    {
        canAttack = false;
        animator.SetTrigger("LightAttack");
        yield return new WaitForSeconds(attackDelay);
        canAttack = true;
    }

    IEnumerator HeavyAttack()
    {
        canAttack = false;
        animator.SetTrigger("HeavyAttack");
        yield return new WaitForSeconds(attackDelay * 2);
        canAttack = true;
    }
}