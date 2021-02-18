using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallForce : MonoBehaviour
{
    public float direction;
    [SerializeField] private float force;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        collision.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2((force * direction), 0f));
    }
}
