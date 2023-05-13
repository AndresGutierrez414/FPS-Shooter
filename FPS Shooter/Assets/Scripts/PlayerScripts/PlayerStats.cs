using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] public int score = 0;
    private int easyEnemyKill;
    private int medEnemyKill;
    private int hardEnemyKill;
    private int BossKill;

    private HighScore highScore;
    private void Start()
    {
        highScore = FindAnyObjectByType<HighScore>();
    }

    // Increase the player's experience and check if they've earned a skill point
    public void IncreaseEasyKill(int amount)
    {
        score += amount + easyEnemyKill;
        highScore.IncreaseScore(score);
    }
    public void IncreaseMedKill(int amount)
    {
        score += amount + medEnemyKill;
        highScore.IncreaseScore(score);
    }
    public void IncreaseHardKill(int amount)
    {
        score += amount + hardEnemyKill;
        highScore.IncreaseScore(score);
    }
    public void BossKilled()
    {
        score += BossKill;
        highScore.IncreaseScore(score);
        Debug.Log(BossKill + " gain!!!");
    }
}