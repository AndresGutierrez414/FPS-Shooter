using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HighScoreManager : MonoBehaviour
{
    public TextMeshProUGUI highScore1Text;
    public TextMeshProUGUI highScore2Text;
    public TextMeshProUGUI highScore3Text;

    int maxHighScores = 3;
    int[] highScores;

    void Start()
    {
        highScores = new int[maxHighScores];
        for (int i = 0; i < maxHighScores; i++)
        {
            highScores[i] = PlayerPrefs.GetInt("ScoreKey" + i, 0);
        }
        DisplayHighScores();
    }

    public void SaveHighScore(int score)
    {
        // Load the high scores from player preferences
        for (int i = 0; i < maxHighScores; i++)
        {
            highScores[i] = PlayerPrefs.GetInt("ScoreKey" + i, 0);
        }

        // Add the new high score to the array
        highScores[maxHighScores - 1] = score;

        // Sort in descending order
        Array.Sort(highScores);
        Array.Reverse(highScores);

        // Save the top 3 high scores to player preferences
        for (int i = 0; i < maxHighScores; i++)
        {
            PlayerPrefs.SetInt("ScoreKey" + i, highScores[i]);
        }
        PlayerPrefs.Save();
    }

    void DisplayHighScores()
    {
        // Load the top 3 high scores from player preferences
        int[] topHighScores = new int[maxHighScores];
        for (int i = 0; i < maxHighScores; i++)
        {
            topHighScores[i] = PlayerPrefs.GetInt("ScoreKey" + i, 0);
        }

        highScore1Text.text = "1. " + topHighScores[0];
        highScore2Text.text = "2. " + topHighScores[1];
        highScore3Text.text = "3. " + topHighScores[2];
    }
}
