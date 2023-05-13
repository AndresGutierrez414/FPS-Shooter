using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingFloor : MonoBehaviour
{
    [Header("-----Functions-----")]
    [SerializeField] public float distance = 2.0f; // Distance of movement
    [SerializeField] public float speed = 2.0f; // Speed of movement
    [SerializeField] public float delay = 0.0f; // Delay before movement starts
    [SerializeField] public bool moveUp = true; // Determines if object should move up or down
    [SerializeField] private bool moving;     // bool for DelayMovement()

    private Vector3 startPosition;
    private Vector3 endPosition;
    private float startTime;
    private float duration;

    // Start is called before the first frame update
    void Start()
    {
        moving = true;
        startPosition = transform.position;
        endPosition = new Vector3(startPosition.x, startPosition.y + distance, startPosition.z);
        duration = distance / speed;
        startTime = Time.time + delay;
    }

    // Update is called once per frame
    void Update()
    {
        MovingFloor();
    }

    void MovingFloor()
    {
        if (!moving)
        {
            return;
        }

        float t = (Time.time - startTime) / duration;
        if (t > 1.0f)
        {
            // Switch direction and reset timer
            moveUp = !moveUp;
            startTime = Time.time;
            startPosition = transform.position;
            endPosition = new Vector3(startPosition.x, moveUp ? startPosition.y + distance : startPosition.y - distance, startPosition.z);

            // Set delay before next movement
            StartCoroutine(DelayMovement());
        }
        else if (t > 0.9f && t < 1.0f) // Stop at the top and bottom for 0.5 seconds
        {
            moving = false;
            StartCoroutine(DelayMovement());
        }
        else
        {
            // Interpolate position
            Vector3 newPos = Vector3.Lerp(startPosition, endPosition, t);
            transform.position = newPos;
        }
    }

    IEnumerator DelayMovement()
    {
        // Wait for delay
        yield return new WaitForSeconds(delay);

        // Resume movement
        moving = true;
    }
}