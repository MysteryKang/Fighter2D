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

    public GameObject hadokenObject;

    //public float attackRate = 100f;
    //float nextAttacttime = 0f;
    public float hadokenRate = 0.3f;
    float nextAttackTime = 0f;

    public float damage = 10f;

    // Start is called before the first frame update
    void Start()
    {
        HealthBar.value = 1.0f;
        health = maxHealth;
    }

    public void SpawnHadoken() {
        GameObject hdk = Instantiate(hadokenObject);
        hdk.GetComponent<Hadoken>().direction = transform.localScale.x;
        hdk.transform.position = attackPoint.position;
        hdk.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal") * speed;

        if (Input.GetKeyDown(KeyCode.Space) && controller.m_Grounded && !jump && !attacking)
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


      
        if (Input.GetKeyDown(KeyCode.J) && controller.m_Grounded) {
            Attack();
        }

        if (Time.time >= nextAttackTime) {
            if (Input.GetKeyDown(KeyCode.K) && controller.m_Grounded)
            {
                Hadoken();
                nextAttackTime = Time.time + 1f / hadokenRate;
            }
        }


        if (animator.GetCurrentAnimatorStateInfo(0).IsName("LightPunch") || animator.GetCurrentAnimatorStateInfo(0).IsName("Hadoken"))
        {
            horizontal = 0f;
            attacking = true;
        }
        else {
            attacking = false;
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
            animator.SetTrigger("TakeHit");
        }

    }

    // hakoden animations
    private void Hadoken() {
        attacking = true;
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Hadoken"))
        {
            return;
        }
        else {
            animator.SetTrigger("Hadoken");
        }
    }

    private void Attack() {
        attacking = true;
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
