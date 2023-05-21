using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lavaFloor : MonoBehaviour
{
    // variables //
    [Header("---------- Stat Reduction ----------")]
    [SerializeField] int damage;
    [SerializeField] float damageTimer;
    [SerializeField] int speedReduction;
    [SerializeField] float sprintReduction;
    [SerializeField] float accelerationReduction;
    [SerializeField] float speedRecoveryTimer;

    // orig stats storage 
    private float speedOrig;
    private float sprintOrig;
    private float accelerationOrig;

    // HashSet to store colliders of player in contact with the lava floor //
    public HashSet<Collider> playerInLava = new HashSet<Collider>();

    private Coroutine recoveryCoroutine;


    private bool statsReduced;

    private void Start()
    {
        speedOrig = gameManager.instance.playerScript.playerSpeed;
        sprintOrig = gameManager.instance.playerScript.playerSprint;
        accelerationOrig = gameManager.instance.playerScript.sprintAcceleration;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController player = other.GetComponent<playerController>();

            if (player != null)
            {
                // add the player collider to HashSet //
                playerInLava.Add(other);

               


                // damage and slow player //
                StartCoroutine(damageAndSlowPlayer(player));


                statsReduced = true;
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController player = other.GetComponent<playerController>();

            if (player != null)
            {
                // Remove player collider from HashSet
                playerInLava.Remove(other);

                if (playerInLava.Count == 0) // Check if the player is still in the lava
                {
                    statsReduced = false;
                }
                else
                {
                    // Restart the damageAndSlowPlayer coroutine for the player
                    StartCoroutine(damageAndSlowPlayer(player));
                }
            }
        }
    }

    IEnumerator damageAndSlowPlayer(playerController _player)
    {
        // set player stats to specific reduced values //
        _player.playerSpeed = speedOrig - speedReduction;
        _player.playerSprint = sprintOrig - sprintReduction;
        _player.sprintAcceleration = accelerationOrig - accelerationReduction;

        // while player collider is in HashSet, take damage //
        while (playerInLava.Contains(_player.GetComponent<Collider>()))
        {
            _player.takeDamage(damage);
            yield return new WaitForSeconds(damageTimer);
        }

        // Wait for the speedRecoveryTime duration before restoring the player's speed
        yield return new WaitForSeconds(speedRecoveryTimer);

        resetStats(_player);
    }

    public void resetStats(playerController _player) // maybe
    {
        _player.playerSpeed = speedOrig;
        _player.playerSprint = sprintOrig;
        _player.sprintAcceleration = accelerationOrig;
    }
}
