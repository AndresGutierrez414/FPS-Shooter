using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndGoalopen : MonoBehaviour
{
    public enemyAI bossEnemy;     // A reference to the enemyAI component
    public GameObject killBoss;
    private bool isDead;

    // Start is called before the first frame update
    void Start()
    {
        isDead = false;       // Set value of isDead to false
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the boss is dead and the game object has not yet been destroyed
        if (BossIsDead() && !isDead)
        {
            Destroy(gameObject);   // Destroy the game object
            isDead = true;          // Set isDead to true so that the game object is not destroyed again
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!BossIsDead())
        {
            if (other.CompareTag("Player"))
            {

                killBoss.SetActive(true);
            }
        }
        if (BossIsDead())
        {
            if (other.CompareTag("Player"))
            {
                killBoss.SetActive(false);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (!BossIsDead())
        {
            if (other.CompareTag("Player"))
            {

                killBoss.SetActive(false);
            }
        }
        if (BossIsDead())
        {
            if (other.CompareTag("Player"))
            {
                killBoss.SetActive(false);
            }
        }
    }
    // Check if the boss is dead
    public bool BossIsDead()
    {
        return bossEnemy.isBossDestroyed;   // Return the value of the isBossDestroyed flag from the enemyAI component
    }

}
