using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public CharacterController2D controller;
    // animator
    public Animator animator;
    FightingCombo combo;

    //  the player's state
    public PlayerState currentState;

    //------------------------
    public float lastTimeBeingAttacted;

    //-- attacking cast
    [Header("AttackingCast")]
    public Transform attackPoint;
    public Transform attackPoint_Back;  // for the spinning kick purpose
    public float attackRange;
    public LayerMask enemyLayers;
    public Transform hadokenPoint;

    //-------- if successfully hit enemy before it wakes up , count++
    //private bool successfullHit = false;
    // int hitCount = 0;

    public bool isBeingAttacked = false;


    [Header("Grouncheck")]
    public Transform groundCheck;
    public float radius;

    public float speed;

    [SerializeField] private float animationSpeed = 1f;

    [SerializeField] private float horizontal;
    bool jump = false;
    public bool attacking = false;

    public float hadokenRate = 0.3f;

    public float damage = 1f;

    [SerializeField] private Vector2 upperCutForce;

    bool isUpperCutting = false;

    public static bool canSpawnHadoken = true;

    public float spinningKickHorizontalSpeed;

    private bool shouldGetup = true;

    private bool isAlive = true;

    [SerializeField] private float force;
    // Start is called before the first frame update
    void Start()
    {
        combo = GetComponent<FightingCombo>();
    }

    // --- spawning hadoken here 
    public void SpawnHadoken() {
        GameObject hdk = new GameObject();
        hdk.transform.position = hadokenPoint.position;
        hdk.AddComponent<SpriteRenderer>();
        hdk.GetComponent<SpriteRenderer>().sprite = Resources.Load("Hadoken", typeof(Sprite)) as Sprite;
        hdk.AddComponent<PolygonCollider2D>();
        hdk.GetComponent<PolygonCollider2D>().isTrigger = true;
        hdk.AddComponent<Hadoken>();
        hdk.GetComponent<Hadoken>().self = gameObject.name;
        hdk.GetComponent<Hadoken>().direction = transform.localScale.x / Mathf.Abs(transform.localScale.x);
        hdk.gameObject.name = "hadoken";
        hdk.gameObject.layer = 11;
        hdk.GetComponent<SpriteRenderer>().sortingLayerName = "Hadoken"; 
        canSpawnHadoken = false;
    }


    //-----------------------------------------------------------------------------------------
    // Update is called once per frame
    void Update()
    {
        animator.speed = animationSpeed;

        // check if the player/myself is alive
        if (GetComponent<HealthSystem>().health <= 0)
            isAlive = false;
        else
            isAlive = true;

        //-------------------------

        // this needs to be optimized
        if ((!attacking && currentState != PlayerState.IsBeingAttacked) || combo.curAttack != null)
        {
            if (isAlive)
                horizontal = Input.GetAxisRaw("Horizontal") * speed;

            if (horizontal == 0)
            {
                animator.SetBool("isMoving", false);
            }
            else {
                animator.SetBool("isMoving", true);
            }
        }
        if(attacking){
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("SpinningKick") || isBeingAttacked)
            {
                return;
            }
            else{
                horizontal = 0f;
            }
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("TakenDown") && !controller.m_Grounded)
        {
            horizontal = controller.m_FacingRight ? -0.5f : 0.5f;
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("TakenDown") && controller.m_Grounded)
        {
            horizontal = 0f;
        }

        if (isAlive && Input.GetKeyDown(KeyCode.Space) && controller.m_Grounded && !attacking && !isBeingAttacked)
        {
            jump = true;
            animator.SetTrigger("Jump");
        }
  
        animator.SetBool("isGrounded", controller.m_Grounded);

        // ------ airborne punch or airborne kick
        if (!controller.m_Grounded ||
            !animator.GetCurrentAnimatorStateInfo(0).IsName("UpperCut")) {
            if (Input.GetKeyDown(KeyCode.J))
            {
                AirBornePunch();
            }
            else if (Input.GetKeyDown(KeyCode.K)) {
                AirborneKick();
            }
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            animator.Play("Combo2", -1, 0);
        }

        // if the player is not getting attacked, start accepting inputs
        if (!isBeingAttacked)
        {
            if (Input.GetKeyDown(KeyCode.J) && controller.m_Grounded)
            {
                LightPunch();
            }
            if (Input.GetKeyDown(KeyCode.K) && controller.m_Grounded)
            {
                Kick();
            }

            if (Input.GetKeyDown(KeyCode.I) && controller.m_Grounded && canSpawnHadoken)
            {
                Hadoken();
            }
 

            if (Input.GetKeyDown(KeyCode.U) && controller.m_Grounded && !animator.GetCurrentAnimatorStateInfo(0).IsName("UpperCut"))
            {
                UpperCut();
            }

            if (Input.GetKeyDown(KeyCode.H) && !animator.GetCurrentAnimatorStateInfo(0).IsName("SpinningKick") && controller.m_Grounded)
            {
                SpinningKick();
            }
        }

        // if any of {LightPunch, Hadoken, Kick}'s  animation is Playing stop moving horizontally
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("LightPunch") || animator.GetCurrentAnimatorStateInfo(0).IsName("Hadoken")
            || animator.GetCurrentAnimatorStateInfo(0).IsName("Kick"))
        {
            horizontal = 0f;
            attacking = true;
        }   
    
        // UpperCutting
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("UpperCut")) {
            horizontal *= 0.3f;
        }

        animator.SetBool("isGrounded", controller.m_Grounded);
        animator.SetFloat("xVelocity", GetComponent<Rigidbody2D>().velocity.x);

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("TakeHit"))
        {
            currentState = PlayerState.IsBeingAttacked;
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("TakenDown"))
        {
            currentState = PlayerState.TakenDown;
        }
        if (attacking) {
            currentState = PlayerState.Attacking;
        }
    }

    // when enemy gets attacked, it'll be pushed backward 
    private void PushBackward(Rigidbody2D rd) {
        float direction = controller.m_FacingRight == true ? 1f : -1f;  
        rd.AddForce(new Vector2(10f * direction, 0f));
    }
    // ------------------------------------------

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
            animator.Play("Hadoken", -1, 0);
        }
    }

    #endregion

    #region  Light Punch
    public void LightPunch()
    {
        if (combo.curAttack == null)
        {
            attacking = true;
            horizontal = 0f;
            animator.SetTrigger("LightPunch");
        }
    }
    #endregion

    #region Kick 
    public void Kick()
    {
        if (combo.curAttack == null)
        {
            horizontal = 0f;
            attacking = true;
            animator.SetTrigger("Kick");
        }
    }
    #endregion

    // used in simple attack
    public void DetectEnemyAndGiveDamage()
    {
        Collider2D en = Physics2D.OverlapCircle(attackPoint.position, attackRange, enemyLayers);
        if (en != null)
        {
            Transform enemy = en.transform.parent;
            if (enemy.GetComponent<HealthSystem>() == null)
            {
                Debug.Log("this enemy does not contain healthsystem");
            }
            else
            {
                if (enemy.GetComponent<HealthSystem>().isInvincible)
                    return;
                else
                {
                    enemy.GetComponent<HealthSystem>().TakeHits(damage);
                    enemy.GetComponent<Animator>().Play("TakeHit2");
                    PushBackward(enemy.GetComponent<Rigidbody2D>());
                    float direction = controller.m_FacingRight == true ? 1f : -1f;
                    //enemy.transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * -direction, enemy.transform.localScale.y, 1f);
                    if (enemy.GetComponent<CharacterController2D>().m_FacingRight == controller.m_FacingRight) {
                        enemy.GetComponent<CharacterController2D>().Flip();
                    } 
                    enemy.GetComponent<Rigidbody2D>().AddForce(new Vector2(force * direction, 0f));
                    this.GetComponent<Rigidbody2D>().AddForce(new Vector2(force * -direction, 0f));
                }
            }
        }
        else return;
    }

    public void ComboTheEnemy() {
        Collider2D enemy = Physics2D.OverlapCircle(attackPoint.position, attackRange, enemyLayers);
        if (enemy != null)
        {
            if (enemy.GetComponent<HealthSystem>().isInvincible)
                return;
            else
            {
                enemy.GetComponent<HealthSystem>().TakeHits(damage);
                
                PushBackward(enemy.GetComponent<Rigidbody2D>());
                float direction = controller.m_FacingRight == true ? 1f : -1f;
                enemy.transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * -direction, enemy.transform.localScale.y, 1f);
                //enemy.GetComponent<Rigidbody2D>().AddForce(new Vector2(force * direction, 0f));
                //this.GetComponent<Rigidbody2D>().AddForce(new Vector2(force * -direction, 0f));
            }
        }
        else return;
    }

    #region   UpperCut

    private void UpperCut() {
        attacking = true;
        horizontal = controller.m_FacingRight ? 2f : -2f;
        // animator.SetTrigger("UpperCut");
        animator.Play("UpperCut", -1, 0);
    }

    // ------- trigger at animation event
    public void UpperCutJump() {
        isUpperCutting = false;
        controller.m_AirControl = false;
        horizontal = controller.m_FacingRight ? 2f : -2f;
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
        float direction = transform.localScale.x / Mathf.Abs(transform.localScale.x);
        spinningKickHorizontalSpeed *= direction;
        horizontal = 1f * direction * 0.5f;
        yield return new WaitForSeconds(1f);
        GetComponent<Rigidbody2D>().gravityScale = 5f;
    }

  //  spinning kick push the enemy both ways   * codes will be simplified later
    public void SpinningKickToEnemy() {
        float direction = controller.m_FacingRight ? 1f : -1f;
        DetectAndAttack(attackPoint.position, direction); // front of the player
        DetectAndAttack(attackPoint_Back.position, -direction); // back of the player
    }

    // supporting private function for the spinning kick 
    private void DetectAndAttack(Vector2 position, float direction) {
        Collider2D[] enemyhits2 = Physics2D.OverlapCircleAll(position, attackRange, enemyLayers);
        foreach (Collider2D enemy in enemyhits2)
        {
            if (enemy.GetComponent<HealthSystem>() != null)
            {
                if (enemy.GetComponent<HealthSystem>().isInvincible)
                    return;
                else
                {
                    enemy.GetComponent<HealthSystem>().TakeHits(damage);
                    enemy.GetComponent<Animator>().SetTrigger("TakenDown");
                    enemy.GetComponent<HealthSystem>().SleepAndWakeUp();
                    if (enemy.GetComponent<CharacterController2D>().m_FacingRight == controller.m_FacingRight)
                    {
                        enemy.GetComponent<CharacterController2D>().Flip();
                    }
                    enemy.GetComponent<Rigidbody2D>().AddForce(new Vector2(upperCutForce.x * direction * 7f, upperCutForce.y / 2f));
                }
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
            if (enemy.GetComponent<HealthSystem>() == null)
            {
                Debug.Log("enemy does not contain healthsystem");
            }
            else {
                if (enemy.GetComponent<HealthSystem>().isInvincible)
                    return;
                else
                {
                    enemy.GetComponent<HealthSystem>().TakeHits(damage);
                    enemy.GetComponent<Animator>().SetTrigger("TakenDown");
                    enemy.GetComponent<HealthSystem>().SleepAndWakeUp();
                    if (enemy.GetComponent<CharacterController2D>().m_FacingRight == controller.m_FacingRight)
                    {
                        enemy.GetComponent<CharacterController2D>().Flip();
                    }
                    float direction = controller.m_FacingRight == true ? 1f : -1f;
                    enemy.GetComponent<Rigidbody2D>().AddForce(new Vector2(upperCutForce.x * direction, upperCutForce.y));
                }
            }
        }
    }

    #region Airborne Punch and Airborne Kick 
    private void AirBornePunch() {
       // attacking = true;
     //   LightAttack();
        animator.SetTrigger("AirbornePunch");
    }
    private void AirborneKick() {
      //  attacking = true;
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
                if (enemy.GetComponent<CharacterController2D>().m_FacingRight == controller.m_FacingRight)
                {
                    enemy.GetComponent<CharacterController2D>().Flip();
                }
                float direction = controller.m_FacingRight == true ? 1f : -1f;
                enemy.GetComponent<Rigidbody2D>().AddForce(new Vector2(force * direction, 0f));
            }
        }
    }

    // deterimine if the player is being attacked by a combo 
    public bool IsBeingComboed() {
        return false;
    }


    //--------------- should I use physics2d.castbox as called raycast to detect hit collision ?? to be determined.
    //private void HitBox() {
    //    RaycastHit hit = Physics2D.BoxCast(position, size, angle, direction);

    //}



    // draw gismos of attackpoint and groundcheck in the  scene
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        Gizmos.DrawWireSphere(groundCheck.position, radius);

    }
}


//// player's different status  TO be used later !!
public enum PlayerState
{
    Idle = 0,
    Attacking = 1,
    IsBeingAttacked = 2,
    TakenDown = 3,
}
//// 