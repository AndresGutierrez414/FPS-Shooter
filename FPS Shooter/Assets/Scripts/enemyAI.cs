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
    [SerializeField] Transform headPos;                     //Enemy line of sight position and projectile firing position
    [SerializeField] Transform shootPos;
    [SerializeField] Material model;                        /***Chance: Original code grabbed the color from the renderer,
    //[SerializeField] Renderer model;                          but the ones we are using have their heirarchy set up weird***/

    [Header("----- Enemy Stats -----")]
    [SerializeField] int HP;                                //Enemy max hit points and current hit points
    private int maxHP;

    [SerializeField] int sightAngle;                        //Enemy stationary rotation speed and line of sight arc angle
    [SerializeField] int playerFaceSpeed;
    float stoppingDistanceOrig;                             //Tracking the original stopping distance

    Vector3 playerDir;                                      //Player information
    float angleToPlayer;
    bool playerInRange;


    [SerializeField] private Slider healthSlider;           //Enemy health bar UI and health bar UI filler 
    [SerializeField] private Image healthLeft;

    [Header("----- Gun Stats -----")]                       //Enemy weapon statistics
    [Range(1, 10)][SerializeField] int shootDamage;
    [Range(0.1f, 5f)][SerializeField] float fireRate;
    [Range(1, 100)][SerializeField] int shootDist;
    bool isShooting;

    [SerializeField] GameObject bullet;                     //Enemy weapon projectile model and speed
    [SerializeField] int bulletSpeed;

    private Animator animator;                              //Enemy animator and death drop game object
    [SerializeField] GameObject drop;

    void Start()
    {
        gameManager.instance.updateGameGoal(1);
        stoppingDistanceOrig = agent.stoppingDistance;

        // health bar setup //
        maxHP = HP;
        healthSlider.maxValue = maxHP;
        healthSlider.value = HP;

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        //agent.speed = movementSpeed; // test  /***Chance: This made like 999+ warning in Unity. What are you trying to do here?***/
        float agentSpeed = agent.velocity.magnitude;
        animator.SetFloat("Speed", agentSpeed);

        if (playerInRange)
        {
            canSeePlayer();
        }
    }

    //Checks if 
    bool canSeePlayer()
    {
        // calculate direction to player chest pos //                                   /***Chance: What's the point to all this?***/
        Vector3 playerChestPos = gameManager.instance.player.transform.position;        /***How it different from casting to this value?***/
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
        animator.SetTrigger("Attack");

        isShooting = true;

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
        animator.SetTrigger("Damaged");

        HP -= amount;
        healthSlider.value = HP;
        healthLeft.fillAmount = (float)HP / maxHP;
        // Set destination of enemy to player's position //
        agent.SetDestination(gameManager.instance.player.transform.position);
        // Reset stopping distance for Agent //
        agent.stoppingDistance = 0;

        StartCoroutine(flashColor());

        if (HP <= 0)
        {
            Instantiate(drop, transform.position, drop.transform.rotation);
            gameManager.instance.updateGameGoal(-1);
            Destroy(gameObject);
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
