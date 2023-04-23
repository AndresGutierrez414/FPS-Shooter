using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraControls : MonoBehaviour
{
    // variables //
    [Header("----------Camera Movement----------")]
    [SerializeField] int sensHor;
    [SerializeField] int sensVer;
    [SerializeField] int lockVerMin;
    [SerializeField] int lockVerMax;
    [SerializeField] bool invertY;
    float xRotation;

    [Header("----------Intro Sequence----------")]
    [SerializeField] bool enableIntroSequence;
    [SerializeField] Transform endPosition;
    [SerializeField] Transform[] focusPoints;
    [SerializeField] Transform playerStartPosition;

    [SerializeField] float introMoveSpeed;
    [SerializeField] float introRotationSpeed;
    [SerializeField] float waitTimeAtFocusPoints;
    [SerializeField] float distanceFromFocusPoint;
    bool introFinsished = false;

    // camera original start pos //
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    void Start()
    {
        // turn cursor off and lock to screen //
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (enableIntroSequence)
        {
            // store original start position of camera //
            originalPosition = transform.position;
            originalRotation = transform.rotation;

            // start camera intro sequence //
            StartCoroutine(cameraIntroSequence());
        }
        else
        {
            introFinsished = true;
        }
    }

    void Update()
    {
        if (!introFinsished) return;


        // get input //
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensVer;
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensHor;

        // convert input to rotation float and give option for inverted controls //
        if (invertY)
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

    IEnumerator cameraIntroSequence()
    {
        // move to end pos and look at it //
        yield return moveAndLookAt(endPosition.position, endPosition.position);

        foreach (Transform focusPoint in focusPoints)
        {
            yield return moveAndLookAt(focusPoint.position, focusPoint.position);
            yield return new WaitForSeconds(waitTimeAtFocusPoints);
        }

        // move to player start pos and look at it //
        yield return moveAndLookAt(transform.parent.position, playerStartPosition.position);

        // reset camera pos and rotation to original //
        transform.position = originalPosition;
        transform.rotation = originalRotation;

        // intro done //
        introFinsished = true;
    }

    IEnumerator moveAndLookAt(Vector3 targetPosition, Vector3 lookAtPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > distanceFromFocusPoint)
        {
            // move camera //
            transform.position = Vector3.Lerp(transform.position, targetPosition, introMoveSpeed * Time.deltaTime);

            // rotate camera to look at target //
            Quaternion targetRotation = Quaternion.LookRotation(lookAtPosition - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, introRotationSpeed * Time.deltaTime);

            yield return null;
        }
    }
}
