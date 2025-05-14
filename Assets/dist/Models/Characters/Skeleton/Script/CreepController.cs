using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreepController : MonoBehaviour
{

    [SerializeField] public float moveSpeed = 0.001f;
    public bool doingSkill = false;

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
    }


}
