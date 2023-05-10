using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinkingFloor : MonoBehaviour
{
    public float sinkingSpeed = 0.5f; // The speed at which the platform sinks
    public float sinkDuration = 3f; // The duration to wait before starting to sink
    public float sinkHeight = 1f; // The amount to sink the platform by

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
            Debug.Log(" here");
            Debug.Log("Player is here");
            // Start the sinking coroutine after the sink duration has passed
            StartCoroutine(SinkAfterDelay());
        }
    }
    IEnumerator SinkAfterDelay()
    {
        yield return new WaitForSeconds(sinkDuration);

        isSinking = true;
        sinkStartTime = Time.time;

        while (transform.position.y > initialYPos - sinkHeight)
        {
            Debug.Log("Drop");
            // Lower the platform's y position over time
            float newYPos = Mathf.Lerp(initialYPos, initialYPos - sinkHeight, (Time.time - sinkStartTime) * sinkingSpeed);
            transform.position = new Vector3(transform.position.x, newYPos, transform.position.z);
            yield return null;
        }
    }
}