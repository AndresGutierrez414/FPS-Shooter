using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage
{
    // variables //
    [Header("----- Components -----")]
    [SerializeField] private CharacterController controller;


    [Header("----- Player Stats -----")]
    [Range(1, 10)][SerializeField] int HP;
    [Range(3, 8)][SerializeField] float playerSpeed;
    [Range(8, 25)][SerializeField] float jumpHeight;
    [Range(10, 50)][SerializeField] float gravityValue;
    [Range(1, 3)][SerializeField] int jumpMax;
    [SerializeField] GameObject cube;


    [Header("----- Gun Stats -----")]
    [Range(1, 10)][SerializeField] int shootDamage;
    [Range(0.1f, 5f)][SerializeField] float shootRate;
    [Range(0.8f, 2f)] [SerializeField] float pillowShootRate;
    [Range(1, 100)][SerializeField] int shootDist;
    [Range(1, 4)] [SerializeField] int pillowShootDist;

    int jumpedTimes;
    private Vector3 playerVelocity;
    private bool groundedPlayer;

    bool isShooting;
    bool isPlacingP;
    Vector3 move;
    int HPOrig;

    private void Start()
    {
        HPOrig = HP;
        playerUIUpdate();
    }

    void Update()
    {
        if (gameManager.instance.activeMenu == null)
        {
            Movement();

            if (!isShooting && Input.GetButton("Shoot"))
                StartCoroutine(shoot());

            if(!isPlacingP && Input.GetButton("PlacePillow"))
                StartCoroutine(placePillow());

        }

    }

    void Movement()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            jumpedTimes = 0;
        }

        move = (transform.right * Input.GetAxis("Horizontal")) +
               (transform.forward * Input.GetAxis("Vertical"));
        controller.Move(move * Time.deltaTime * playerSpeed);

        if (Input.GetButtonDown("Jump") && jumpedTimes < jumpMax)
        {
            jumpedTimes++;
            playerVelocity.y = jumpHeight;
        }

        playerVelocity.y -= gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    IEnumerator shoot()
    {
        isShooting = true;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDist))
        {
            IDamage damagable = hit.collider.GetComponent<IDamage>();

            if (damagable != null)
            {
                damagable.takeDamage(shootDamage);
            }
        }

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        playerUIUpdate();

        if (HP <= 0)
        {
            gameManager.instance.playerDead();
        }
    }

    void playerUIUpdate()
    {
        gameManager.instance.HPBar.fillAmount = (float)HP / (float)HPOrig;
    }

    IEnumerator placePillow()
    {

        isPlacingP = true;
        RaycastHit hit2;
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit2, pillowShootDist))
        {
            Instantiate(cube, hit2.point, transform.rotation);
        }

            yield return new WaitForSeconds(pillowShootRate);

        isPlacingP = false;
    }

}
