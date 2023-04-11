using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class enemyHealthBar : MonoBehaviour
{
    // variables //
    private enemyAI enemy;
    public Image healthBarFill;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        enemy = GetComponent<enemyAI>();
    }

    private void Update()
    {
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
           mainCamera.transform.rotation * Vector3.up);
    }

    public void updateHealthBar()
    {
        float healthPercentage = (float)enemy.HP / (float)enemy.MaxHP;
        healthBarFill.fillAmount = healthPercentage;
    }
}
