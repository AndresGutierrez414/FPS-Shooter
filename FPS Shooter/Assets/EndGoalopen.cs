using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGoalopen : MonoBehaviour
{

    enemyAI enemy; 
    bool isDead;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            Destroy(gameObject);
        }
    }

    public bool BossIsDead()
    {
        return enemy.isBossDestroyed;
    }
}
