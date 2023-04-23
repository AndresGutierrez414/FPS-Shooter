using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingFloor : MonoBehaviour
{

    public GameObject player;

    public float leftDistance = 5f; // Distance to move left
    public float rightDistance = 5f; // Distance to move right
    public float speed = 5f; // Speed of the platform

    private float currentDistance; // Current distance
    private bool moveLeft = true; // Flag to determine movement direction

    void Update()
    {
        MoveFloor();   
    }
    void MoveFloor()
    {
        // Move the platform horizontally
        if (moveLeft)
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime);
            currentDistance += speed * Time.deltaTime;
        }
        else
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
            currentDistance -= speed * Time.deltaTime;
        }

        // Check for reaching left or right distance
        if (currentDistance <= -leftDistance || currentDistance >= rightDistance)
        {
            // Reverse platform direction when reaching left or right distance
            moveLeft = !moveLeft;

            // Pause at the end of each movement
            StartCoroutine(Pause());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            player.transform.parent = transform; // Making the player a child of the object
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            player.transform.parent = null; // Removing the player as a child 
        }
    }

    IEnumerator Pause()
    {
        yield return new WaitForSeconds(3f);
        // Reset current distance
        currentDistance = Mathf.Clamp(currentDistance, -leftDistance, rightDistance);
    }
}
