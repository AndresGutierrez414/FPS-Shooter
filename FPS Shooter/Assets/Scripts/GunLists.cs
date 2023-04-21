using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GunLists : ScriptableObject
{
    public int shootingDamage;
    public float shootingRate;
    public float shootingDist;

    public GameObject gunModel;
    public AudioClip gunBlastAudio;
}
