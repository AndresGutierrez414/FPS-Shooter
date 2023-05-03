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
    [SerializeField] public bool enableIntroSequence;
    [SerializeField] Transform endPosition;
    [SerializeField] Transform[] focusPoints;
    [SerializeField] Transform playerStartPosition;
    [SerializeField] float introMoveSpeed;
    [SerializeField] float halfCircleMoveSpeed;
    [SerializeField] float introRotationSpeed;
    [SerializeField] float waitTimeAtFocusPoints;
    [SerializeField] float distanceFromFocusPoint;
    bool introFinsished = false;
    public bool isSkippingIntro = false;

    // camera original start pos //
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    public playerController player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<playerController>();

        // turn cursor off and lock to screen //
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // if intro is going on //
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
            player.canMove = true;
        }
    }

    void Update()
    {
        if (!introFinsished)
        {
            CheckForSkipIntro();
            return;
        }


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
        yield return new WaitForSeconds(1);
        if (!isSkippingIntro) yield return StartCoroutine(moveAndLookAt(endPosition.position, endPosition.position));

        // iterate through focus points in array //
        int focusPointIndex = 0;
        while (focusPointIndex < focusPoints.Length && !isSkippingIntro)
        {
            Transform focusPoint = focusPoints[focusPointIndex];

            // move to next focus point and look at it //
            if (!isSkippingIntro) yield return StartCoroutine(moveAndLookAt(focusPoint.position, focusPoint.position));

            // if skip intro is pressed, break the loop //
            if (isSkippingIntro) break;

            // wait before going to the next point //
            yield return new WaitForSeconds(waitTimeAtFocusPoints);

            focusPointIndex++;
        }

        // move to player start pos and look at it //
        if (!isSkippingIntro) yield return StartCoroutine(moveAndLookAt(transform.parent.position, playerStartPosition.position));

        // reset camera pos and rotation to original //
        if (!isSkippingIntro)
        {
            transform.position = new Vector3(originalPosition.x, originalPosition.y + 0.5f, originalPosition.z);
            transform.rotation = originalRotation;
        }

        // intro done //
        introFinsished = true;
        player.canMove = true;
    }

    IEnumerator moveAndLookAt(Vector3 targetPosition, Vector3 lookAtPosition)
    {
        // Add a small offset to the target position to avoid zero vector error //
        targetPosition += new Vector3(0.0001f, 0.0001f, 0.0001f);

        // calculate start pos of half circle movement //
        Vector3 startCirclePosition = targetPosition + (transform.position - targetPosition).normalized * distanceFromFocusPoint;

        // Move camera to the starting position of the half-circle movement //
        while (Vector3.Distance(transform.position, startCirclePosition) > 0.1f && !isSkippingIntro)
        {
            // move camera to start position //
            transform.position = Vector3.Lerp(transform.position, startCirclePosition, introMoveSpeed * Time.deltaTime);

            // calculate and rotate camera around each point //
            Quaternion targetRotation = Quaternion.LookRotation(lookAtPosition - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, introRotationSpeed * Time.deltaTime);

            // wait for next frame //
            yield return null;
        }

        // Calculate the center of the circle and the radius //
        Vector3 circleCenter = targetPosition;
        float radius = distanceFromFocusPoint;

        // initialize angle for half circle movement //
        float angle = 0;
        float targetAngle = 180f;

        // perform half circle movement //
        while (angle < targetAngle && !isSkippingIntro)
        {
            // Calculate the angle increment based on the half-circle move speed //
            float step = halfCircleMoveSpeed * Time.deltaTime;
            angle += step;

            // Calculate the position on the half-circle path using trigonometry //
            float xPos = circleCenter.x + radius * Mathf.Sin(Mathf.Deg2Rad * angle);
            float yPos = circleCenter.y + 10; // adjust height of half circle rotation
            float zPos = circleCenter.z + radius * Mathf.Cos(Mathf.Deg2Rad * angle);

            // update camera pos and rotation //
            transform.position = new Vector3(xPos, yPos, zPos);
            transform.rotation = Quaternion.LookRotation(lookAtPosition - transform.position);

            // wait for next frame //
            yield return null;
        }

    }

    void CheckForSkipIntro()
    {
        if (Input.GetKeyDown(KeyCode.F) && !isSkippingIntro)
        {
            isSkippingIntro = true;
            StopCoroutine(cameraIntroSequence());

            // Set the camera to the final position and rotation
            transform.position = new Vector3(originalPosition.x, originalPosition.y + 0.5f, originalPosition.z);
            transform.rotation = originalRotation;

            // intro done
            introFinsished = true;
            player.canMove = true;
        }
    }

    IEnumerator WaitForSkipIntro()
    {
        while (!isSkippingIntro)
        {
            yield return null;
        }
    }
}
