using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hadoken : MonoBehaviour
{
    public float direction = 1;
    private float speed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 position = transform.position;
        position += new Vector2(direction * speed * Time.deltaTime, 0f);
        transform.position = position;

        if (transform.position.x > 10f) {
            Destroy(this.gameObject);
        }
    }
}
