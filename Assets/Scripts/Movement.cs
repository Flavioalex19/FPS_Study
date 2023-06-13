using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class Movement : MonoBehaviour
{
    #region Movement Variables
    [Header("Movement Variables")]
    [SerializeField] float _movementSpeed = 5f;
    [SerializeField] float _mouseSensitivity = 2f;
    #endregion

    
    #region Dash Variables
    [Header("Dash Variables")]
    [SerializeField] float _dashDuration = 0.5f;
    [SerializeField] float _dashSpeed = 10f;
    private float _dashTimer = 0f;
    private bool _isDashing = false;
    bool _isUpperDashing = false;
    #endregion

    #region Bubble Head
    [Header("Bubble Head")]
    [SerializeField] float bubbleHeadSpeed = 1f;
    [SerializeField] float maxBubbleHeadIntensity = 1f;
    [SerializeField] float bubbleHeadHeight = 0.25f;
    [SerializeField] float bubbleHeadVerticalSpeed = 1f;
    [SerializeField] float bubbleHeadVerticalRange = 0.5f;
    #endregion

    #region Jump Variables
    [Header("Jump Variables")]
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] float _gravityMultiplier = 2f;
    private bool _isJumping = false;
    bool _canUpperDash = true;
    float _verticalVelocity = 0f;
    #endregion

    #region UpperDash Variables
    [SerializeField] float _upperDashVelocity = 2.5f;
    [SerializeField] bool _canGroundPound = false;
    #endregion

    [Header("Ground Pound")]
    [SerializeField] ParticleSystem _vfx_groundpound;
    [SerializeField] AudioSource _sfx_groundpound;

    [Header("Camera Shake Variables")]
    #region Camera Variables
    [SerializeField] float _shakeDuration = 0.5f;  // Duration of the camera shake
    [SerializeField] float _shakeIntensity = 0.1f;  // Intensity of the camera shake
    float _verticalRotation = 0f;
    Vector3 _originalCameraPosition;
    Transform _cameraTransform;
    
    #endregion

    #region Components
    CharacterController controller;
    Camera playerCamera;
    [SerializeField] CameraShake _cameraShake;

    #endregion

    private void Awake()
    {
        _cameraTransform = Camera.main.transform;
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _originalCameraPosition = playerCamera.transform.localPosition;

        //_vfx_groundpound.gameObject.SetActive(false);
    }

    void Update()
    {
        // Player movement
        float horizontalMovement = Input.GetAxis("Horizontal") * _movementSpeed;
        float verticalMovement = Input.GetAxis("Vertical") * _movementSpeed;

        Vector3 movement = transform.forward * verticalMovement + transform.right * horizontalMovement;

        if (_isDashing)
        {
            controller.Move(movement * _dashSpeed * Time.deltaTime);

            _dashTimer -= Time.deltaTime;
            if (_dashTimer <= 0f)
            {
                _isDashing = false;
            }
        }
        else
        {
            controller.SimpleMove(movement);
        }


        // Calculate movement intensity
        float movementIntensity = Mathf.Clamp01(movement.magnitude / _movementSpeed);

        // Player rotation
        float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity;

        _verticalRotation -= mouseY;
        _verticalRotation = Mathf.Clamp(_verticalRotation, -90f, 90f);

        playerCamera.transform.localRotation = Quaternion.Euler(_verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // Bubble head effect
        //float bubbleHeadX = Mathf.Sin(Time.time * bubbleHeadSpeed) * movementIntensity * maxBubbleHeadIntensity;
        //float bubbleHeadY = Mathf.Cos(Time.time * bubbleHeadSpeed) * movementIntensity * maxBubbleHeadIntensity;

        //float bubbleHeadVerticalOffset = Mathf.Sin(Time.time * bubbleHeadVerticalSpeed) * bubbleHeadVerticalRange;
        //Vector3 bubbleHeadOffset = new Vector3(bubbleHeadX, bubbleHeadY, bubbleHeadVerticalOffset) * bubbleHeadHeight;
        playerCamera.transform.localPosition = _originalCameraPosition; //*+ bubbleHeadOffset*/;

        // Apply horizontal movement
        movement = transform.forward * verticalMovement + transform.right * horizontalMovement;
        movement.y = _verticalVelocity;
        controller.Move(movement * Time.deltaTime);

        

        // Reset _isJumping flag
        if (controller.isGrounded)
        {
            if (_isJumping)
            {
                //AOI for the ground pound attack---TODO Change this to another script
                if (_canGroundPound)
                {
                    GroundPoundAOI();
                    ShakeCamera();
                    _canGroundPound = false;
                }
            }
            _isJumping = false;
            _canUpperDash = true;

            //Call the camera shake
            if (controller.velocity.magnitude > .5f && _isJumping == false)
            {
                _cameraShake.ShakeCamera();
            }
            else
            {
                _cameraShake.StopCameraShake();
            }


        }
        

    }
    public void ShakeCamera()
    {
        _originalCameraPosition = _cameraTransform.localPosition;  // Store the original position of the camera

        // Start the coroutine to shake the camera
        StartCoroutine(ShakeCoroutine());
    }

    private IEnumerator ShakeCoroutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < _shakeDuration)
        {
            // Generate a random offset for the camera position
            Vector3 randomOffset = Random.insideUnitSphere * _shakeIntensity;

            // Apply the random offset to the camera's local position
            _cameraTransform.localPosition = _originalCameraPosition + randomOffset;

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // Reset the camera position to the original position after the shake
        _cameraTransform.localPosition = _originalCameraPosition;
    }
    #region Jump Functions
    public void Jump()
    {
        if (controller.isGrounded)
        {
            _verticalVelocity = -0.5f; // Reset vertical velocity
            if (Input.GetButtonDown("Jump"))
            {
                //_canGroundPound = true;
                _isJumping = true;
                _verticalVelocity = _jumpForce;
            }
        }
        else
        {
            
            _verticalVelocity += Physics.gravity.y * Time.deltaTime * _gravityMultiplier;//Gravity
        }
    }
    #region Groundpound
    public void GroundPound()
    {
        if (_isJumping)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                _canGroundPound = true;
                _verticalVelocity = -_jumpForce * 2;
            }
        }
    }
    void GroundPoundAOI()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 10f);
        foreach (var hitCollider in hitColliders)
        {
            
            if(hitCollider.tag == "Enemy") hitCollider.GetComponent<Enemy>().TakeDamage(180f);
            VfxGroundPound();
        }
    }
    void VfxGroundPound()
    {
        /*
        _vfx_groundpound.gameObject.SetActive(true);
        ParticleSystem.MainModule mainModule = _vfx_groundpound.main;
        mainModule.stopAction = ParticleSystemStopAction.None;
        _vfx_groundpound.Play();
        */
        ParticleSystem groundPound = Instantiate(_vfx_groundpound, transform.position, Quaternion.identity);
        groundPound.Play();
    }
    void SFXGroudnpound()
    {
        AudioSource audioSource = Instantiate(_sfx_groundpound, _sfx_groundpound.transform.position, Quaternion.identity);
        audioSource.Play();
    }
    #endregion
    public void UpperDash()
    {
        if (_isJumping)
        {
            if (_canUpperDash)
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    _canUpperDash = false;
                    _verticalVelocity = _jumpForce * _upperDashVelocity;
                }
            }
            
        }
    }
    #endregion

    #region Get & Set
    public bool GetIsDashing()
    {
        return _isDashing;
    }
    public void SetIsDashing(bool isDashing)
    {
        _isDashing = isDashing;
    }
    public float GetDashDuration()
    {
        return _dashDuration;
    }
    public void SetDashDuration(float dashDuration)
    {
        _dashDuration = dashDuration;
    }
    public float GetDashTimer()
    {
        return _dashTimer;
    }
    public void SetDashTimer(float dashTimer)
    {
        _dashTimer = dashTimer;
    }
    public bool GetIsJumping()
    {
        return _isJumping;
    }
    #endregion
}
