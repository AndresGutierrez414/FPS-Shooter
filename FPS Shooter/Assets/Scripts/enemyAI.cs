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

    [Header("----- Death Settings -----")]
    [SerializeField] float deathAnimationTime;
    [SerializeField] GameObject drop;
    [SerializeField] private GameObject deathExplosionPrefab;
    [SerializeField] private AudioClip explosionSound;
    [Range(0, 1)][SerializeField] private float audioVolume;
    [SerializeField] private float audioDistance;

    [Header("----- Rising Settings -----")]
    [SerializeField] private bool riseFromGround;
    [SerializeField] public float riseDelay;
    [SerializeField] private float riseTime;
    [SerializeField] private float yOffset;
    private Vector3 initialPosition;
    private Vector3 offsetPosition;

    // other variables //
    Vector3 playerDir;
    float angleToPlayer;
    bool playerInRange;
    float stoppingDistanceOrig;
    bool destinationChosen;
    Vector3 startingPos;
    float speed;


    void Start()
    {
        // Store the initial position
        initialPosition = transform.position;
        offsetPosition = new Vector3(initialPosition.x, initialPosition.y - yOffset, initialPosition.z);
        transform.position = offsetPosition;

        if (riseFromGround)
        {
            agent.enabled = false;
            GetComponent<CapsuleCollider>().enabled = false;
            StartCoroutine(RiseFromGround(riseDelay, riseTime));
        }


        stoppingDistanceOrig = agent.stoppingDistance;
        startingPos = transform.position;

        // health bar setup //
        maxHP = HP;
        healthSlider.maxValue = maxHP;
        healthSlider.value = HP;

        animator = GetComponent<Animator>();

        agent.updateRotation = false;
    }

    void Update()
    {
        if (agent.isActiveAndEnabled)
        {
            speed = Mathf.Lerp(speed, agent.velocity.normalized.magnitude, Time.deltaTime * animTransSpeed);
            animator.SetFloat("Speed", speed);

            // if moving update look direction //
            if (agent.velocity.magnitude > 0.1f)
            {
                Quaternion rotation = Quaternion.LookRotation(agent.velocity.normalized);
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * playerFaceSpeed);
            }

            // if player in range and can see player //
            if (playerInRange && !canSeePlayer()) 
            {
                StartCoroutine(roam());
            }
            // if can see player //
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
            agent.stoppingDistance = 0;
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

            animator.enabled = false;
            StartCoroutine(deathAnimation(deathAnimationTime));
        }
        // if not dead //
        else
        {
            animator.SetTrigger("Damage");
            agent.SetDestination(gameManager.instance.player.transform.position);
            agent.stoppingDistance = 0;
        }
    }

    IEnumerator deathAnimation(float time)
    {
        float elapsedTime = 0f;
        Quaternion initialRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(-90, transform.eulerAngles.y, transform.eulerAngles.z);

        while (elapsedTime < time)
        {
            transform.rotation = Quaternion.Lerp(initialRotation, targetRotation, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation;

        GameObject exlosion = Instantiate(deathExplosionPrefab, transform.position, Quaternion.Euler(-90, transform.eulerAngles.y, transform.eulerAngles.z)); // Quaternion.identity
        playExplosionSound();

        // Destroy the GameObject after the set time
        Destroy(gameObject, time);
    }

    IEnumerator RiseFromGround(float delay, float time)
    {
        yield return new WaitForSeconds(delay);

        float elapsedTime = 0f;
       
        transform.position = offsetPosition;

        while (elapsedTime < time)
        {
            transform.position = Vector3.Lerp(offsetPosition, initialPosition, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = initialPosition;

        agent.enabled = true;
        GetComponent<CapsuleCollider>().enabled = true;
        animator.enabled = true;
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
