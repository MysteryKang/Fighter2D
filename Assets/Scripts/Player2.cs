using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player2 : MonoBehaviour
{
    public float health;
    public TextMeshProUGUI healthText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Hadoken") {
            health--;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Hadoken")
        {
            health--;
        }
    }


    // Update is called once per frame
    void Update()
    {
        healthText.text = "Health: " + health;
    }
}
