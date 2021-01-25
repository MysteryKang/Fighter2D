using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hadoken : MonoBehaviour
{
    public float direction = 1;
    private float speed = 5f;
    Vector2 scale;
    [SerializeField] private float hadokenDamage = 10f;

    // Start is called before the first frame update
    void Start()
    {
        scale = transform.localScale;
        transform.localScale = new Vector3(scale.x * direction, scale.y);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10) {
            collision.gameObject.GetComponent<HealthSystem>().TakeHits(hadokenDamage);
            collision.gameObject.transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 1f);
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 position = transform.position;
        position += new Vector2(direction * speed * Time.deltaTime, 0f);
        transform.position = position;

        if (Mathf.Abs(transform.position.x) > 20f) {
            Destroy(this.gameObject);
        }
    }
}
