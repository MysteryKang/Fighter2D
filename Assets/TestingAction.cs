using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingAction : MonoBehaviour
{
    public CharacterController2D controller;
    bool jump = false;
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.m_Grounded)
        {
            jump = true;
        }
        else {
            jump = false;
        }
        animator.SetFloat("yVelocity", GetComponent<Rigidbody2D>().velocity.y);

    }

    private void FixedUpdate()
    {
        controller.Move(0f, false, jump);
    }
}
