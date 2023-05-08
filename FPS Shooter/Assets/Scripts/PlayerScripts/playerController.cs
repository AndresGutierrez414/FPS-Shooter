using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage
{
    //Componets and variables//
    [Header("----- Components -----")]
    [SerializeField] private CharacterController controller;
    [SerializeField] Animator animator; // human character
    [SerializeField] AudioSource audio;


    [Header("----- Player Stats -----")]                        //Player current hit points and maximum hit points
    [Range(1, 10)][SerializeField] public int HP;
    int maxHP;

    [Range(3, 8)][SerializeField] public float playerSpeed;     //Player movement speed and current velocity
    [Range(4, 10)][SerializeField] public float playerSprint;          // Used for player sprint
    [Range(0, 5)][SerializeField] public float sprintAcceleration;     // Player sprint acceleration
    [Range(0, 10)][SerializeField] float sprintDrainRate;       // Player sprint drain rate
    [Range(0, 10)][SerializeField] float sprintRechargeRate;    // Player sprint recharge rate;
    Vector3 playerVelocity;
    Vector3 movementVec;

    [Range(10, 50)][SerializeField] float gravityValue;         //Player jump statistics and gravity value
    [Range(8, 25)][SerializeField] float jumpHeight;
    [Range(1, 3)][SerializeField] int maxJumps;
    int timesJumped;
    bool isPlayerGrounded;

    [Header("----- Gun Stats -----")]                           //Weapon statistics 
    public List<GunLists> gunList = new List<GunLists>();
    [Range(1, 10)][SerializeField] int shootDamage;
    [Range(0.1f, 5f)][SerializeField] float fireRate;
    [Range(1, 100)][SerializeField] int shootDist;
    [SerializeField] GameObject bullet;
    [SerializeField] int bulletSpeed;
    [SerializeField] Transform shootPos;
    public MeshRenderer gunMaterial;
    public MeshFilter gunModel;
    public int selectedGun;

    [Header("----- Pillow Stats -----")]                           //Weapon statistics 
    [Range(0.8f, 2f)][SerializeField] float pillowShootRate;
    [Range(1, 4)][SerializeField] int pillowShootDist;
    [SerializeField] GameObject cube;                              //Pillow object

    [Header("----- Lava Floor -----")]                           
    [SerializeField] public GameObject lavaFloor;
    [SerializeField] public lavaFloor lavaFloorScript;

    [Header("----- Audio -----")]
    [SerializeField] AudioClip[] audioSteps;
    [SerializeField][Range(0, 1)] float audioStepsVolume;
    [SerializeField] AudioClip[] audioJump;
    [SerializeField][Range(0, 1)] float audioJumpVolume;
    [SerializeField] AudioClip[] audioDamage;
    [SerializeField][Range(0, 1)] float audioDamageVolume;

    [Header("----- Effects -----")]
    [SerializeField] GameObject fireDamage;

    bool isPlayingSteps;

    private float currentSpeed;
    private float targetSpeed;
    bool isShooting;
    bool isPlacingP;
    bool isSprinting;
    public float rotationAxis;

    public bool canMove = false;

    private void Awake()
    {
        lavaFloor = GameObject.FindGameObjectWithTag("Floor");
        lavaFloorScript = lavaFloor.GetComponent<lavaFloor>();
    }

    private void Start()
    {
        maxHP = HP;
        playerUIUpdate();
        respawnPlayer();
    }

    void Update()
    {
        if (canMove)
        {
            if (gameManager.instance.activeMenu == null)
            {
                selectGun();
                Movement();

                if (gunList.Count > 0 && !isShooting && Input.GetButton("Shoot"))
                    StartCoroutine(shootBullet());
                //StartCoroutine(shoot());

                if (!isPlacingP && Input.GetButton("Fire2"))         //Check for mouse 2 press
                    StartCoroutine(placePillow());

            }
        }
    }

    //Player movement handler
    void Movement()
    {
        isPlayerGrounded = controller.isGrounded;       //Reset player.y velocity and times jumped if the player is grounded
        if (isPlayerGrounded)
        {
            if (!isPlayingSteps && movementVec.normalized.magnitude > 0.5)
            {
                StartCoroutine(playSteps());
            }
            if (playerVelocity.y < 0)
            {
                playerVelocity.y = 0f;
                timesJumped = 0;
            }
        }
        
        //if (isPlayerGrounded && playerVelocity.y < 0)
        //{
        //    playerVelocity.y = 0f;
        //    timesJumped = 0;
        //}

        float targetSpeed = isSprinting ? playerSprint : playerSpeed;   // Determine the target speed based on whether the player is sprinting or not

        movementVec = (transform.right * Input.GetAxis("Horizontal")) + (transform.forward * Input.GetAxis("Vertical"));

        if (gameManager.instance.SprintBar.fillAmount > 0 && Input.GetKeyDown(KeyCode.LeftShift) && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
        {
            isSprinting = true; // Start sprinting when left shift key is pressed and player is moving, and fill amount is greater than 0
        }

        if (!isSprinting || gameManager.instance.SprintBar.fillAmount <= 0 || Input.GetKeyUp(KeyCode.LeftShift))
        {
            isSprinting = false; // Stop sprinting when left shift key is released or fill amount is 0
        }

        if (isSprinting)
        {
            gameManager.instance.SprintBar.fillAmount -= sprintDrainRate * Time.deltaTime; // Decrease fill amount based on sprint drain rate
        }
        else
        {
            gameManager.instance.SprintBar.fillAmount += sprintRechargeRate * Time.deltaTime; // Increase fill amount based on sprint recharge rate
        }

        // Clamp the fill amount of the SprintBar to be within [0, 1] range
        gameManager.instance.SprintBar.fillAmount = Mathf.Clamp01(gameManager.instance.SprintBar.fillAmount);

        // Smoothly transition the player's speed to the target speed using sprintAcceleration
        float currentSpeed = Vector3.ProjectOnPlane(controller.velocity, Vector3.up).magnitude;
        float speedDifference = targetSpeed - currentSpeed;
        float acceleration = isSprinting ? sprintAcceleration : 1f;
        movementVec *= (currentSpeed + speedDifference * acceleration) * Time.deltaTime;

        controller.Move(movementVec); //Applies user input to the controller

        // Calculate and set the speed parameter for the Animator
        float animationSpeed = movementVec.magnitude / Time.deltaTime; // human test
        animator.SetFloat("Speed", animationSpeed); // human test

        if (Input.GetButtonDown("Jump") && timesJumped < maxJumps)  //Check for space bar press. Handles max amounts of jumps
        {
            animator.SetTrigger("Jump");
            audio.PlayOneShot(audioJump[Random.Range(0, audioJump.Length)], audioJumpVolume);
            timesJumped++;
            playerVelocity.y = jumpHeight;
        }

        playerVelocity.y -= gravityValue * Time.deltaTime;      //Applies gravity (and jump if there has been one) to the controller
        controller.Move(playerVelocity * Time.deltaTime);
    }

    IEnumerator playSteps()
    {
        isPlayingSteps = true;

        audio.PlayOneShot(audioSteps[Random.Range(0, audioSteps.Length)], audioStepsVolume);

        if (!isSprinting)
            yield return new WaitForSeconds(0.5f);
        else
            yield return new WaitForSeconds(0.3f);

        isPlayingSteps = false;
    }

    //Called whenever the player takes damage
    public void takeDamage(int amount)
    {
        audio.PlayOneShot(audioDamage[Random.Range(0, audioJump.Length)], audioDamageVolume);

        HP -= amount;                                           //Updates the player's hit points and the respective UI element
        playerUIUpdate();

        if (HP <= 0)                                            //Triggers a losing state if the player has no more hit points
        {
            gameManager.instance.playerDead();
        }
    }

    //Updates the UI elements regarding the player (called every frame)
    void playerUIUpdate()
    {
        gameManager.instance.HPBar.fillAmount = (float)HP / (float)maxHP;
    }

    //Coroutine handling shooting
    IEnumerator shoot()
    {
        isShooting = true;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDist))
        {
            IDamage damagable = hit.collider.GetComponent<IDamage>(); //Checks for IDamage on collision

            if (damagable != null)
            {
                damagable.takeDamage(shootDamage);              //Applies damage accordingly if IDamage is found
            }
        }

        yield return new WaitForSeconds(fireRate);
        isShooting = false;
    }

    IEnumerator shootBullet()
    {
        isShooting = true;

        audio.PlayOneShot(gunList[selectedGun].gunBlastAudio, gunList[selectedGun].gunShotAudioVolume);

        int bulletCount = gunList[selectedGun].bulletCount;

        // for each bullet //
        for (int i = 0; i < bulletCount; i++)
        {
            RaycastHit hit;
            Vector3 targetPoint;
            float maxRaycastDistance = 1000f;

            if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDist))
            {
                // If the raycast hits an object, use the hit point as the target //
                targetPoint = hit.point;
            }
            else
            {
                // If the raycast doesn't hit anything, calculate a point at the maximum raycast distance //
                targetPoint = Camera.main.transform.position + (Camera.main.transform.forward * maxRaycastDistance);
            }

            // apply rand angle offset if shotgun //
            float spreadAngle = gunList[selectedGun].spreadAngle;
            Quaternion randomRotation = Quaternion.Euler(
                Random.Range(-spreadAngle, spreadAngle),
                Random.Range(-spreadAngle, spreadAngle),
                0);

            // Calculate the direction vector from the shootPos to the target point
            Vector3 shootDirection = (targetPoint - shootPos.position).normalized;
            shootDirection = randomRotation * shootDirection;

            // Instantiate the bullet with the correct rotation
            GameObject bulletClone = Instantiate(bullet, shootPos.position, Quaternion.LookRotation(shootDirection));
            bulletClone.GetComponent<Rigidbody>().velocity = shootDirection * bulletSpeed;

            playerBullet bulletScript = bulletClone.GetComponent<playerBullet>();
            if (bulletScript != null)
            {
                bulletScript.damage = shootDamage;
                bulletScript.maxTravelDistance = gunList[selectedGun].shootingDist;
            }
        }

        yield return new WaitForSeconds(fireRate);
        isShooting = false;
    }

    //This coroutine handles throwing pillows
    IEnumerator placePillow()
    {
        isPlacingP = true;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, pillowShootDist))
        {
            GameObject pillow = Instantiate(cube, hit.point, Quaternion.identity);   //Instantiates cube at the hit location // transform.rotation

            // allign pillow up vector with surface //
            //pillow.transform.forward = hit.normal;     // <- maybe
        }

        yield return new WaitForSeconds(pillowShootRate);
        isPlacingP = false;
    }

    public void reFillHealth(int amount)
    {
        HP += amount;

        HP = Mathf.Clamp(HP, 0, maxHP);

        playerUIUpdate();
    }
    //Function that will respawn the player
    public void respawnPlayer()
    {
        HP = maxHP;
        playerUIUpdate();
        controller.enabled = false;
        transform.position = gameManager.instance.playerSpawnLocation.transform.position;
        controller.enabled = true;

        // clear player collider from hashset list in lavaFloor script //
        lavaFloorScript.playerInLava.Clear();
        //lavaFloorScript.resetStats(this);
    }
    public void gunPick(GunLists gunStat)
    {
        ApplyGunTransform(gunStat);

        gunList.Add(gunStat);

        shootDamage = gunStat.shootingDamage;
        shootDist = gunStat.shootingDist;
        fireRate = gunStat.shootingRate;

        gunModel.mesh = gunStat.gunModel.GetComponent<MeshFilter>().sharedMesh;
        MeshRenderer gunRenderer = gunStat.gunModel.GetComponent<MeshRenderer>();
        Material[] gunMaterials = gunRenderer.sharedMaterials;
        gunMaterial.materials = gunMaterials;

        selectedGun = gunList.Count - 1;
    }
    void selectGun()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && selectedGun < gunList.Count - 1)
        {
            selectedGun++;
            changeGun();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && selectedGun > 0)
        {
            selectedGun--;
            changeGun();
        }
    }
    void changeGun()
    {
        shootDamage = gunList[selectedGun].shootingDamage;
        shootDist = gunList[selectedGun].shootingDist;
        fireRate = gunList[selectedGun].shootingRate;

        gunModel.mesh = gunList[selectedGun].gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunMaterial.material = gunList[selectedGun].gunModel.GetComponent<MeshRenderer>().sharedMaterial;
        ApplyGunTransform(gunList[selectedGun]);
    }

    private void ApplyGunTransform(GunLists gunStat)
    {
        if (gunStat.name == "StaffStats")
        {
            gunModel.gameObject.transform.localScale = new Vector3(40, 40, 40);
            gunModel.gameObject.transform.localPosition = new Vector3(0.35f, -0.61f, 0.337f);
            gunModel.gameObject.transform.localRotation = Quaternion.Euler(new Vector3(-45, 0, 0));
            shootPos.transform.localPosition = new Vector3(0.366f, -0.075f, 1.361f);
        }
        else
        {
            gunModel.gameObject.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            gunModel.gameObject.transform.localRotation = Quaternion.identity;
        }
    }

    //When the player hit the floor the player will be set on fire
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Floor"))
        {
            fireDamage.SetActive(true);
        }
    }
    //When the player leaves the floor the fire will be set off
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Floor"))
        {
            fireDamage.SetActive(false);
        }
    }
}