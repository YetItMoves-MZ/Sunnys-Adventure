using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public GameObject levelClearMessage;
    public float windupTime;
    float currentTime;
    public static bool levelEnded;



    CanvasGroup canvasGroup;

    // Start is called before the first frame update
    void Start()
    {
        levelEnded = false;
        currentTime = 0f;
        canvasGroup = levelClearMessage.GetComponent<CanvasGroup>();

    }

    // Update is called once per frame
    void Update()
    {
        if (!levelEnded)
            return;

        levelClearMessage.SetActive(true);
        if (windupTime < currentTime)
        {
            // if i had more levels then i would have moved to the next one here.
            // maybe add a static int that saves what level i am at.
            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }
        else
        {
            float percent = currentTime / (windupTime - 1f);
            percent = percent > 1f ? 1f : percent;
            canvasGroup.alpha = percent;
            currentTime += Time.deltaTime;
        }
    }
}
