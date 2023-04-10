using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    // variables //
    [SerializeField] int damage;
    [SerializeField] int timer;
    [SerializeField] float rotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
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
        if (damagable != null)
        {
            damagable.takeDamage(damage);
        }

        Destroy(gameObject);
    }
}
