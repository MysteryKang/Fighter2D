using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public CharacterController2D controller;
    // animator
    public Animator animator;

    //-- attacking cast
    public Transform attackPoint;
    public float attackRange;
    public LayerMask enemyLayers;

    public Transform groundCheck;
    public float radius;

    public float speed;

    const float maxHealth = 100;
    [SerializeField] private float health;
    public Slider HealthBar;

    float horizontal;
    bool jump = false;
    bool attacking = false;

    //public float attackRate = 100f;
    //float nextAttacttime = 0f;

    public float damage = 10f;

    // Start is called before the first frame update
    void Start()
    {
        HealthBar.value = 1.0f;
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal") * speed;

        if (Input.GetKeyDown(KeyCode.Space) && controller.m_Grounded && !jump)
        {
           // animator.SetTrigger("Jump");
            jump = true;
        }
 
        animator.SetBool("isGrounded", controller.m_Grounded);
        if (Mathf.Abs(horizontal) > 0) 
        {
            animator.SetBool("isMoving", true);
        }
        else {
            animator.SetBool("isMoving", false);
        }


        //   better use animator.setTrigger("Attack");
        //if (Time.time >= nextAttacttime)
        //{
        //    if (Input.GetKeyDown(KeyCode.J) && controller.m_Grounded)
        //    {
        //        Attack();
        //        nextAttacttime = Time.time + 1f / attackRate;
        //        attacking = true;
        //    }
        //}
        if (Input.GetKeyDown(KeyCode.J) && controller.m_Grounded) {
            Attack();
            attacking = true;
        }


        if (animator.GetCurrentAnimatorStateInfo(0).IsName("LightPunch")) {
            horizontal = 0f;
        }

        animator.SetBool("isGrounded", controller.m_Grounded);
        animator.SetFloat("yVelocity", GetComponent<Rigidbody2D>().velocity.y);
        animator.SetFloat("xVelocity", GetComponent<Rigidbody2D>().velocity.x);
    }

    private void FixedUpdate()
    {
        controller.Move(horizontal, false, jump);
        jump = false;

    }

    public void TakeHits(float damage) {
        health -= damage;
        if (health <= 0)
        {
            // player dies
            // play death anination;
            this.enabled = false;
        }
        else {
            HealthBar.value = health / maxHealth;
            if (health <= 30)
            {
                HealthBar.fillRect.GetComponent<Image>().color = Color.red;
            }
        }

    }

    private void Attack() {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("LightPunch")) // there needs better if statement
        {
            //animator.Play("LightPunch", 0, 0f);
            Debug.Log("is still playing animation");
        }
        else
        {
            // animator.SetTrigger("LightPunch");
            Debug.Log("start new animation");
        }
        animator.ResetTrigger("LightPunch");
        animator.SetTrigger("LightPunch");
        Collider2D[] enemyhits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in enemyhits) {
            enemy.GetComponent<HealthSystem>().TakeHits(damage);
        }
    }

    //private IEnumerator AttackEnemy() {
    //    animator.SetTrigger("Attack");
    //    yield return new WaitForSeconds(0.8235294f);
    //    attacking = false;
    //}

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        Gizmos.DrawWireSphere(groundCheck.position, radius);
    }
}
