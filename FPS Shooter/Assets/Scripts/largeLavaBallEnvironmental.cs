using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class largeLavaBallEnvironmental : MonoBehaviour
{
    // variables //
    [SerializeField] float minYPos;
    [SerializeField] Vector2 launchForceRange;
    [SerializeField] Vector2 launchArcRange;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < minYPos)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
            LaunchLavaBall();
    }

    private void LaunchLavaBall()
    {
        float launchForce = Random.Range(launchForceRange.x, launchForceRange.y);
        float launchArc = Random.Range(launchArcRange.x, launchArcRange.y);
        LaunchWithForceAndArc(launchForce, launchArc);
    }

    public void LaunchWithForceAndArc(float force, float arc)
    {
        // Apply the random launch force and arc
        Vector3 launchDirection = Quaternion.Euler(0, arc, 0) * Vector3.up;
        rb.velocity = Vector3.zero;
        rb.AddForce(launchDirection * force, ForceMode.Impulse);
    }
}
