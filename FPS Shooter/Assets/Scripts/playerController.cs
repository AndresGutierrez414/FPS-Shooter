using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage
{
    //Componets and variables//
    [Header("----- Components -----")]
    [SerializeField] private CharacterController controller;

    [Header("----- Player Stats -----")]                        //Player current hit points and maximum hit points
    [Range(1, 10)][SerializeField] int HP;
    int maxHP;

    [Range(3, 8)][SerializeField] float playerSpeed;            //Player movement speed and current velocity
    Vector3 playerVelocity;
    Vector3 movementVec;

    [Range(10, 50)][SerializeField] float gravityValue;         //Player jump statistics and gravity value
    [Range(8, 25)][SerializeField] float jumpHeight;
    [Range(1, 3)][SerializeField] int maxJumps;
    int timesJumped;
    bool isPlayerGrounded;

    [Header("----- Gun Stats -----")]                           //Weapon statistics 
    [Range(1, 10)][SerializeField] int shootDamage;
    [Range(0.1f, 5f)][SerializeField] float fireRate;
    [Range(0.8f, 2f)][SerializeField] float pillowShootRate;
    [Range(1, 100)][SerializeField] int shootDist;
    [Range(1, 4)][SerializeField] int pillowShootDist;
    [SerializeField] GameObject cube;                           //Pillow object
    bool isShooting;
    bool isPlacingP;

    private void Start()
    {
        maxHP = HP;
        playerUIUpdate();
    }

    void Update()
    {
        if (gameManager.instance.activeMenu == null)
        {
            Movement();

            if (!isShooting && Input.GetButton("Fire1"))        //Check for mouse 1 press
                StartCoroutine(shoot());

            if(!isPlacingP && Input.GetButton("Fire2"))         //Check for mouse 2 press
                StartCoroutine(placePillow());

        }

    }

    //Player movement handler
    void Movement()
    {
        isPlayerGrounded = controller.isGrounded;               //Reset player.y velocity and times jumped if the player is grounded
        if (isPlayerGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            timesJumped = 0;
        }

        movementVec = (transform.right * Input.GetAxis("Horizontal")) + (transform.forward * Input.GetAxis("Vertical"));
        controller.Move(movementVec * Time.deltaTime * playerSpeed); //Applies user input to the controller

        if (Input.GetButtonDown("Jump") && timesJumped < maxJumps)  //Check for space bar press. Handles max amounts of jumps
        {
            timesJumped++;
            playerVelocity.y = jumpHeight;
        }

        playerVelocity.y -= gravityValue * Time.deltaTime;      //Applies gravity (and jump if there has been one) to the controller
        controller.Move(playerVelocity * Time.deltaTime);       
    }

    //Called whenever the player takes damage
    public void takeDamage(int amount)
    {
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

    //This coroutine handles throwing pillows
    IEnumerator placePillow()
    {
        isPlacingP = true;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, pillowShootDist))
        {
            Instantiate(cube, hit.point, transform.rotation);   //Instantiates cube at the hit location
        }

        yield return new WaitForSeconds(pillowShootRate);
        isPlacingP = false;
    }

}