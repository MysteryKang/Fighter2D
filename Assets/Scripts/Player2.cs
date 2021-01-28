using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player2 : MonoBehaviour
{
    //public float health;
    //public TextMeshProUGUI healthText;
    GameObject sign = new GameObject("Player2");
    // Start is called before the first frame update
    void Start()
    {
        sign.transform.rotation = Camera.main.transform.rotation; // Causes the text faces camera.
        TextMesh tm = sign.AddComponent<TextMesh>();
        tm.text = "put your text here. You can use some of the html attributes";
        tm.color = new Color(0.8f, 0.8f, 0.8f);
        tm.fontStyle = FontStyle.Bold;
        tm.alignment = TextAlignment.Center;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.characterSize = 0.065f;
        tm.fontSize = 60;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Hadoken") {
         //   health--;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Hadoken")
        {
         //   health--;
        }
    }


    // Update is called once per frame
    void Update()
    {
        // healthText.text = "Health: " + health;
        sign.transform.position = transform.position + Vector3.up * 3f;
    }
}
