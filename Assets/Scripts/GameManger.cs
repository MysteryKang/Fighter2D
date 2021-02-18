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
    private float timeTillShowMenu = 2f;

    public Player player1;
    public Player2 player2;

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
        if (IsGameOver()) {
            Time.timeScale = 0.2f;
            if (timeTillShowMenu > 0)
            {
                timeTillShowMenu -= Time.deltaTime;
            }
            else {
                Time.timeScale = 1f;
                controller.SetActive(false);
                menu.SetActive(true);
            }
        }
    }

    private bool IsGameOver() {
        if (player1.GetComponent<HealthSystem>().health <= 0
            || player2.GetComponent<HealthSystem>().health <= 0)
        {
            return true;
        }
        else
            return false;
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
