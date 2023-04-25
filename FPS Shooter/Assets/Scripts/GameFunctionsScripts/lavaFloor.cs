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
                // remove player collider frim HashSet //
                playerInLava.Remove(other);
            }
        }
    }

    IEnumerator damageAndSlowPlayer(playerController _player)
    {
        // if player speed is not reduced, then reduce it //
        if(_player.playerSpeed > speedReduction)
        {
            _player.playerSpeed -= speedReduction;
            _player.playerSprint -= sprintReduction;
            _player.sprintAcceleration -= accelerationReduction;
        }

        // while player collider is in HashSet, take damage //
        while (playerInLava.Contains(_player.GetComponent<Collider>())) 
        {
            _player.takeDamage(damage);
            yield return new WaitForSeconds(damageTimer);
        }

        // Wait for the speedRecoveryTime duration before restoring the player's speed
        yield return new WaitForSeconds(speedRecoveryTimer);

        // return player speed to normal //
        _player.playerSpeed += speedReduction;
        _player.playerSprint += sprintReduction;
        _player.sprintAcceleration += accelerationReduction;
    }

    public void resetStats(playerController _player)
    {
        _player.playerSpeed = speedOrig;
        _player.playerSprint = sprintOrig;
        _player.sprintAcceleration = accelerationOrig;
    }
}
