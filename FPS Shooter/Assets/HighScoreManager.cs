using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HighScoreManager : MonoBehaviour
{
    public TextMeshProUGUI highScore1Text;

    void Start()
    {
        DisplayHighScores();
    }

    void DisplayHighScores()
    {   
        int topHighScores = PlayerPrefs.GetInt("ScoreKey");
        highScore1Text.text = topHighScores.ToString();
    }
}
