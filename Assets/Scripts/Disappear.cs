using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disappear : MonoBehaviour
{
    [SerializeField] private float wait = 0.3f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Remove());
    }

    IEnumerator Remove() {
        yield return new WaitForSeconds(wait);
        Destroy(this.gameObject);
    }
}
