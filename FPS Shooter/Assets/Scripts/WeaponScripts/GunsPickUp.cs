using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunsPickUp : MonoBehaviour
{
    [SerializeField] GunLists gun;
    [SerializeField] MeshFilter model;
    [SerializeField] MeshRenderer mat;
    [SerializeField] AudioClip pickupGunSound;
    //public AudioSource audio;

    private void Start()
    {
        model.mesh = gun.gunModel.GetComponent<MeshFilter>().sharedMesh;
        mat.material = gun.gunModel.GetComponent<MeshRenderer>().sharedMaterial;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.instance.playerScript.gunPick(gun);

            AudioSource.PlayClipAtPoint(pickupGunSound, other.transform.position);
            //audio.Play();

            Destroy(gameObject);
        }
    }
}
