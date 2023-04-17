/*=========================================================
	PARTICLE PRO FX volume one 
	PPFXMeteor.cs

	
	(c) 2014
=========================================================*/

using UnityEngine;
using System.Collections;

public class PPFXMeteor : MonoBehaviour {
	
	Vector3 groundPos = new Vector3(0,0,0);
	public Vector3 spawnPosOffset = new Vector3(0,0,0);
	
	public float speed = 10f;
	public GameObject detonationPrefab;
	
	public bool destroyOnHit;
	public bool setRateToNull;
	
	float dist = 0f;
	float radius = 2f;
	
	ParticleSystem[] psystems;
}
