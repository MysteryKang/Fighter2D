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
    public Transform attackPoint_Back;  // for the spinning kick purpose
    public float attackRange;
    public LayerMask enemyLayers;

    public Transform groundCheck;
    public float radius;

    public float speed;

    const float maxHealth = 1000f;
    [SerializeField] private float health;
    public Slider HealthBar;

    [SerializeField] private float horizontal;
    bool jump = false;
    public bool attacking = false;

    public float hadokenRate = 0.3f;
    float nextAttackTime = 0f;

    public float damage = 1f;

    [SerializeField] private Vector2 upperCutForce;

    bool isUpperCutting = false;

    public static bool canSpawnHadoken = true;

    public float spinningKickHorizontalSpeed;

    [SerializeField] private float force;
    // Start is called before the first frame update
    void Start()
    {
        HealthBar.value = 1.0f;
        health = maxHealth;
    }

    // --- spawning hadoken here and assign <Hadoken> class to the gameobject
    public void SpawnHadoken() {
        GameObject hdk = new GameObject();
        hdk.transform.position = attackPoint.position;
        hdk.AddComponent<SpriteRenderer>();
        hdk.GetComponent<SpriteRenderer>().sprite = Resources.Load("Hadoken", typeof(Sprite)) as Sprite;
        hdk.AddComponent<PolygonCollider2D>();
        hdk.GetComponent<PolygonCollider2D>().isTrigger = true;
        hdk.AddComponent<Hadoken>();
        hdk.GetComponent<Hadoken>().direction = transform.localScale.x / Mathf.Abs(transform.localScale.x);
        hdk.gameObject.name = "hadoken";
        canSpawnHadoken = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!attacking)
        {
            horizontal = Input.GetAxisRaw("Horizontal") * speed;
        }
        else {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("SpinningKick"))
            {
                return;
            }
            else {
                horizontal = 0f;
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Space) && controller.m_Grounded && !jump && !attacking)
        {
            jump = true;
        }
 
        animator.SetBool("isGrounded", controller.m_Grounded);

        //------- determine whether it is moving horizontally or not
        if (Mathf.Abs(horizontal) > 0) 
        {
            animator.SetBool("isMoving", true);
        }
        else {
            animator.SetBool("isMoving", false);
        }
        // ---------


        // ------ airborne punch or airborne kick
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle_Jump_Up") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("Idle_Jump_Down")||
            animator.GetCurrentAnimatorStateInfo(0).IsName("Rolling_Jump")) {
            if (Input.GetKeyDown(KeyCode.J))
            {
                AirBornePunch();
            }
            else if (Input.GetKeyDown(KeyCode.K)) {
                AirborneKick();
            }
        }

      
        if (Input.GetKeyDown(KeyCode.J) && controller.m_Grounded) {
            LightPunch();
        }
        if (Input.GetKeyDown(KeyCode.K) && controller.m_Grounded)
        {
            Kick();
        }

        if (canSpawnHadoken) {
            if (Input.GetKeyDown(KeyCode.I) && controller.m_Grounded)
            {
                Hadoken();
            }
        }

        // if any of {LightPunch, Hadoken, Kick}'s  animation is Playing stop moving horizontally
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("LightPunch") || animator.GetCurrentAnimatorStateInfo(0).IsName("Hadoken")
            || animator.GetCurrentAnimatorStateInfo(0).IsName("Kick"))
        {
            horizontal = 0f;
            attacking = true;
        }
       
        //if (animator.GetCurrentAnimatorStateInfo(0).IsName("Ryu") || animator.GetCurrentAnimatorStateInfo(0).IsName("Ryu_Running")) {
        //    attacking = false;
        //}

        // UpperCutting
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("UpperCut")) {
            if (isUpperCutting == true)
            {
                horizontal = 0f;
            }
            else
            {
                horizontal *= 0.3f;
            }
        }

        if (Input.GetKeyDown(KeyCode.U) && controller.m_Grounded && !attacking) {
            UpperCut();
        }

        if (Input.GetKeyDown(KeyCode.H) && !animator.GetCurrentAnimatorStateInfo(0).IsName("SpinningKick") && controller.m_Grounded)
        {
            SpinningKick();
        }

        animator.SetBool("isGrounded", controller.m_Grounded);
        animator.SetFloat("yVelocity", GetComponent<Rigidbody2D>().velocity.y);
        animator.SetFloat("xVelocity", GetComponent<Rigidbody2D>().velocity.x);
    }

    private void PushBackward(Rigidbody2D rd) {
        float direction = controller.m_FacingRight == true ? 1f : -1f;  
        rd.AddForce(new Vector2(10f * direction, 0f));
    }

    private void FixedUpdate()
    {
        controller.Move(horizontal, false, jump);
        jump = false;
    }

    #region Hadoken 
    //  ---------- hakoden animations
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

    #endregion

    #region  Light Punch

    //  ----------- light punch
    private void LightPunch() {  
        attacking = true;
        animator.SetTrigger("LightPunch");
        Collider2D[] enemyhits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in enemyhits) {
            if (enemy.GetComponent<HealthSystem>().isInvincible)
                return;
            else
            {
                enemy.GetComponent<HealthSystem>().TakeHits(damage);
                PushBackward(enemy.GetComponent<Rigidbody2D>());
                enemy.transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 1f);
                float direction = controller.m_FacingRight == true ? 1f : -1f;
                enemy.GetComponent<Rigidbody2D>().AddForce(new Vector2(force * direction, 0f));
            }
        }
    }
    #endregion

    #region LightKick
    private void Kick() {
        attacking = true;
        animator.SetTrigger("Kick");
        Collider2D[] enemyhits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in enemyhits)
        {
            if (enemy.GetComponent<HealthSystem>().isInvincible)
                return;
            else
            {
                enemy.GetComponent<HealthSystem>().TakeHits(damage);
                PushBackward(enemy.GetComponent<Rigidbody2D>());
                enemy.transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 1f);
                float direction = controller.m_FacingRight == true ? 1f : -1f;
                enemy.GetComponent<Rigidbody2D>().AddForce(new Vector2(force * direction, 0f));
            }
        }
    }
    #endregion


    #region   UpperCut

    private void UpperCut() {
        attacking = true;
        horizontal = controller.m_FacingRight ? 0.5f : -0.5f;
        animator.SetTrigger("UpperCut");
    }

    // ------- trigger at animation event
    public void UpperCutJump() {
        isUpperCutting = false;
        controller.m_AirControl = false;
        Vector2 force = upperCutForce;
        if (controller.m_FacingRight)
        {
            force.x *= 1f;
        }
        else {
            force.x *= -1f;
        }
        jump = true;
        horizontal *= 0.3f;
    }
    #endregion

    #region Spinning Kick
    public void SpinningKick() {
        attacking = true;
        animator.SetTrigger("SpinningKick");
        GetComponent<Rigidbody2D>().gravityScale = 0f;
        transform.position += new Vector3(0f, 0.6f);
        StartCoroutine(SpinningKickMovement());
    }

    IEnumerator SpinningKickMovement() {
        Debug.Log(attacking);
        float direction = transform.localScale.x / Mathf.Abs(transform.localScale.x);
        spinningKickHorizontalSpeed *= direction;
        horizontal = 1f * direction * 0.5f;
        Debug.Log(horizontal);
        yield return new WaitForSeconds(1f);
        Debug.Log(attacking);
        Debug.Log(horizontal);
        GetComponent<Rigidbody2D>().gravityScale = 5f;
    }

  //  spinning kick push the enemy both ways   * codes will be simplified later
    public void SpinningKickToEnemy() {
        Collider2D[] enemyhits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in enemyhits)
        {
            if (enemy.GetComponent<HealthSystem>().isInvincible)
                return;
            else
            {
                enemy.GetComponent<HealthSystem>().TakeHits(damage);
                enemy.GetComponent<Animator>().SetTrigger("TakenDown");
                enemy.GetComponent<HealthSystem>().SleepAndWakeUp();
                enemy.transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 1f);
                float direction = controller.m_FacingRight == true ? 1f : -1f;
                enemy.GetComponent<Rigidbody2D>().AddForce(new Vector2(upperCutForce.x * direction * 4f, upperCutForce.y / 2f));
            }
        }
        Collider2D[] enemyhits2 = Physics2D.OverlapCircleAll(attackPoint_Back.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in enemyhits2)
        {
            if (enemy.GetComponent<HealthSystem>().isInvincible)
                return;
            else
            {
                enemy.GetComponent<HealthSystem>().TakeHits(damage);
                enemy.GetComponent<Animator>().SetTrigger("TakenDown");
                enemy.GetComponent<HealthSystem>().SleepAndWakeUp();
                enemy.transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 1f);
                float direction = controller.m_FacingRight == true ? -1f : 1f;
                enemy.GetComponent<Rigidbody2D>().AddForce(new Vector2(upperCutForce.x * direction * 4f, upperCutForce.y / 2f));
            }
        }
    }

    public void ResetGravity()
    {
        GetComponent<Rigidbody2D>().gravityScale = 5f;
    }
    #endregion

    // ------ heavy attack caused by upper cut ,  lift up the enemy and move  horizontally 
    public void HeavyAttack()
    {
        Collider2D[] enemyhits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in enemyhits)
        {
            if (enemy.GetComponent<HealthSystem>().isInvincible)
                return;
            else {
                enemy.GetComponent<HealthSystem>().TakeHits(damage);
                enemy.GetComponent<Animator>().SetTrigger("TakenDown");
                enemy.GetComponent<HealthSystem>().SleepAndWakeUp();
                enemy.transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 1f);
                float direction = controller.m_FacingRight == true ? 1f : -1f;
                enemy.GetComponent<Rigidbody2D>().AddForce(new Vector2(upperCutForce.x * direction, upperCutForce.y));
            }
        }
    }

    #region Airborne Punch and Airborne Kick 
    private void AirBornePunch() {
        attacking = true;
     //   LightAttack();
        animator.SetTrigger("AirbornePunch");
    }
    private void AirborneKick() {
        attacking = true;
    //    LightAttack();
        animator.SetTrigger("AirborneKick");
    }
    #endregion

    public void LightAttack()
    {
        Collider2D[] enemyhits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in enemyhits)
        {
            if (enemy.GetComponent<HealthSystem>().isInvincible)
                return;
            else
            {
                enemy.GetComponent<HealthSystem>().TakeHits(damage);
               //enemy.GetComponent<HealthSystem>().SleepAndWakeUp();
                enemy.transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 1f);
                float direction = controller.m_FacingRight == true ? 1f : -1f;
                enemy.GetComponent<Rigidbody2D>().AddForce(new Vector2(force * direction, 0f));
            }
        }
    }

    // draw gismos of attackpoint and groundcheck in the  scene
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        Gizmos.DrawWireSphere(groundCheck.position, radius);
    }
}

//// player's different status  TO be used later !!
//public enum PlayerState {
    
//    LightPunch,
//    LightKick,
//    AirbornePunch,
//    AirborneKick,
//    SpinningKick,
//    UpperCut

//}
//// 