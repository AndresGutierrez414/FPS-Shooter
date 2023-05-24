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
    [Range(1, 10)] [SerializeField] public int critHP;
    [SerializeField] public int critHPPulseSpeed;
    [SerializeField] public float dmgIndicatorDuration;
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
    public float recoilAmount = 0.1f;  
    public float recoilSpeed = 5.0f;  
    private Vector3 originalPosition;  
    [SerializeField] GameObject bullet;
    [SerializeField] int bulletSpeed;
    [SerializeField] Transform shootPos;
    public MeshRenderer gunMaterial;
    public MeshFilter gunModel;
    public int selectedGun;
    public Transform gunPos;

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
    [SerializeField] AudioClip giveHp;

    [Header("----- Effects -----")]
    [SerializeField] GameObject stunned;
    [SerializeField] GameObject fireDamage;
    [SerializeField] GameObject fireFx;
    [SerializeField] GameObject gravityFx;
    [SerializeField] GameObject iceFx;
    [SerializeField] GameObject rapidFx;
    [SerializeField] float hpDrainSpeed;
    float hpDrainTimer;
    bool isPlayingSteps;
    public Vector3 recoilDirection;
    
    private float currentSpeed;
    private float targetSpeed;
    bool isShooting;
    bool isPlacingP;
    bool isSprinting;
    public bool isCritHP = false;
    public float rotationAxis;
    private bool isSwitching = false;
    private bool changing = false;
    private bool isSelecting = false;
    private float weaponSwitchDelay = 0.1f;
    public bool canMove = false;
    public bool canShoot = true;
    GameObject bulletPrefab; 

    private void Awake()
    {
        lavaFloor = GameObject.FindGameObjectWithTag("Floor");
        lavaFloorScript = lavaFloor.GetComponent<lavaFloor>();
    }

    private void Start()
    {
        originalPosition = gunModel.transform.localPosition;
        maxHP = HP;
        playerUIUpdate();
        respawnPlayer();
    }
    void ApplyRecoil()
    {
        // Get the recoil direction for the currently selected gun
        Vector3 localRecoilDirection = gunList[selectedGun].recoilDirection;

        // Convert the local direction to world space
        Vector3 worldRecoilDirection = gunModel.transform.TransformDirection(localRecoilDirection);

        // Use the recoil amount for the currently selected gun
        float recoilAmount = gunList[selectedGun].recoilAmount;

        // Apply the recoil in world space
        gunModel.transform.position -= worldRecoilDirection * recoilAmount;
        StartCoroutine(MoveGunToPosition(gunModel.transform, originalPosition, recoilSpeed));
    }

    IEnumerator MoveGunToPosition(Transform target, Vector3 position, float speed)
    {
        while (target.localPosition != position)
        {
            target.localPosition = Vector3.MoveTowards(target.localPosition, position, Time.deltaTime * speed);
            yield return null;
        }
    }
    void Update()
    {
        if (canMove)
        {
            if (gameManager.instance.activeMenu == null)
            {
                selectGun();
                Movement();
                playerUIUpdate();

                if (gunList.Count > 0 && !isShooting && Input.GetButton("Shoot"))
                    StartCoroutine(shootBullet());
                //StartCoroutine(shoot());

                if (isCritHP)
                    StartCoroutine(critHealthEffect());
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
            if(canShoot == true)
            {
                animator.SetTrigger("Jump");
                audio.PlayOneShot(audioJump[Random.Range(0, audioJump.Length)], audioJumpVolume);
                timesJumped++;
                playerVelocity.y = jumpHeight;
            }
           
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
        StartCoroutine(showDamageIndicator());

        HP -= amount;                                           //Updates the player's hit points and the respective UI element
        playerUIUpdate();

        if (HP <= critHP)
            isCritHP = true;

        if (HP <= 0)                                            //Triggers a losing state if the player has no more hit points
        {
            canShoot = false;
            gunModel.gameObject.SetActive(false);
            gameManager.instance.playerDead();
        }
    }

    //Updates the UI elements regarding the player (called every frame)
    void playerUIUpdate()
    {
        hpDrainTimer += Time.deltaTime;
        gameManager.instance.HPBar.fillAmount = (float)HP / (float)maxHP;
        gameManager.instance.HPBarDelay.fillAmount = Mathf.Lerp(gameManager.instance.HPBarDelay.fillAmount, (float)HP / (float)maxHP, hpDrainTimer / hpDrainSpeed);
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

    //Creates the flashing low health UI effect - Chance
    IEnumerator critHealthEffect()
    {
        gameManager.instance.critHeathImg.gameObject.SetActive(true);
        Color FXColor = gameManager.instance.critHeathImg.color;

        //Changing the alpha of the crit health image
        FXColor.a = Mathf.Sin(Time.time * critHPPulseSpeed);
        gameManager.instance.critHeathImg.color = FXColor;

        yield return new WaitForSeconds(critHPPulseSpeed * Time.deltaTime);
    }

    public IEnumerator showDamageIndicator()
    {
        // Store original color
        Color originalColor = gameManager.instance.dmgIndicator.color;

        // Set the color alpha to 1
        gameManager.instance.dmgIndicator.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);

        gameManager.instance.dmgIndicator.gameObject.SetActive(true);

        float fadeDuration = 0.2f;
        float startTime = Time.time;

        while (Time.time < startTime + fadeDuration)
        {
            float t = (Time.time - startTime) / fadeDuration;

            Color newColor = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(1, 0, t));

            gameManager.instance.dmgIndicator.color = newColor;

            yield return null;
        }

        gameManager.instance.dmgIndicator.gameObject.SetActive(false);
    }
    public void reFillHealth(int amount)
    {
        audio.clip = giveHp;
        audio.Play();
        HP += amount;

        HP = Mathf.Clamp(HP, 0, maxHP);

        if (HP > critHP)
        {
            isCritHP = false;
            gameManager.instance.critHeathImg.gameObject.SetActive(false);
            
            Color FXColor = gameManager.instance.critHeathImg.color;
            FXColor.a = 0;
            gameManager.instance.critHeathImg.color = FXColor;
        }

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

    IEnumerator shootBullet()
    {
        if (canShoot == true)
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


                GameObject bulletClone = Instantiate(bullet, shootPos.position, Quaternion.LookRotation(shootDirection));
                bulletClone.GetComponent<Rigidbody>().velocity = shootDirection * bulletSpeed;
                ApplyRecoil();
                playerBullet bulletScript;
                if (gunList[selectedGun].name == "FlameStaff")
                {
                    bulletScript = bulletClone.GetComponent<playerBullet>();
                    if (bulletScript != null)
                    {
                        bulletScript.damage = shootDamage;
                        bulletScript.maxTravelDistance = gunList[selectedGun].shootingDist;
                    }

                    RapidfireBullet rapidFireBulletScript;
                    if (gunList[selectedGun].name == "RapidFireStaff")
                    {
                        rapidFireBulletScript = bulletClone.GetComponent<RapidfireBullet>();
                        if (rapidFireBulletScript != null)
                        {
                            rapidFireBulletScript.damage = shootDamage;
                            rapidFireBulletScript.maxTravelDistance = gunList[selectedGun].shootingDist;
                        }
                    }

                    IceBullet iceBulletScript;
                    if (gunList[selectedGun].name == "IceStaff")
                    {
                        iceBulletScript = bulletClone.GetComponent<IceBullet>();
                        if (iceBulletScript != null)
                        {
                            iceBulletScript.damage = shootDamage;
                            iceBulletScript.maxTravelDistance = gunList[selectedGun].shootingDist;
                        }

                    }

                    GravityBullet gravityBulletScript;

                    if (gunList[selectedGun].name == "GravityStaff")
                    {
                        gravityBulletScript = bulletClone.GetComponent<GravityBullet>();
                        if (gravityBulletScript != null)
                        {
                            gravityBulletScript.damage = shootDamage;
                            gravityBulletScript.maxTravelDistance = gunList[selectedGun].shootingDist;
                        }
                    }
                }
                yield return new WaitForSeconds(fireRate);
                isShooting = false;
            }
        }
       
    }
    public void gunPick(GunLists gunStat)
    {
        ApplyGunTransform(gunStat);
        gunList.Add(gunStat);

        // Set the initial selected gun to the first one in the list.
        selectedGun = 0;
        UpdateGunStats(gunStat);
        SetGunModel(gunStat);
        // Store the original position
        originalPosition = gunModel.transform.localPosition;
    }
    void selectGun()
    {
        // If there are no weapons in the gunList or switching is in progress, return
        if (gunList.Count == 0 || changing || isSelecting)
        {
            return;
        }

        int newSelectedGun = selectedGun;

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            newSelectedGun += 1;
            if (newSelectedGun >= gunList.Count)
            {
                newSelectedGun = 0;
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            newSelectedGun -= 1;
            if (newSelectedGun < 0)
            {
                newSelectedGun = gunList.Count - 1;
            }
        }
        else
        {
            // Return early if there's no change in the scroll wheel input
            return;
        }

        if (newSelectedGun != selectedGun)
        {
            StartCoroutine(SwitchWeaponWithDelay(newSelectedGun - selectedGun));
        }
    }
    IEnumerator SwitchWeaponWithDelay(int direction)
    {
        if (direction == 0)
        {
            // Invalid direction, exit the coroutine
            yield break;
        }

        isSelecting = true;
        int prevSelectedGun = selectedGun;
        int weaponCount = gunList.Count;
        // Calculate the new selectedGun index
        int newSelectedGun = (selectedGun + direction + weaponCount) % weaponCount;
        yield return new WaitForSeconds(weaponSwitchDelay);
        if (newSelectedGun != prevSelectedGun)
        {
            selectedGun = newSelectedGun;
            changing = true;
            changeGun();
        }

        isSelecting = false;
    }
    IEnumerator AnimateWeaponChange(Transform target, Vector3 startPos, Quaternion startRot, Vector3 endPos, Quaternion endRot, float duration, GunLists gunStat)
    {
        
        float elapsedTime = 0;
        float halfDuration = duration * 0.5f;

        // First half: move the current weapon out of view
        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / halfDuration;

            target.localPosition = Vector3.Lerp(startPos, startPos + Vector3.down, t);
            target.localRotation = Quaternion.Slerp(startRot, Quaternion.Euler(startRot.eulerAngles + new Vector3(45, 0, 0)), t);

            yield return null;
        }

        // Update the gun model at the halfway point
        target.localPosition = startPos + Vector3.down;
        target.localRotation = Quaternion.Euler(startRot.eulerAngles + new Vector3(45, 0, 0));
        SetGunModel(gunStat);
        ApplyGunTransform(gunList[selectedGun]);

        elapsedTime = 0;

        // Second half: move the new weapon into view
        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / halfDuration;

            target.localPosition = Vector3.Lerp(startPos + Vector3.down, endPos, t);
            target.localRotation = Quaternion.Slerp(Quaternion.Euler(startRot.eulerAngles + new Vector3(45, 0, 0)), endRot, t);

            yield return null;
        }

        target.localPosition = endPos;
        target.localRotation = endRot;
        changing = false;
       
    }

    void changeGun()
    {
        GunLists gunStat = gunList[selectedGun];
        UpdateGunStats(gunStat);
        

        Vector3 startPos = gunModel.transform.localPosition;
        Quaternion startRot = gunModel.transform.localRotation;

        Vector3 endPos;
        Quaternion endRot;
        GetGunTransform(gunStat, out endPos, out endRot);
        
            StartCoroutine(AnimateWeaponChange(gunModel.transform, startPos, startRot, endPos, endRot, 1f, gunStat));
        

    }
    private void GetGunTransform(GunLists gunStat, out Vector3 endPos, out Quaternion endRot)
    {
        if (gunStat.name == "FlameStaff" || gunStat.name == "GravityStaff" || gunStat.name == "IceStaff" || gunStat.name == "RapidFireStaff")
        {
            endPos = new Vector3(0.35f, -0.61f, 0.337f);
            endRot = Quaternion.Euler(new Vector3(-45, 0, 0));
        }
        else
        {
            endPos = Vector3.zero;
            endRot = Quaternion.identity;
        }
    }
    private void UpdateGunStats(GunLists gunStat)
{
        if (gunStat.name == "FlameStaff")
        {
            gravityFx.SetActive(false);
            rapidFx.SetActive(false);
            iceFx.SetActive(false);
            fireFx.SetActive(true);
        }
        if (gunStat.name == "GravityStaff")
        {
            gravityFx.SetActive(true);
            rapidFx.SetActive(false);
            iceFx.SetActive(false);
            fireFx.SetActive(false);
        }
        if (gunStat.name == "IceStaff")
        {
            gravityFx.SetActive(false);
            rapidFx.SetActive(false);
            iceFx.SetActive(true);
            fireFx.SetActive(false);
        }
        if (gunStat.name == "RapidFireStaff")
        {
            gravityFx.SetActive(false);
            rapidFx.SetActive(true);
            iceFx.SetActive(false);
            fireFx.SetActive(false);
        }

        bullet = gunStat.gunBullet;
        shootDamage = gunStat.shootingDamage;
    shootDist = gunStat.shootingDist;
    fireRate = gunStat.shootingRate;
        recoilDirection = gunStat.recoilDirection;
        recoilAmount = gunStat.recoilAmount;
    }

private void SetGunModel(GunLists gunStat)
{
    gunModel.mesh = gunStat.gunModel.GetComponent<MeshFilter>().sharedMesh;

    MeshRenderer gunRenderer = gunStat.gunModel.GetComponent<MeshRenderer>();
    Material[] gunMaterials;
    if (gunStat.name == "FlameStaff" || gunStat.name == "GravityStaff" || gunStat.name == "IceStaff" || gunStat.name == "RapidFireStaff")
    {
        gunMaterials = gunRenderer.gameObject.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().sharedMaterials;
    }
    else
    {
        gunMaterials = gunRenderer.sharedMaterials;
    }
    gunMaterial.materials = gunMaterials;
}

    private void ApplyGunTransform(GunLists gunStat)
    {
        Vector3 endPos;
        Quaternion endRot;
        GetGunTransform(gunStat, out endPos, out endRot);

        gunModel.gameObject.transform.localPosition = endPos;
        gunModel.gameObject.transform.localRotation = endRot;

        // Update the original position
        originalPosition = endPos;

        if (gunStat.name == "FlameStaff" || gunStat.name == "GravityStaff" || gunStat.name == "IceStaff" || gunStat.name == "RapidFireStaff")
        {
            gunModel.gameObject.transform.localScale = new Vector3(40, 40, 40);
            shootPos.transform.localPosition = new Vector3(0.366f, -0.075f, 1.361f);
        }
        else
        {
            gunModel.gameObject.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        }
    }
    //When the player hit the floor the player will be set on fire
    private Coroutine flashCoroutine; // Reference to the flashing coroutine

    private IEnumerator FlashStunnedObject()
    {
        float totalTime = 2.5f; // Total time for the flashing effect
        float flashInterval = 0.5f; // Interval between each flash

        float elapsedTime = 0f;
        bool isVisible = true;

        while (elapsedTime < totalTime)
        {
            stunned.SetActive(isVisible);
            yield return new WaitForSeconds(flashInterval);
            isVisible = !isVisible;
            elapsedTime += flashInterval;
        }

        stunned.SetActive(false); // Ensure the object is deactivated at the end
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Floor"))
        {
            fireDamage.SetActive(true);
            flashCoroutine = StartCoroutine(FlashStunnedObject());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Floor"))
        {
            fireDamage.SetActive(false);
            if (flashCoroutine != null)
            {
                StopCoroutine(flashCoroutine);
                flashCoroutine = null;
            }
            stunned.SetActive(false);
        }
    }
}