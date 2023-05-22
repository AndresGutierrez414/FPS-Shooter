using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class enemyAI : MonoBehaviour, IDamage
{
    //Componets and variables//
    [Header("----- Components -----")]
    [SerializeField] public NavMeshAgent agent;
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
    [SerializeField] private bool isBoss;
    public bool isBossDestroyed { get; private set; }


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

    [Header("----- Death Explosion Settings -----")]
    [SerializeField] private float explosionRadius;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float launchDuration; // Adjust the value to control the duration of the launch effect
    [SerializeField] private float elapsedTime;
    [SerializeField] private float launchHeight; // Adjust the value to control the launch height
    [SerializeField] private float launchDistance; // Adjust the value to control the launch distance

    [Header("----- Rising Settings -----")]
    [SerializeField] private bool riseFromGround;
    [SerializeField] public float riseDelay;
    [SerializeField] private float riseTime;
    private float countdownTime;
    [SerializeField] private GameObject countdownDisplayParent;
    [SerializeField] private TextMeshProUGUI countdownDisplay;
    [SerializeField] private float yOffset;
    private Vector3 initialPosition;
    private Vector3 offsetPosition;
    [SerializeField] public UnityEvent onBossRising;

    [Header("----- Collider Child Objects -----")]
    [SerializeField] private GameObject sphereColliderChild;

    [Header("----- Intro Enemy Settings -----")]
    [SerializeField] public bool isIntroEnemy = false;

    // other variables //
    Vector3 playerDir;
    float angleToPlayer;
    bool playerInRange;
    float stoppingDistanceOrig;
    bool destinationChosen;
    Vector3 startingPos;
    float speed;
    public int experienceOnKill;
    public bool frozen = false;
    //public GameObject box;
    private int easyEnemyKill;
    private int medEnemyKill;
    private int hardEnemyKill;
    private int BossKill;

    void Start()
    {
        easyEnemyKill = Random.Range(5, 60);
        medEnemyKill = Random.Range(60, 130);
        hardEnemyKill = Random.Range(130, 380);
        BossKill = Random.Range(380, 850);

        if (isBoss == true)
        {
           // if (box.CompareTag("Player"))
            //{


                countdownTime = riseDelay;
                StartCoroutine(CountdownToStart());
            //}
        }
       
        // Store the initial position
        initialPosition = transform.position;
        offsetPosition = new Vector3(initialPosition.x, initialPosition.y - yOffset, initialPosition.z);
        transform.position = offsetPosition;
        

        if (!isIntroEnemy && riseFromGround)
        {
            agent.enabled = false;
            GetComponent<CapsuleCollider>().enabled = false;
            StartCoroutine(RiseFromGround(riseDelay, riseTime));
        }
        else if (isIntroEnemy)
        {
            agent.enabled = false;
            GetComponent<CapsuleCollider>().enabled = false;
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

            // if moving, update look direction //
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
            if (agent.enabled == true)
            {
                agent.SetDestination(hit.position);
            }
          
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
        if (riseFromGround && !agent.enabled)
            yield break;

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

    public void playerEnteredRange()
    {
        playerInRange = true;
    }

    public void playerExitedRange()
    {
        playerInRange = false;
        agent.stoppingDistance = 0;
    }

   public void Freeze(int freezeTime)
    {
        if (frozen == false)
        {
            frozen = true;
            this.GetComponent<Rigidbody>().isKinematic = true;
            agent.enabled = false;
                StartCoroutine(EnableTime(freezeTime));
        }
        
    }

    IEnumerator  EnableTime(int freezeTime)
    {
        yield return new WaitForSeconds(freezeTime);
        agent.enabled = true;
        this.GetComponent<Rigidbody>().isKinematic = false;
        frozen = false;
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        StartCoroutine(flashColor());


        healthSlider.value = HP;
        healthLeft.fillAmount = (float)HP / maxHP;
        // Set destination of enemy to player's position //
        if (agent.enabled == true)
        {
            agent.SetDestination(gameManager.instance.player.transform.position);
        }
       
        // Reset stopping distance for Agent //
        agent.stoppingDistance = 0;

        // if dead //
        if (HP <= 0)
        {

            StopAllCoroutines();

            Exp();

            if (drop && Random.Range(0, 2) == 0)
                Instantiate(drop, transform.position, drop.transform.rotation);
            GetComponent<Rigidbody>().isKinematic =  true;
            GetComponent<CapsuleCollider>().enabled = false;
            agent.enabled = false;

            animator.enabled = false;
            StartCoroutine(deathAnimation(deathAnimationTime));

            if (isBoss)
            {
                PlayerStats Stats = FindObjectOfType<PlayerStats>();
                isBossDestroyed = true;
                Stats.BossKilled();
            }
        }
        // if not dead //
        else
        {
            animator.SetTrigger("Damage");
            if (agent.enabled == true)
            {
                agent.SetDestination(gameManager.instance.player.transform.position);
            }
           
            agent.stoppingDistance = 0;
        }
    }
    public void Exp()
    {
        // Add experience to the player
        PlayerStats playerStats = FindObjectOfType<PlayerStats>();

        if (playerStats != null)
        {
            
            if (gameObject.CompareTag("Easy Enemy"))
            {
                playerStats.IncreaseEasyKill(easyEnemyKill);
            }
            else if (gameObject.CompareTag("Medium Enemy"))
            {
                playerStats.IncreaseMedKill(medEnemyKill);
            }
            else if (gameObject.CompareTag("Hard Enemy"))
            {
                playerStats.IncreaseHardKill(hardEnemyKill);
            }
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

        GameObject exlosion = Instantiate(deathExplosionPrefab, transform.position, Quaternion.Euler(-90, transform.eulerAngles.y, transform.eulerAngles.z));
        playExplosionSound();

        applyExplosionDamage(transform.position, explosionRadius, shootDamage);

        // Destroy the GameObject after the set time
        Destroy(gameObject, time);
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

    private void applyExplosionDamage(Vector3 explosionPosition, float explosionRadius, int damage)
    {
        // Get all colliders inside the explosion radius
        Collider[] hitColliders = Physics.OverlapSphere(explosionPosition, explosionRadius, playerLayer);

        // Iterate through all the colliders and apply damage to the player
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                IDamage playerDamage = hitCollider.GetComponent<IDamage>();
                if (playerDamage != null)
                {
                    playerDamage.takeDamage(damage);
                }

                // Launch the player upwards and away from the explosion using a coroutine
                StartCoroutine(launchPlayer(hitCollider.transform, explosionPosition));
            }
        }
    }

    private IEnumerator launchPlayer(Transform playerTransform, Vector3 explosionPosition)
    {
        Vector3 initialPosition = playerTransform.position;
        Vector3 targetPosition = initialPosition + (playerTransform.position - explosionPosition).normalized * launchDistance;
        targetPosition.y += launchHeight;

        while (elapsedTime < launchDuration)
        {
            playerTransform.position = Vector3.Lerp(initialPosition, targetPosition, (elapsedTime / launchDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    IEnumerator CountdownToStart()
    {
        while (countdownTime > 0)
        {
            countdownDisplay.text = countdownTime.ToString();

            yield return new WaitForSeconds(1f);

            countdownTime--;
        }

        countdownDisplay.text = "0";

        yield return new WaitForSeconds(1f);
        countdownDisplayParent.SetActive(false);
       
    }
    public void startBossRising()
    {
         if(!agent.enabled && !GetComponent<CapsuleCollider>().enabled)
        {
           
            StartCoroutine(RiseFromGround(riseDelay, riseTime));
            onBossRising?.Invoke();
        }
    }
}
