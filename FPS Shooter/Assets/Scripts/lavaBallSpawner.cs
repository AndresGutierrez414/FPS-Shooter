using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class lavaBallSpawner : MonoBehaviour
{
    // variables //
    [Header("----------small Lava Ball Stats----------")]
    [SerializeField] GameObject lavaBallPrefab;
    [SerializeField] int poolSize;
    [SerializeField] float spawnInterval;
    [SerializeField] Vector2 xRange;
    [SerializeField] Vector2 zRange;
    [SerializeField] float yOffset;

    private List<GameObject> lavaBallPool;

    // Start is called before the first frame update
    void Start()
    {
        lavaBallPool = new List<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject lavaBall = Instantiate(lavaBallPrefab, transform);
            lavaBall.SetActive(false);
            lavaBallPool.Add(lavaBall);
        }

        StartCoroutine(spawnLavaBalls());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator spawnLavaBalls()
    {
        while (true)
        {
            // Find an inactive lava ball
            GameObject lavaBall = lavaBallPool.Find(obj => !obj.activeInHierarchy);

            if (lavaBall != null)
            {
                // generate rando position //
                float x = Random.Range(xRange.x, xRange.y);
                float z = Random.Range(zRange.x, zRange.y);
                Vector3 spawnPos = new Vector3(x, yOffset, z);

                // Set lava ball pos //
                lavaBall.transform.position = spawnPos;
                lavaBall.transform.rotation = Quaternion.identity;

                lavaBall.SetActive(true);
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
