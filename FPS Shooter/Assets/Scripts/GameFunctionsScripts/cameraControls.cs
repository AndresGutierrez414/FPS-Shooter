using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraControls : MonoBehaviour
{
    // variables //
    [SerializeField] int sensHor;
    [SerializeField] int sensVer;

    [SerializeField] int lockVerMin;
    [SerializeField] int lockVerMax;

    [SerializeField] bool invertY;

    float xRotation;

    void Start()
    {
        // turn cursor off and lock to screen //
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // get input //
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensVer;
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensHor;

        // convert input to rotation float and give option for inverted controls //
        if(invertY )
        xRotation += mouseY;
        else 
        xRotation -= mouseY;

        // clamp camera rotation //
        xRotation = Mathf.Clamp(xRotation, lockVerMin, lockVerMax);

        // rotate the camera on the X-axis //
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0); // localRotation -> based on character and not global 

        // rotate player along Y-axis //
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}
