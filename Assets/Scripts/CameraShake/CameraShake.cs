using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] float _shakeMagnitude = 0.1f;  // Magnitude of the camera shake
    [SerializeField] float _shakeSpeed;  // Speed of the camera shake

    private Vector3 _originalCameraPosition;
    private bool _isShaking = false;

    private void Start()
    {
        _originalCameraPosition = transform.localPosition;
    }

    public void ShakeCamera()
    {
        if (!_isShaking)
        {
            StartCoroutine(ShakeCoroutine());
        }
    }

    IEnumerator ShakeCoroutine()
    {
        float elapsed = 0f;
        float offsetY = 0f;

        _isShaking = true;

        while (elapsed < _shakeSpeed)
        {
            // Calculate the vertical shake offset using a sine wave
            offsetY = Mathf.Sin(elapsed * _shakeSpeed) * _shakeMagnitude;

            // Apply the shake offset to the camera position
            transform.localPosition = _originalCameraPosition + new Vector3(0f, offsetY, 0f);

            elapsed += Time.deltaTime * .5f;

            yield return null;
        }

        // Reset the camera position to the original position
        transform.localPosition = _originalCameraPosition;

        // Pause between waves
        yield return new WaitForSeconds(5f);

        _isShaking = false;
    }
    public void StopCameraShake()
    {
        StopAllCoroutines();
        transform.localPosition = _originalCameraPosition;
        _isShaking = false;
    }
}
