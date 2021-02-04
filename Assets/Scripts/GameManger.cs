using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManger : MonoBehaviour
{
    [SerializeField] private float timeScale = 1f;
    public static bool isGameOver;

    // Start is called before the first frame update
    void Start()
    {
        isGameOver = false;
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = timeScale;
      
    }
}
