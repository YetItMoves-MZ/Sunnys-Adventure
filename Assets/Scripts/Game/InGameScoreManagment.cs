using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Playables;

public class InGameScoreManagment
{
    public static int gemsCollected;
    public static int cherriesCollected;
    public static int levelsFinished;
    public static bool newHighScore;
    public static int highScoreCount;

    public static void StartNewGame()
    {
        gemsCollected = 0;
        cherriesCollected = 0;
        levelsFinished = 0;
        newHighScore = false;
        highScoreCount = 0;
    }

    public static void CheckNewHighScore()
    {
        int totalScore = (levelsFinished * 50) + gemsCollected;
        int scoreAmount = PlayerPrefs.GetInt("PeopleScored", 0);
        if (scoreAmount == 0)
        {
            newHighScore = true;
            highScoreCount = 1;
            return;
        }
        for (int i = 1; i <= scoreAmount; i++)
        {
            if (totalScore > PlayerPrefs.GetInt("TotalScore" + i, 0))
            {
                newHighScore = true;
                highScoreCount = i;
                return;
            }
        }
        if (scoreAmount < 3)
        {
            highScoreCount = scoreAmount + 1;
            newHighScore = true;
        }
    }
}
