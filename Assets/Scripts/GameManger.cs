using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManger : MonoBehaviour
{
    [SerializeField] private float timeScale = 1f;
    public static bool isGameOver;
    public GameObject controller;
    public GameObject menu;

    // Start is called before the first frame update
    void Start()
    {
        isGameOver = false;
        Application.targetFrameRate = 60;
        //DecideWhetherOrNotShowTheController();
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameOver) {
            controller.SetActive(false);
            menu.SetActive(true);
        }
        Time.timeScale = timeScale;
      
    }

    public void StartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

//    private void DecideWhetherOrNotShowTheController() {
//#if UNITY_EDITOR_OSX //UNITY_IOS
//        bool showController = true;
//#else
//        bool showController = false;
//#endif
//        controller.SetActive(showController);
//    }
}
