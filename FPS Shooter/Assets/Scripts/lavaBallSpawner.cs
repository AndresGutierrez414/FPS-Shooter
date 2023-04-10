using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lavaBallSpawner : MonoBehaviour
{
    // variables //
    [SerializeField] GameObject lavaBallPrefab;
    [SerializeField] float spawnInterval;
    [SerializeField] Vector2 xRange;
    [SerializeField] Vector2 zRange;
    [SerializeField] float yOffset;


    // Start is called before the first frame update
    void Start()
    {
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
            // generate rando position //
            float x = Random.Range(xRange.x, xRange.y);
            float z = Random.Range(zRange.x, zRange.y);
            Vector3 spawnPos = new Vector3(x, yOffset, z);

            // spawn lava ball //
            Instantiate(lavaBallPrefab, spawnPos, Quaternion.identity);

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
