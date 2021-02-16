using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player2 : MonoBehaviour
{

    [Header("InputButtons")]
    public Button leftButton;
    public Button rightButton;
    public Button buttonA;
    public Button buttonB;
    public Button buttonJump;
    public Button buttonSp;


    public PlayerState currentState = PlayerState.Idle;
    public bool isRightDirectionButtonClicked = false;
    public bool isLeftDirectionButtonClicked = false;

    public CharacterController2D controller;
    // animator
    public Animator animator;
    FightingCombo combo;

    public TextMeshProUGUI comboText;

    //  the player's state
    //public PlayerState currentState;

    //------------------------
    public float lastTimeBeingAttacted;

    //-- attacking cast
    [Header("AttackingCast")]
    public Transform attackPoint;
    public Transform attackPoint_Back;  // for the spinning kick purpose
    public float attackRange;
    public LayerMask enemyLayers;
    public Transform hadokenPoint;

    public bool isBeingAttacked = false;

    public int comboHit = 1;

    [Header("Grouncheck")]
    public Transform groundCheck;
    public float radius;

    public float speed;

    [SerializeField] private float animationSpeed = 1f;

    public float horizontal;
    bool jump = false;
    public bool attacking = false;

    public float hadokenRate = 0.3f;

    public float damage = 1f;

    [SerializeField] private Vector2 upperCutForce;

    bool isUpperCutting = false;

    public static bool canSpawnHadoken = true;

    public float spinningKickHorizontalSpeed;

    public bool isMoveable = false;

    // inputs
    public bool isJumping = false;
    public bool punching = false;
    public bool kicking = false;
    public bool isSpecialKeyPressed = false;


    [SerializeField] private float force;
    // Start is called before the first frame update
    void Start()
    {
        combo = GetComponent<FightingCombo>();
        SetUpButtons();
        
        
    }

    // --- spawning hadoken here and assign <Hadoken> class to the gameobject
    public void SpawnHadoken()
    {
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
        hdk.GetComponent<SpriteRenderer>().sortingLayerName = "Hadoken";
        canSpawnHadoken = false;
    }

   

    #region setting up buttons here
    public void SetUpButtons() {
        //buttonA.onClick.AddListener(LightPunch);
        //buttonB.onClick.AddListener(Kick);
        //buttonJump.onClick.AddListener(Jumping);
        //buttonSp.onClick.AddListener(Hadoken);
    }
    #endregion
    public void Jumping() {
        if (controller.m_Grounded) {
            jump = true;
            animator.SetTrigger("Jump");
        }
    }

    

    // Update is called once per frame
    void Update()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("TakeHit"))
        {
            isBeingAttacked = true;
        }


        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("TakeHit"))
        {
            if (isRightDirectionButtonClicked && controller.m_Grounded)
            {
                horizontal = 1f;
            }
            if (isLeftDirectionButtonClicked && controller.m_Grounded)
            {
                horizontal = -1f;
            }
        }
        else {
            return;
        }

       
        if (Mathf.Abs(horizontal) > 0)
        {
            animator.SetBool("isMoving", true);
        }
        if (horizontal == 0)
        {
            animator.SetBool("isMoving", false);
        }

        animator.SetFloat("xVelocity", GetComponent<Rigidbody2D>().velocity.x);


        animator.speed = animationSpeed;
       

        //------- determine whether it is moving horizontally or not
        if (Mathf.Abs(horizontal) > 0)
        {
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
        // ---------


        if (combo.curAttack != null)
        {
            animator.ResetTrigger("LightPunch");
            animator.ResetTrigger("Kick");
            animator.ResetTrigger("Hadoken");
        }

        if (controller.m_Grounded) {
            isUpperCutting = false;
            animator.ResetTrigger("AirbornePunch");
            animator.ResetTrigger("AirborneKick");
        }
      

        //// if any of {LightPunch, Hadoken, Kick}'s  animation is Playing stop moving horizontally
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("LightPunch") || animator.GetCurrentAnimatorStateInfo(0).IsName("Hadoken")
            || animator.GetCurrentAnimatorStateInfo(0).IsName("Kick"))
        {
            horizontal = 0f;
            attacking = true;
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Ryu") || animator.GetCurrentAnimatorStateInfo(0).IsName("Ryu_Running"))
        {
            attacking = false;
        }


        animator.SetBool("isGrounded", controller.m_Grounded);
        animator.SetFloat("yVelocity", GetComponent<Rigidbody2D>().velocity.y);
        animator.SetFloat("xVelocity", GetComponent<Rigidbody2D>().velocity.x);

    }

    // when enemy gets attacked, it'll be pushed backward 
    private void PushBackward(Rigidbody2D rd)
    {
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
    public void Hadoken()
    {
        if (canSpawnHadoken && controller.m_Grounded)
        {
            attacking = true;
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Hadoken"))
            {
                return;
            }
            else
            {
                // animator.SetTrigger("Hadoken");
                animator.Play("Hadoken", -1, 0);
            }
        }
        else {
            return;
        }
    }

    #endregion

    #region  Light Punch
    public void LightPunch()
    {
        if (controller.m_Grounded)
        {
            animator.SetTrigger("LightPunch");
        }
        else {
            if (!isUpperCutting)
                AirbornePunch();
            else return;
        } 
    }
    #endregion

    #region Kick 
    public void Kick()
    {
        if (controller.m_Grounded)
        {
            animator.SetTrigger("Kick");
        }
        else
        {
            if (!isUpperCutting)
                AirborneKick();
            else return;
        }
    }
    #endregion

    // used in simple attack
    public void DetectEnemyAndGiveDamage()
    {
        Collider2D enem = Physics2D.OverlapCircle(attackPoint.position, attackRange, enemyLayers);
        if (enem != null)
        {
            Transform enemy = enem.transform.parent;
            if (enemy.GetComponent<HealthSystem>() == null)
            {
                Debug.Log("this enemy does not contain healthsystem");
            }
            else {
                if (enemy.GetComponent<HealthSystem>().isInvincible)
                    return;
                else
                {
                    enemy.GetComponent<HealthSystem>().TakeHits(damage);
                    PushBackward(enemy.GetComponent<Rigidbody2D>());
                    float direction = controller.m_FacingRight == true ? 1f : -1f;
                    enemy.transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * -direction, enemy.transform.localScale.y, 1f);
                    enemy.GetComponent<CharacterController2D>().m_FacingRight = controller.m_FacingRight == true ? false : true;
                    enemy.GetComponent<Rigidbody2D>().AddForce(new Vector2(force * direction, 0f));
                    this.GetComponent<Rigidbody2D>().AddForce(new Vector2(force * -direction, 0f));
                }
            }
        }
        else return;
    }

    public void ComboTheEnemy()
    {
        Collider2D enemy = Physics2D.OverlapCircle(attackPoint.position, attackRange, enemyLayers);
        if (enemy != null)
        {
            if (enemy.transform.parent == null)
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
            else {
                Transform en = enemy.transform.parent;
                if (en.GetComponent<HealthSystem>().isInvincible)
                    return;
                else
                {
                    en.GetComponent<HealthSystem>().TakeHits(damage);
                    PushBackward(en.GetComponent<Rigidbody2D>());
                    float direction = controller.m_FacingRight == true ? 1f : -1f;
                    en.transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * -direction, en.transform.localScale.y, 1f);
                    //enemy.GetComponent<Rigidbody2D>().AddForce(new Vector2(force * direction, 0f));
                    //this.GetComponent<Rigidbody2D>().AddForce(new Vector2(force * -direction, 0f));
                }


            }
        }
        else return;
    }

   
    #region   UpperCut

    private void UpperCut()
    {
        attacking = true;
        horizontal = controller.m_FacingRight ? 2f : -2f;
        // animator.SetTrigger("UpperCut");
        animator.Play("UpperCut", -1, 0);
    }

    // ------- trigger at animation event
    public void UpperCutJump()
    {
        isUpperCutting = true;
        controller.m_AirControl = false;
        horizontal = controller.m_FacingRight ? 2f : -2f;
        Vector2 force = upperCutForce;
        if (controller.m_FacingRight)
        {
            force.x *= 1f;
        }
        else
        {
            force.x *= -1f;
        }
        jump = true;
        horizontal *= 0.3f;
    }
    #endregion

    #region Spinning Kick
    public void SpinningKick()
    {
        attacking = true;
        animator.SetTrigger("SpinningKick");
        GetComponent<Rigidbody2D>().gravityScale = 0f;
        transform.position += new Vector3(0f, 0.6f);
        StartCoroutine(SpinningKickMovement());
    }

    IEnumerator SpinningKickMovement()
    {
        float direction = transform.localScale.x / Mathf.Abs(transform.localScale.x);
        spinningKickHorizontalSpeed *= direction;
        horizontal = 1f * direction * 0.5f;
        yield return new WaitForSeconds(1f);
        GetComponent<Rigidbody2D>().gravityScale = 5f;
    }

    //  spinning kick push the enemy both ways   * codes will be simplified later
    public void SpinningKickToEnemy()
    {
        float direction = controller.m_FacingRight ? 1f : -1f;
        DetectAndAttack(attackPoint.position, direction); // front of the player
        DetectAndAttack(attackPoint_Back.position, -direction); // back of the player
    }

    // supporting private function for the spinning kick 
    private void DetectAndAttack(Vector2 position, float direction)
    {
        Collider2D[] enemyhits2 = Physics2D.OverlapCircleAll(position, attackRange, enemyLayers);
        foreach (Collider2D enemy in enemyhits2)
        {
            if (enemy.transform.parent != null)
            {
                Transform en = enemy.transform.parent;
                if (en.GetComponent<HealthSystem>().isInvincible)
                    return;
                else
                {
                    en.GetComponent<HealthSystem>().TakeHits(damage);
                    en.GetComponent<Animator>().SetTrigger("TakenDown");
                    en.GetComponent<HealthSystem>().SleepAndWakeUp();
                    en.transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 1f);
                    en.GetComponent<Rigidbody2D>().AddForce(new Vector2(upperCutForce.x * direction * 5f, upperCutForce.y / 2f));
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
            if (enemy.transform.parent != null)
            {
                Transform en = enemy.transform.parent;
                if (en.GetComponent<HealthSystem>().isInvincible)
                    return;
                else
                {
                    en.GetComponent<HealthSystem>().TakeHits(damage);
                    en.GetComponent<Animator>().SetTrigger("TakenDown");
                    en.GetComponent<HealthSystem>().SleepAndWakeUp();
                    en.transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 1f);
                    float direction = controller.m_FacingRight == true ? 1f : -1f;
                    en.GetComponent<Rigidbody2D>().AddForce(new Vector2(upperCutForce.x * direction, upperCutForce.y));
                }
            }

            
        }
    }

    #region Airborne Punch and Airborne Kick 
    private void AirbornePunch()
    {
        // attacking = true;
        //   LightAttack();
        animator.SetTrigger("AirbornePunch");
    }
    private void AirborneKick()
    {
        //  attacking = true;
        //    LightAttack();
        animator.SetTrigger("AirborneKick");
    }
    #endregion

    public void LightAttack()
    {
        Collider2D[] enemyhits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D en in enemyhits)
        {
            if (en.transform.parent != null) {
                Transform enemy = en.transform.parent;
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
    }

    // deterimine if the player is being attacked by a combo 
    public bool IsBeingComboed()
    {
        return false;
    }   

}


