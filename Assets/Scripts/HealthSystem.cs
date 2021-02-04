using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class HealthSystem : MonoBehaviour
{
    public float health = 1000f;
    const float maxHealth = 1000f;
    public Slider healthBar;
    public Animator animator;

    public bool isInvincible = false;
    
    // Start is called before the first frame update
    void Start()
    {
        healthBar.value = 1.0f;
    }

    public void ResetAirControl() {
        if (GetComponent<CharacterController2D>() != null) {
            GetComponent<CharacterController2D>().m_AirControl = true;
        }
        if (GetComponent<Player>() != null) {
            GetComponent<Player>().attacking = false;
            GetComponent<Player>().currentState = PlayerState.Idle;
        }
        
    }

    public void TakeHits(float damage) {
        if (isInvincible)
            return;
        else {
            health -= damage;
            if (health <= 0)
            {
                healthBar.value = 0f;
                // play death animation
                this.enabled = false;
            }
            if (health > 0)
            {
                // play taking hits animation
                animator.SetTrigger("TakeHit");
                healthBar.value = health / maxHealth;
                if (health <= 30)
                {
                    healthBar.fillRect.GetComponent<Image>().color = Color.red;
                }
            }
        }
    }

    public void SleepAndWakeUp() {
        StartCoroutine(TakenDownAndGetUp());
    }

    IEnumerator TakenDownAndGetUp() {
        isInvincible = true;
        yield return new WaitForSeconds(2.0f);
        isInvincible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(health <= 0f)
        {
            GameManger.isGameOver = true;
        }
    }
}
