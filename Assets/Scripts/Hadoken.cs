using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hadoken : MonoBehaviour
{
    public float direction = 1f;
    private float speed = 10f;
    Vector2 scale;
    [SerializeField] private float hadokenDamage = 1f;
    CharacterController2D controller;
    private float force = 100f;

    public string self;
    public bool canSpawnHadoken = true;

    public GameObject hitEffect;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController2D>();
        scale = transform.localScale;
        transform.localScale = new Vector3(Mathf.Abs(scale.x) * direction * 2f, scale.y * 2f);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10 && collision.gameObject.name != self)
        {
            collision.gameObject.GetComponent<HealthSystem>().TakeHits(hadokenDamage);
            ShowHitEffect(collision.transform);
            collision.gameObject.GetComponent<Animator>().Play("TakeHit2", -1, 0);
            AudioManager.PlayHitSound(); //  hit sound
            Transform enemy = collision.gameObject.transform;
            enemy.localScale = new Vector3(Mathf.Abs(enemy.localScale.x) * -direction, enemy.localScale.y, 1f);
            enemy.GetComponent<Rigidbody2D>().AddForce(new Vector2(force * direction, 0f));
            Player.canSpawnHadoken = true;
            Destroy(this.gameObject);
        }

        if (collision.transform.parent.name == "Player" && collision.gameObject.name != self) {
            Transform parent = collision.transform.parent;
            ShowHitEffect(parent);
            parent.gameObject.GetComponent<HealthSystem>().TakeHits(hadokenDamage);
            AudioManager.PlayHitSound(); // hit sound
            parent.gameObject.GetComponent<Animator>().Play("TakeHit2", -1, 0);
            parent.localScale = new Vector3(Mathf.Abs(parent.localScale.x) * -direction, parent.localScale.y, 1f);
            parent.GetComponent<Rigidbody2D>().AddForce(new Vector2(force * direction, 0f));
            Player2.canSpawnHadoken = true;
            Destroy(this.gameObject);
        }
    }

    private void ShowHitEffect(Transform tras)
    {
        GameObject effect = Instantiate(hitEffect);
        effect.transform.position = tras.position;
        effect.gameObject.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        Vector2 position = transform.position;
        position += new Vector2(direction * speed * Time.deltaTime, 0f);
        transform.position = position;

        if (Mathf.Abs(transform.position.x) > 10f) {
            if (self == "Player2")
            {
                Player2.canSpawnHadoken = true;
            }
            else {
                Player.canSpawnHadoken = true;
            }
            Destroy(this.gameObject);
        }
    }
}
