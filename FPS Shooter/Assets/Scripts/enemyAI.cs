using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class enemyAI : MonoBehaviour, IDamage
{
    // variables //
    [Header("----- Components -----")]
    //[SerializeField] Renderer model;
    [SerializeField] Material model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform headPos;
    [SerializeField] Transform shootPos;

    [Header("----- Enemy Stats -----")]
    [SerializeField] int HP;
    private int maxHP;
    [SerializeField] private Slider healthSlider; // health bar
    [SerializeField] private Image healthLeft; // health bar filler
    [SerializeField] int playerFaceSpeed;
    [SerializeField] int sightAngle;

    [Header("----- Gun Stats -----")]
    [Range(1, 10)][SerializeField] int shootDamage;
    [Range(0.1f, 5f)][SerializeField] float shootRate;
    [Range(1, 100)][SerializeField] int shootDist;
    [SerializeField] GameObject bullet;
    [SerializeField] int bulletSpeed;

    private Animator animator;
    [SerializeField] GameObject drop;

    Vector3 playerDir;
    bool playerInRange;
    float angleToPlayer;
    bool isShooting;
    float stoppingDistanceOrig;


    // Start is called before the first frame update
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

    // Update is called once per frame
    void Update()
    {
        //agent.speed = movementSpeed; // test  //Chance: This made like 999+ warning in Unity. What are you trying to do here?
        float agentSpeed = agent.velocity.magnitude;
        animator.SetFloat("Speed", agentSpeed);

        if (playerInRange)
        {
            canSeePlayer();
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
        animator.SetTrigger("Attack");

        isShooting = true;

        // Calculate direction from shootPos to player's chest //
        Vector3 playerChestPos = gameManager.instance.player.transform.position;
        playerChestPos.y += gameManager.instance.player.transform.localScale.y / 2;
        Vector3 bulletDirection = (playerChestPos - shootPos.position).normalized;

        // Instantiate bullet and set initial velocity //
        GameObject bulletClone = Instantiate(bullet, shootPos.position, Quaternion.LookRotation(bulletDirection));
        bulletClone.GetComponent<Rigidbody>().velocity = bulletDirection * bulletSpeed;

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

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

    IEnumerator flashColor()
    {
        model.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.color = Color.white;
    }

    void facePlayer()
    {
        // Calculate rotation needed to face player //
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * playerFaceSpeed);
    }

   
}
