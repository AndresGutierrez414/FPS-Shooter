using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HighScore : MonoBehaviour
{
    HighScoreManager highScoreManager;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private string ScoreKey = "ScoreKey";

    private int score = 0;
    private int highScore = 0;

    public void IncreaseScore(int amount)
    {
        score += amount;
        if (score > highScore)
        {
            highScore = score;
            highScoreText.text = highScore.ToString();
        }
        UpdateScoreText();
    }
    private void Start()
    {
        highScore = PlayerPrefs.GetInt(ScoreKey, 0);
        highScoreText.text = highScore.ToString();
        UpdateScoreText();
    }
    // Reset the score and save the high score to PlayerPrefs when the game ends
    public void GameOver()
    {
        if (highScoreManager != null)
        {
            highScoreManager.SaveHighScore(highScore);
        }
        PlayerPrefs.SetInt(ScoreKey, highScore);
        score = 0;
        UpdateScoreText();
    }

    // Update the score text
    private void UpdateScoreText()
    {
        scoreText.text = score.ToString();
    }
}