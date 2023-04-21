using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunsPickUp : MonoBehaviour
{
    [SerializeField] GunLists gun;
    [SerializeField] MeshFilter model;
    [SerializeField] MeshRenderer material;

    private void Start()
    {
        model.mesh = gun.gunModel.GetComponent<MeshFilter>().sharedMesh;
        material.material = gun.gunModel.GetComponent<MeshRenderer>().sharedMaterial;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.instance.playerScript.gunPick(gun);

            Destroy(gameObject);
        }
    }
}
