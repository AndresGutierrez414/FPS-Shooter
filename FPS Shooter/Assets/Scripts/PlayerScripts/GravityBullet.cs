using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GravityBullet : MonoBehaviour
{
    // variables //
    [SerializeField] public int damage;
    [SerializeField] public int maxTravelDistance;
    private Vector3 startPos;
    public float knockbackForce = 5f;
  
    public string enemyTag = "Enemy";

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

        if (gameManager.instance.gravityUpgrade == true)
        {
            if (other.CompareTag(enemyTag))
            {
                Rigidbody enemyRb = other.GetComponent<Rigidbody>();
                if (enemyRb != null)
                {
                    Vector3 knockbackDirection = -other.transform.forward;
                    enemyRb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
                }
            }
        }
        Destroy(gameObject);
    }

   
}
