using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GunLists : ScriptableObject
{
  
    public Vector3 recoilDirection;
    public int shootingDamage;
    public float shootingRate;
    public int shootingDist;
    public int bulletCount;
    public float spreadAngle;
    public float recoilAmount;
    public GameObject gunModel;
    public GameObject gunBullet;
    public AudioClip gunBlastAudio;

    [Range(0, 1)] public float gunShotAudioVolume;
}
