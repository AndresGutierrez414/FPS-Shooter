using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    // variables //
    [Header("----------Stats----------")]
    [SerializeField] int damage;
    [SerializeField] int timer;
    [SerializeField] float rotationSpeed;

    [Header("----------Components/Prefabs----------")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private AudioClip explosionSound;

    private AudioSource audioSource;


    void Start()
    {
        // destroy projectile after set time //
        Destroy(gameObject, timer);

        audioSource = GetComponent<AudioSource>();
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

            // explosion effect //
            GameObject exlosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            // explosion audio //
            playExplosionSound();
        }
        // explosion effect //
        GameObject exlosionFloor = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        // explosion audio //
        playExplosionSound();

        Destroy(gameObject);
    }

    private void playExplosionSound()
    {
        if (audioSource != null && explosionSound != null)
        {
            audioSource.PlayOneShot(explosionSound);
        }
    }
}
