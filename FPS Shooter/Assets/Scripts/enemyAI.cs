using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class enemyAI : MonoBehaviour, IDamage
{
    //Componets and variables//
    [Header("----- Components -----")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform headPos;
    [SerializeField] Transform shootPos;
    [SerializeField] Material model;
    [SerializeField] Animator animator;


    [Header("----- Enemy Stats -----")]
    [SerializeField] int HP;
    private int maxHP;
    [SerializeField] int sightAngle;
    [SerializeField] int playerFaceSpeed;
    [SerializeField] int roamPauseTime;
    [SerializeField] int roamDist;
    [SerializeField] float animTransSpeed;

    Vector3 playerDir;
    float angleToPlayer;
    bool playerInRange;

    // health bar canvas //
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image healthLeft;

    [Header("----- Gun Stats -----")]
    [Range(1, 10)][SerializeField] int shootDamage;
    [Range(0.1f, 5f)][SerializeField] float fireRate;
    [Range(1, 100)][SerializeField] int shootDist;
    [SerializeField] GameObject bullet;
    [SerializeField] int bulletSpeed;
    bool isShooting;

    [SerializeField] GameObject drop;
    float stoppingDistanceOrig;
    bool destinationChosen;
    Vector3 startingPos;
    float speed;


    void Start()
    {
        //gameManager.instance.updateGameGoal(1); // for winning when game goal is 0 
        stoppingDistanceOrig = agent.stoppingDistance;
        startingPos = transform.position;

        // health bar setup //
        maxHP = HP;
        healthSlider.maxValue = maxHP;
        healthSlider.value = HP;

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (agent.isActiveAndEnabled)
        {
            speed = Mathf.Lerp(speed, agent.velocity.normalized.magnitude, Time.deltaTime * animTransSpeed);
            animator.SetFloat("Speed", speed);

            if (playerInRange && !canSeePlayer())
            {
                StartCoroutine(roam());
            }
            else if (agent.destination != gameManager.instance.player.transform.position)
            {
                StartCoroutine(roam());
            }
        }
    }

    IEnumerator roam()
    {
        if (!destinationChosen && agent.remainingDistance < 0.05f)
        {
            destinationChosen = true;

            agent.stoppingDistance = 0;

            yield return new WaitForSeconds(roamPauseTime);
            destinationChosen = false;

            // generate roandom pos to go to //
            Vector3 randPos = Random.insideUnitSphere * roamDist;
            // keep unit in roam area of start pos //
            randPos += startingPos;

            // check pos then move to pos //
            NavMeshHit hit;
            NavMesh.SamplePosition(randPos, out hit, roamDist, 1);
            agent.SetDestination(hit.position);
        }
    }

    bool canSeePlayer()
    {
        // calculate direction to player chest pos //                                   
        Vector3 playerChestPos = gameManager.instance.player.transform.position;
        playerChestPos.y += gameManager.instance.player.transform.localScale.y / 2;
        playerDir = (playerChestPos - headPos.position);

        // Calculate angle between enemy's forward direction and player //
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);

        Debug.DrawRay(headPos.position, playerDir);
        // raycast check if enemy has line of sight to the player // 
        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            // if raycast hit player and if player is in enemy's field of view //
            if (hit.collider.CompareTag("Player") && angleToPlayer <= sightAngle)
            {
                agent.stoppingDistance = stoppingDistanceOrig;
                agent.SetDestination(gameManager.instance.player.transform.position);

                // Face player if enemy is close enough //
                if (agent.remainingDistance < agent.stoppingDistance)
                    facePlayer();

                // Shoot if enemy is not shooting //
                if (!isShooting)
                {
                    StartCoroutine(shoot());
                }

                return true;
            }
        }
        return false;
    }

    IEnumerator shoot()
    {
        isShooting = true;
        animator.SetTrigger("Shoot");

        // Calculate direction from shootPos to player's chest //
        Vector3 playerChestPos = gameManager.instance.player.transform.position;
        playerChestPos.y += gameManager.instance.player.transform.localScale.y / 2;
        Vector3 bulletDirection = (playerChestPos - shootPos.position).normalized;

        // Instantiate bullet and set initial velocity //
        GameObject bulletClone = Instantiate(bullet, shootPos.position, Quaternion.LookRotation(bulletDirection));
        bulletClone.GetComponent<Rigidbody>().velocity = bulletDirection * bulletSpeed;

        yield return new WaitForSeconds(fireRate);
        isShooting = false;
    }

    //Check for player when something enters its collider
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    //Check for player when something exits its collider
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        StartCoroutine(flashColor());


        healthSlider.value = HP;
        healthLeft.fillAmount = (float)HP / maxHP;
        // Set destination of enemy to player's position //
        agent.SetDestination(gameManager.instance.player.transform.position);
        // Reset stopping distance for Agent //
        agent.stoppingDistance = 0;

        // if dead //
        if (HP <= 0)
        {
            StopAllCoroutines();

            if (drop)
                Instantiate(drop, transform.position, drop.transform.rotation);

            GetComponent<CapsuleCollider>().enabled = false;
            agent.enabled = false;

            Destroy(gameObject); // replace this with death animation

        }
        // if not dead //
        else
        {
            animator.SetTrigger("Damage");
            agent.SetDestination(gameManager.instance.player.transform.position);
            agent.stoppingDistance = 0;
        }
    }

    //Flashes the enemy red
    IEnumerator flashColor()
    {
        model.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.color = Color.white;
    }

    //Faces the player
    void facePlayer()
    {
        // Calculate rotation needed to face player //
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * playerFaceSpeed);
    }
}
