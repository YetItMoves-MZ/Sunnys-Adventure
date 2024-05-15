using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
public class ScoreScreenUIEngine : MonoBehaviour
{
    [Header("Leaderboards")]
    public GameObject scorePrefab;
    public GameObject leaderboardsParentObject;

    [Header("New Score")]
    public GameObject highScoreNameObject;
    public TMP_Text highScoreName;
    public List<char> availableChars;
    List<GameObject> scores;
    public static bool createNewScore = false;

    // Start is called before the first frame update
    void Start()
    {
        scores = new List<GameObject>();
        if (InGameScoreManagment.newHighScore)
            NewHighScore();
        else
            ShowScores();
    }

    private void NewHighScore()
    {
        InGameScoreManagment.newHighScore = false;
        leaderboardsParentObject.SetActive(false);
        highScoreNameObject.SetActive(true);
    }

    public void OnNewHighScoreConfirmClick()
    {
        string name = "" + GetChar(1) + GetChar(2) + GetChar(3);
        int cherriesCollected = InGameScoreManagment.cherriesCollected;
        int gemsCollected = InGameScoreManagment.gemsCollected;
        int totalScore = (InGameScoreManagment.levelsFinished * 50) + gemsCollected;
        int highScoreCount = InGameScoreManagment.highScoreCount;


        for (int i = 3; i > highScoreCount; i--)
        {
            string currentPlaceName = PlayerPrefs.GetString("Name" + (i - 1));
            int currentPlaceCherriesCollected = PlayerPrefs.GetInt("CherriesCollected" + (i - 1));
            int currentPlaceGemsCollected = PlayerPrefs.GetInt("GemsCollected" + (i - 1));
            int currentPlaceTotalScore = PlayerPrefs.GetInt("TotalScore" + (i - 1));

            PlayerPrefs.SetString("Name" + (i), currentPlaceName);
            PlayerPrefs.SetInt("CherriesCollected" + (i), currentPlaceCherriesCollected);
            PlayerPrefs.SetInt("GemsCollected" + (i), currentPlaceGemsCollected);
            PlayerPrefs.SetInt("TotalScore" + (i), currentPlaceTotalScore);
        }

        PlayerPrefs.SetString("Name" + highScoreCount, name);
        PlayerPrefs.SetInt("CherriesCollected" + highScoreCount, cherriesCollected);
        PlayerPrefs.SetInt("GemsCollected" + highScoreCount, gemsCollected);
        PlayerPrefs.SetInt("TotalScore" + highScoreCount, totalScore);



        int totalPeopleScored = PlayerPrefs.GetInt("PeopleScored", 0);
        if (totalPeopleScored < 3)
            PlayerPrefs.SetInt("PeopleScored", totalPeopleScored + 1);

        highScoreNameObject.SetActive(false);
        leaderboardsParentObject.SetActive(true);
        ShowScores();
    }

    public void OnNameDownClick(int num)
    {
        ChangeName(num, -1);
    }
    public void OnNameUpClick(int num)
    {
        ChangeName(num, 1);
    }

    public void OnClearLeaderboardsClick()
    {
        PlayerPrefs.DeleteKey("PeopleScored");
        ShowScores();
    }

    private void ChangeName(int num, int direction)
    {
        char nextChar = GetChar(num);
        int index = availableChars.IndexOf(nextChar);
        index = direction > 0 ? index + 1 : index - 1;
        if (index >= availableChars.Count)
            nextChar = availableChars[0];
        else if (index < 0)
            nextChar = availableChars[availableChars.Count - 1];
        else
            nextChar = availableChars[index];
        string name = highScoreName.text;
        if (num == 1)
            name = nextChar + "  " + name[3] + "  " + name[6];
        else if (num == 2)
            name = name[0] + "  " + nextChar + "  " + name[6];
        else
            name = name[0] + "  " + name[3] + "  " + nextChar;
        highScoreName.text = name;
    }

    private char GetChar(int num)
    {
        string text = highScoreName.text;
        if (num == 1)
            return text[0];
        else if (num == 2)
            return text[3];
        else
            return text[6];
    }

    private void ShowScores()
    {
        ClearUI();
        int scoreAmount = PlayerPrefs.GetInt("PeopleScored", 0);
        if (scoreAmount == 0)
            return;

        scores = new List<GameObject>();
        for (int i = 1; i <= scoreAmount; i++)
        {
            string name = PlayerPrefs.GetString("Name" + i, "TAV");
            int cherriesCollected = PlayerPrefs.GetInt("CherriesCollected" + i, 0);
            int gemsCollected = PlayerPrefs.GetInt("GemsCollected" + i, 0);
            int totalScore = PlayerPrefs.GetInt("TotalScore" + i, 0);
            scores.Add(CreateNewScoreObject(name, cherriesCollected, gemsCollected, totalScore, i - 1));
        }
    }

    private void ClearUI()
    {
        foreach (GameObject scoreUI in scores)
        {
            Destroy(scoreUI);
        }
        scores.Clear();
    }

    private GameObject CreateNewScoreObject(string name, int cherriesCollected, int gemsCollected, int totalScore, int place)
    {
        GameObject newScore = Instantiate(scorePrefab, leaderboardsParentObject.transform);

        RectTransform transformComponent = newScore.GetComponent<RectTransform>();
        transformComponent.Translate(0f, -100f * place, 0f);

        newScore.transform.GetChild(0).GetComponent<TMP_Text>().text = name;
        newScore.transform.GetChild(1).GetComponent<TMP_Text>().text = cherriesCollected.ToString();
        newScore.transform.GetChild(2).GetComponent<TMP_Text>().text = gemsCollected.ToString();
        newScore.transform.GetChild(3).GetComponent<TMP_Text>().text = totalScore.ToString();

        return newScore;
    }

    public void OnBackClick()
    {
        SceneManager.UnloadSceneAsync(1);
    }
}
