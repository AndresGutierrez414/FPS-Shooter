using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lavaBallEnvironmental : MonoBehaviour
{
    // variables //
    [SerializeField] float launchForce;
    [SerializeField] float timeToLive;
    [SerializeField] float minYPosition;

    private Rigidbody rb;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (transform.position.y < minYPosition)
        {
            gameObject.SetActive(false);
        }
    }

    // when obj is turned on //
    private void OnEnable()
    {
        if (rb != null)
        {
            Launch();
        }
    }

    private void Launch()
    {
        // apply force to launch lava ball up //
        rb.velocity = Vector3.zero;
        rb.AddForce(Vector3.up * launchForce, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Decor"))
        {
            gameObject.SetActive(false);
        }
    }
}
