using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraShake : MonoBehaviour
{
    // variables //
    private Vector3 originalPosition;
    private bool isShaking;


    public void shake(float _duration, float _magnitude)
    {
        if (!isShaking)
        {
            originalPosition = transform.localPosition;
            StartCoroutine(shakeCamera(_duration, _magnitude));
        }
    }

    private IEnumerator shakeCamera(float _duration, float _magnitude)
    {
        isShaking = true;
        float elapsedTime = 0f;

        while (elapsedTime < _duration)
        {
            float x = Random.Range(-_magnitude, _magnitude);
            float y = Random.Range(-_magnitude, _magnitude);

            transform.localPosition = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
        isShaking = false;
    }
}
