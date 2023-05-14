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
        DisplayHighScores();
    }

    public void SaveHighScore(int score)
    {
        // Load the existing high scores from player preferences
        for (int i = 0; i < maxHighScores; i++)
        {
            highScores[i] = PlayerPrefs.GetInt("HighScore" + i, 0);
        }

        // Add the new high score to the array
        highScores[maxHighScores - 1] = score;

        // Sort the array in descending order
        Array.Sort(highScores);
        Array.Reverse(highScores);

        // Save the top 3 high scores to player preferences
        for (int i = 0; i < maxHighScores; i++)
        {
            PlayerPrefs.SetInt("HighScore" + i, highScores[i]);
        }

        // Save the changes
        PlayerPrefs.Save();
    }

    void DisplayHighScores()
    {
        // Load the top 3 high scores from player preferences
        int[] topHighScores = new int[maxHighScores];
        for (int i = 0; i < maxHighScores; i++)
        {
            topHighScores[i] = PlayerPrefs.GetInt("HighScore" + i, 0);
        }

        // Set the text of each TextMeshPro component to the corresponding high score
        highScore1Text.text = "1. " + topHighScores[0];
        highScore2Text.text = "2. " + topHighScores[1];
        highScore3Text.text = "3. " + topHighScores[2];
    }
}
