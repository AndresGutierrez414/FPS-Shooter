using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nestedEnemy : MonoBehaviour
{
    [Header("----- NestedEnemys -----")]
    [SerializeField] private GameObject nestedEnemysPrefab;
    [SerializeField] private int numNestedEnemysPrefab;
    [SerializeField] private enemyAI bossEnemy;

    // Start is called before the first frame update
    void Start()
    {
        bossEnemy = FindAnyObjectByType<enemyAI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (bossEnemy && bossEnemy.isBossDestroyed)
        {
            for (int i = 0; i < numNestedEnemysPrefab; i++)
            {
                Vector3 spawnPos = transform.position + new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
                Instantiate(nestedEnemysPrefab, spawnPos, Quaternion.identity);
            }
        }
    }
    public  bool BossIsDead()
    {
        return bossEnemy.isBossDestroyed;
    }
}
