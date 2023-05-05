using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class largeLavaBallEnvironmental : MonoBehaviour
{
    // variables //
    [Header("----------Stats----------")]
    [SerializeField] float minYPos;
    [SerializeField] Vector2 launchForceRange;
    [SerializeField] Vector2 launchArcRange;
    private float initialTrailTime;

    [Header("----------Components----------")]
    private Rigidbody rb;
    private TrailRenderer trailRenderer;

    [Header("----------Explosion Effects----------")]
    [SerializeField] private GameObject bigExplosionPrefab;
    [SerializeField] private GameObject sparklesPrefab;
    [SerializeField] private GameObject showckwavePrefab;
    [SerializeField] private GameObject verticalEffectPrefab;
    [SerializeField] private AudioClip explosionSound;
    [Range(0, 1)][SerializeField] private float audioVolume;
    [SerializeField] private float audioDistance;

    [Header("----------Explosion Effects to player----------")]
    [SerializeField] private float explosionRadius;
    [SerializeField] private float cameraShakeDuration;
    [SerializeField] private float cameraShakeMagnitude;

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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Floor") || collision.collider.CompareTag("Decor"))
        {
            createExplosion();
            playExplosionSound();
            trailRenderer.enabled = false;

            // check if player is within explosion radius //
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            // if player is in explosion radius -> shake camera //
            if (distanceToPlayer <= explosionRadius)
            {
                cameraShake cameraShook = Camera.main.GetComponent<cameraShake>();
                cameraShook.shake(cameraShakeDuration, cameraShakeMagnitude);
            }

            gameObject.SetActive(false);
        }
    }

    private void createExplosion()
    {
        GameObject bigExplosion = Instantiate(bigExplosionPrefab, transform.position, Quaternion.identity);
        GameObject sparkles = Instantiate(sparklesPrefab, transform.position, Quaternion.identity);
        GameObject shockwave = Instantiate(showckwavePrefab, transform.position, Quaternion.identity);
        GameObject verticalEffect = Instantiate(verticalEffectPrefab, transform.position, Quaternion.identity);
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

    private void playExplosionSound()
    {
        if (explosionSound != null)
        {
            // create an new GameObject at explosion location //
            GameObject audioObject = new GameObject("ExplosionAudio");
            audioObject.transform.position = transform.position;

            // add audio source component to new object //
            AudioSource audioSource = audioObject.AddComponent<AudioSource>();

            // configure audio source component //
            audioSource.clip = explosionSound;
            audioSource.spatialBlend = 1; // 1 -> for 3D sound
            // set volume rolloff to logarithmic and adjust max distance
            audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
            audioSource.maxDistance = audioDistance;
            // adjust volume
            audioSource.volume = audioVolume;

            audioSource.Play();

            // destroy audio source after done playing sound //
            Destroy(audioObject, explosionSound.length);
        }
    }
}
