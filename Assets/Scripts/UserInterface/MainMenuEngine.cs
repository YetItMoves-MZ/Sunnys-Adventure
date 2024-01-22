using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuEngine : MonoBehaviour
{
    void Start()
    {
        if (InGameUIEngine.needToCheckHighScore)
        {
            InGameScoreManagment.CheckNewHighScore();
            if (InGameScoreManagment.newHighScore)
            {
                SceneManager.LoadScene(1, LoadSceneMode.Additive);
            }

        }


    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            OnExitClick();
    }
    public void OnPlayClick()
    {
        InGameScoreManagment.StartNewGame();
        SceneManager.LoadScene(2, LoadSceneMode.Single);
    }
    public void OnExitClick()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
    public void OnLeaderboardsClick()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Additive);
    }
}
