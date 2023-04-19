using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossBullet : MonoBehaviour
{
    // variables //
    [Header("----------Stats----------")]
    [SerializeField] int damage;
    [SerializeField] int timer;
    [SerializeField] float rotationSpeed;

    [Header("----------Effects----------")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private AudioClip explosionSound;
    [Range(0, 1)][SerializeField] private float audioVolume;
    [SerializeField] private float audioDistance;

    // Start is called before the first frame update
    void Start()
    {
        // destroy projectile after set time //
        Destroy(gameObject, timer);
    }

    private void Update()
    {
        // rotate projectile //
        transform.Rotate(rotationSpeed * Time.deltaTime, rotationSpeed * Time.deltaTime, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamage damagable = other.GetComponent<IDamage>();
        // check if collision is with object that can take damage //
        if (damagable != null)
        {
            damagable.takeDamage(damage);
            GameObject exlosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            playExplosionSound();

        }
        GameObject exlosionFloor = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        playExplosionSound();

        Destroy(gameObject);
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
