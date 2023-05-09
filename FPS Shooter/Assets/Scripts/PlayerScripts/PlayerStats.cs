using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] public int experience = 0;
    private int skillPoints = 0;
    private int experiencePerKill = 17;
    private int experiencePerSkillPoint = 100;
    

    // Increase the player's experience and check if they've earned a skill point
    public void IncreaseExperience(int amount)
    {
        experience += amount + experiencePerKill;

        if (experience >= experiencePerSkillPoint)
        {
            Debug.Log("You Got a SkillPoint!!");
            skillPoints++;
            experience -= experiencePerSkillPoint;
        }
    }
    public void BossKilled()
    {
        int bossExperience = Random.Range(60, 260);
        experience += bossExperience;
        Debug.Log("Boss Experience gain!!!");
    }
}
