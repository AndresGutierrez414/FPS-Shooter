using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levitateObject : MonoBehaviour
{
    // variables //
    [SerializeField] float speed;
    [SerializeField] float verticalSpeed;
    [SerializeField] float height;

    private Vector3 initialPosition;

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // rotation //
        transform.Rotate(new Vector3(0, 45, 0) * Time.deltaTime);

        // up and down movement //
        // Mathf.Sin -> creates sin wave that moves between -1 and 1
        float yPos = initialPosition.y + Mathf.Sin(Time.time * verticalSpeed) * height;
        // calculated y postition with and existing x/z positions
        transform.position = new Vector3(transform.position.x, yPos, transform.position.z);

    }
}
