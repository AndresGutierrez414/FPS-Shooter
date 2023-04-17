using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class volcanoLavaBallSpawner : MonoBehaviour
{
    // variables //
    [SerializeField] GameObject largeLavaBallPrefab;
    [SerializeField] int poolSize;
    [SerializeField] float spawnInterval;
    private Vector3 spawnLocation;

    private List<GameObject> largeLavaBallPool;

    // Start is called before the first frame update
    void Start()
    {
        spawnLocation = transform.position;

        largeLavaBallPool = new List<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject largeLavaBall = Instantiate(largeLavaBallPrefab, transform);
            largeLavaBall.SetActive(false);
            largeLavaBallPool.Add(largeLavaBall);
        }

        StartCoroutine(spawnLargeLavaBalls());
    }

    IEnumerator spawnLargeLavaBalls()
    {
        while (true)
        {
            // Find an inactive lava ball
            GameObject largeLavaBall = largeLavaBallPool.Find(obj => !obj.activeInHierarchy);

            if (largeLavaBall != null)
            {
                // Set the lava ball's position and rotation
                largeLavaBall.transform.position = spawnLocation;
                largeLavaBall.transform.rotation = Quaternion.identity;

                // Activate the lava ball
                largeLavaBall.SetActive(true);
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
