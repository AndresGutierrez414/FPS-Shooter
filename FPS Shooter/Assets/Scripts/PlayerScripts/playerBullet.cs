using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerBullet : MonoBehaviour
{
    // variables //
    [SerializeField] public int damage;
    [SerializeField] public int maxTravelDistance;
    [SerializeField] public int particleLifetime;
    [SerializeField] public ParticleSystem particle;
    private Vector3 startPos;
    //[SerializeField] public int timer;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        float travelDistance = Vector3.Distance(startPos, transform.position);

        if (travelDistance > maxTravelDistance)
        {
            Destroy(gameObject);
        }
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

    private void OnDestroy()
    {
        ParticleSystem ps = Instantiate(particle, gameObject.transform.position, gameObject.transform.rotation);
        Destroy(ps, particleLifetime);
    }
}