using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lavaBallEnvironmental : MonoBehaviour
{
    // variables //
    [SerializeField] float launchForce;
    [SerializeField] float timeToLive;

    private Rigidbody rb;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // apply force to launch lava ball up //
        rb.AddForce(Vector3.up * launchForce, ForceMode.Impulse);

        // destroy lava ball after time passed //
        Destroy(gameObject, timeToLive);
    }
}
