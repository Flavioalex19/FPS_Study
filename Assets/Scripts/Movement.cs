using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private float _jumpForce = 5f;
    private bool _isJumping = false;
    bool _canUpperDash = true;
    float _verticalVelocity = 0f;
    #endregion

    #region Camera Variables
    private float verticalRotation = 0f;
    private Vector3 originalCameraPosition;
    #endregion

    #region Components
    private CharacterController controller;
    private Camera playerCamera;
    #endregion
    

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        originalCameraPosition = playerCamera.transform.localPosition;
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

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // Bubble head effect
        float bubbleHeadX = Mathf.Sin(Time.time * bubbleHeadSpeed) * movementIntensity * maxBubbleHeadIntensity;
        float bubbleHeadY = Mathf.Cos(Time.time * bubbleHeadSpeed) * movementIntensity * maxBubbleHeadIntensity;

        float bubbleHeadVerticalOffset = Mathf.Sin(Time.time * bubbleHeadVerticalSpeed) * bubbleHeadVerticalRange;
        Vector3 bubbleHeadOffset = new Vector3(bubbleHeadX, bubbleHeadY, bubbleHeadVerticalOffset) * bubbleHeadHeight;
        playerCamera.transform.localPosition = originalCameraPosition + bubbleHeadOffset;

        /*
        if(Input.GetKeyDown(KeyCode.Space)) _isUpperDashing = true;
        if (_isUpperDashing)
        {
            // Apply vertical velocity
            Vector3 jumpVector = new Vector3(0f, _dashSpeed, 0f);
            controller.Move(jumpVector * Time.deltaTime);
        }
        */
        // Jump input
        

        // Apply horizontal movement
        movement = transform.forward * verticalMovement + transform.right * horizontalMovement;
        movement.y = _verticalVelocity;
        controller.Move(movement * Time.deltaTime);

        // Reset _isJumping flag
        if (controller.isGrounded)
        {
            _isJumping = false;
            _canUpperDash = true;
        }

    }
    public void Jump()
    {
        if (controller.isGrounded)
        {
            _verticalVelocity = -0.5f; // Reset vertical velocity
            if (Input.GetButtonDown("Jump"))
            {
                _isJumping = true;
                _verticalVelocity = _jumpForce;
            }
        }
        else
        {
            _verticalVelocity += Physics.gravity.y * Time.deltaTime * 2;//Gravity
        }
    }
    public void GroundPound()
    {
        if (_isJumping)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                _verticalVelocity = -_jumpForce;
            }
        }
    }
    public void UpperDash()
    {
        if (_isJumping)
        {
            if (_canUpperDash)
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    _canUpperDash = false;
                    _verticalVelocity = _jumpForce * 3f;
                }
            }
            
        }
    }

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
