using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform target;
    public float searchingRange;
    public LayerMask targetLayger;

    public float searchTime = 2f;
    public float lastTimeSearched = 0f;

    public float multiplier;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SearchTargetAndAction() {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, searchingRange, targetLayger);
        foreach (Collider2D target in colliders) {
            RunToDestination(target.gameObject.transform.position);
        }
    }

    private void RunToDestination(Vector3 position) {
        Vector3 direction = position - transform.position;
        GetComponent<Rigidbody2D>().AddForce(new Vector3(direction.normalized.x * multiplier, 0f));
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= lastTimeSearched) {
            SearchTargetAndAction();
            lastTimeSearched += searchTime;
        }
    }
}
