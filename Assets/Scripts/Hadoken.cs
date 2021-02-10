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


    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController2D>();
        scale = transform.localScale;
        transform.localScale = new Vector3(Mathf.Abs(scale.x) * direction * 2f, scale.y * 2f);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10 && collision.gameObject.name != self) {
            collision.gameObject.GetComponent<HealthSystem>().TakeHits(hadokenDamage);
            Transform enemy = collision.gameObject.transform;
            enemy.localScale = new Vector3(Mathf.Abs(enemy.localScale.x) * -direction, enemy.localScale.y, 1f);
            enemy.GetComponent<Rigidbody2D>().AddForce(new Vector2(force * direction, 0f));
            Player.canSpawnHadoken = true;
            Destroy(this.gameObject);
        }
     
        if (collision.gameObject.layer == 11) {
            Destroy(collision.gameObject);
            Destroy(this.gameObject);
        }
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
