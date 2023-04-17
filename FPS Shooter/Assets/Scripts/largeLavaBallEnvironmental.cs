using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class largeLavaBallEnvironmental : MonoBehaviour
{
    // variables //
    [SerializeField] float minYPos;
    [SerializeField] Vector2 launchForceRange;
    [SerializeField] Vector2 launchArcRange;
    private float initialTrailTime;

    private Rigidbody rb;
    private TrailRenderer trailRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        trailRenderer = GetComponent<TrailRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < minYPos)
        {
            trailRenderer.enabled = false;
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

        trailRenderer.Clear();
        trailRenderer.enabled = true;
    }

    public void LaunchWithForceAndArc(float force, float arc)
    {
        // Apply the random launch force and arc
        float randomYRotation = Random.Range(0, 360);
        Vector3 launchDirection = Quaternion.Euler(arc, randomYRotation, 0) * Vector3.forward;
        launchDirection.y = Mathf.Abs(launchDirection.y); // Ensure the launch direction is upwards
        rb.velocity = Vector3.zero;
        rb.AddForce(launchDirection * force, ForceMode.Impulse);
    }
}
