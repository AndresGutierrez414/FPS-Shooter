using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    // variables //
    [SerializeField] int damage;
    [SerializeField] int timer;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, timer);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("bullet trigger");
        IDamage damagable = other.GetComponent<IDamage>();
        Debug.Log(damagable);
        if (damagable != null)
        {
            Debug.Log("bullet function called");
            damagable.takeDamage(damage);
        }

        Destroy(gameObject);
    }
}
