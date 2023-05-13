using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinkingFloor : MonoBehaviour
{
    
    private float sinkingSpeed = 0.5f; // The speed at which the platform sinks
    private float sinkDuration = 1f; // The duration to wait before starting to sink
    private float sinkHeight = 3f; // The amount to sink the platform by
    private float riseDuration = 1f; // The duration to wait before rising back up
    private float riseSpeed = 0.1f; // The speed at which the platform rise

    private bool isSinking = false;
    private float initialYPos;
    private float sinkStartTime;
    void Start()
    {
        // Store the initial y position of the platform
        initialYPos = transform.position.y;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !isSinking)
        {
            Debug.Log("Player is here");

            // Start the sinking coroutine after the sink duration has passed
            StartCoroutine(SinkAfterDelay());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player is left");
        }
    }

    IEnumerator SinkAfterDelay()
    {
        yield return new WaitForSeconds(sinkDuration);

        isSinking = true;
        sinkStartTime = Time.time;

        while (transform.position.y > initialYPos - sinkHeight)
        {
            // Lower the platform's y position over time
            float newYPos = Mathf.Lerp(initialYPos, initialYPos - sinkHeight, (Time.time - sinkStartTime) * sinkingSpeed);
            transform.position = new Vector3(transform.position.x, newYPos, transform.position.z);
            yield return null;
        }

        yield return new WaitForSeconds(riseDuration);

        float riseStartTime = Time.time;

        while (transform.position.y < initialYPos)
        {
            // Raise the platform's y position over time
            float newYPos = Mathf.Lerp(transform.position.y, initialYPos, (Time.time - riseStartTime) * riseSpeed);
            transform.position = new Vector3(transform.position.x, newYPos, transform.position.z);
            yield return null;
        }

        isSinking = false;
    }
}