using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    public float health = 1000f;
    const float maxHealth = 1000f;
    public Slider healthBar;
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        healthBar.value = 1.0f;
    }

    public void TakeHits(float damage) {
        health -= damage;
        if (health <= 0) {
            healthBar.value = 0f;
            // play death animation
            this.enabled = false;
        }
        if (health > 0) {
            // play taking hits animation
            animator.SetTrigger("TakeHit");
            healthBar.value = health / maxHealth;
            if (health <= 30) {
                healthBar.fillRect.GetComponent<Image>().color = Color.red;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
