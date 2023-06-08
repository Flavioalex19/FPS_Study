using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{

    Movement _movement;
    Shooting _shooting;
    HitScanShooting _hitScanShooting;
    Weapon _weapon;
    CharacterController _characterController;

    // Start is called before the first frame update
    void Start()
    {
        _movement = GetComponent<Movement>();
        _shooting = GameObject.FindGameObjectWithTag("Weapon").GetComponent<Shooting>();
        _hitScanShooting = GameObject.FindGameObjectWithTag("Weapon").GetComponent<HitScanShooting>();
        _weapon = GameObject.FindGameObjectWithTag("Weapon").GetComponent<Weapon>();
        _characterController = GetComponent<CharacterController>();

        /*
        //Starts with the minigun enable
        _shooting.enabled = false;
        _hitScanShooting.enabled = true;
        */
        
    }

    // Update is called once per frame
    void Update()
    {

        _movement.Jump();
        _movement.GroundPound();
        _movement.UpperDash();

        #region Shooting
        //Change the Fire Mode
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            /*
            _shooting.enabled = !_shooting.enabled;
            _hitScanShooting.enabled = !_hitScanShooting.enabled;
            */
            _weapon.ChangeFireMode();

        }

        if (Input.GetButtonDown("Fire1")|| Input.GetButton("Fire1"))
        {
            /*
            //If the Semi-auto/hand cannon is enable
            if (_shooting.enabled) 
            {
                if(_shooting.GetCanShoot())
                _shooting.Shoot();
            } 
            else if(_hitScanShooting.enabled)
            {
                _hitScanShooting.Shoot();
                _hitScanShooting.SetFireTimer(0f);
            }
            */
            
            if (_weapon.GetAlternateFireMode())
            {
                _weapon.GranadeLauncher();
            }
            else if (!_weapon.GetAlternateFireMode())
            {
                _weapon.HitScanFire();
            }
            
        }
        
        #endregion

        #region Dash input
        if (Input.GetKeyDown(KeyCode.LeftShift) && _characterController.isGrounded)
        {
            if (!_movement.GetIsDashing())
            {
                _movement.SetIsDashing(true);
                _movement.SetDashTimer(_movement.GetDashDuration()); 
            }
        }
        #endregion
    }
}
