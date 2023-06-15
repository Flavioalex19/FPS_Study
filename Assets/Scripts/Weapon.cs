using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    #region Rifle Variables
    [Header("Rifle Variables")]
    [SerializeField] float damage = 10f;//Damage
    [SerializeField] float _fireRate = 0.1f;
    [SerializeField] float range = 100f;
    [SerializeField] float _maxOverheatValue = 100;
    [SerializeField] float _overheatMultiplier = 2f;
    public float _currentOverheatValue = 0;
    public bool _hasOverheated = false;
    [SerializeField] LayerMask _hitLayers;//What you can hit/deal damage
    #endregion

    [Header("Granade Launcher Variables")]
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public float bulletSpeed = 10f;
    public bool canShoot = true;

    private float _fireTimer = 0f;

    #region Recoil Variables
    [Header("Recoil Variables")]
    [SerializeField] Transform _weaponTransform;  // Transform of the weapon
    [SerializeField] float recoilForce = 1f;  // Recoil force applied to the weapon
    [SerializeField] float recoilDuration = 0.1f;  // Duration of the recoil

    private Vector3 _originalPosition;
    private float recoilTimer = 0f;
    private bool isRecoiling = false;
    #endregion

    [Header("Muzzle Particle")]
    [SerializeField] ParticleSystem _muzzleFlash;
    [Header("Camera")]
    public Camera PlayerCamera;
    #region SFX
    [SerializeField] AudioSource _sfx_rifle;
    [SerializeField] AudioClip _sfx_rifle_clip;
    [SerializeField] AudioClip _sfx_grenade_clip;
    #endregion
    bool _alternateFire = false;
    private void Awake()
    {
        _originalPosition = _weaponTransform.localPosition;
    }
    // Start is called before the first frame update
    void Start()
    {
        _muzzleFlash.gameObject.SetActive(false);
        //_sfx_rifle.gameObject.SetActive(false);
    }
    private void Update()
    {
        #region Overheat update
        //Enter overheat state
        if (_currentOverheatValue > _maxOverheatValue -2 && !_hasOverheated)
        {
            _hasOverheated = true;
        }
        
        //Initiate cooldown after arrive ate the overheat state
        if (_hasOverheated)
        {
            _currentOverheatValue -= Time.deltaTime;
            
        }
        //Reset the current overheat value
        if (_currentOverheatValue < 0 && _hasOverheated)
        {
            _hasOverheated = false;
            _currentOverheatValue = 0;
        }
        else if(_currentOverheatValue > 0 && !_hasOverheated) _currentOverheatValue -= Time.deltaTime * .5f;


        #endregion

        #region Fire Timer
        _fireTimer += Time.deltaTime;
        if (_alternateFire)
        {
            if (_fireTimer >= _fireRate)
            {
                canShoot = true;
            }
        }
        else
        {
            if (_fireTimer >= _fireRate)
            {
                canShoot = true;
                _fireTimer = 0;
            }
        }
        #endregion
        #region Alternate Fire
        if (_alternateFire)
        {
            _sfx_rifle.clip = _sfx_grenade_clip;
            _sfx_rifle.volume = 1;
            _fireRate = .5f;
        }
        else
        {
            _sfx_rifle.clip = _sfx_rifle_clip;
            _sfx_rifle.volume = .2f;
            _fireRate = .05f;
        }
        #endregion
        #region Recoil
        //Execution of the recoil
        if (isRecoiling)
        {
            // Calculate the interpolation factor based on the recoil timer
            float interpolationFactor = recoilTimer / recoilDuration;

            // Apply recoil forces to move the weapon
            float recoilDistance = recoilForce * interpolationFactor;
            //float recoilVerticalOffset = Random.Range(-recoilDistance, recoilDistance);
            float recoilHorizontalOffset = Random.Range(-recoilDistance * 1.2f, recoilDistance * 1.2f);

            _weaponTransform.localPosition = _originalPosition - new Vector3(recoilHorizontalOffset, /*recoilVerticalOffset*/0, recoilDistance);

            // Update the recoil timer
            recoilTimer += Time.deltaTime;

            // Check if the recoil duration has elapsed
            if (recoilTimer >= recoilDuration)
            {
                StopRecoil();
            }
        }
        #endregion
    }
    public void ChangeFireMode()
    {
        _alternateFire = !_alternateFire;
    }
    #region hitScan/Minigun
    public void HitScanFire()
    {
        if (!canShoot) return;
        _currentOverheatValue += Time.deltaTime * _overheatMultiplier;
        ApplyRecoil();
        Muzzle();
        _sfx_rifle.Play();
        //fireRate = .1f;
        Ray ray = PlayerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range, _hitLayers))
        {
            // Perform hit logic
            Debug.Log("Hit: " + hit.transform.name);
            Enemy enemy = hit.transform.GetComponent<Enemy>();
            enemy.TakeDamage(damage);
            /*
            // Apply damage to the hit object if it has a Health component
            Health health = hit.transform.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }*/
        }
        canShoot = false;
        ResetFireRate(canShoot);
    }
    void ResetFireRate(bool canFire)
    {
        /*
        //yield return new WaitForSeconds(_fireRate);
        float timer = 0;
        while(timer < _fireRate)
        {
            timer += Time.deltaTime;
        }
        canFire = true;*/
        StartCoroutine(ResetFireRateCoroutine(canFire));
    }
    IEnumerator ResetFireRateCoroutine(bool canFire)
    {
        yield return new WaitForSeconds(_fireRate);
        canFire = true;
    }
    #endregion
    public void GranadeLauncher()
    {
        if (canShoot)
        {
            Muzzle();
            _sfx_rifle.Play();
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
            bulletRigidbody.velocity = bulletSpawnPoint.forward * bulletSpeed;

            _fireTimer = 0f;
            canShoot = false;
        }
    }
    //Muzzle
    void Muzzle()
    {
        _muzzleFlash.gameObject.SetActive(true);
        ParticleSystem.MainModule mainModule = _muzzleFlash.main;
        mainModule.stopAction = ParticleSystemStopAction.None;
        _muzzleFlash.Play();
    }
    #region Sfx Rifle
    
    #endregion
    #region Recoil
    //Recoil
    public void ApplyRecoil()
    {
        if (!isRecoiling)
        {
            // Start the recoil effect
            recoilTimer = 0f;
            isRecoiling = true;
        }
    }

    private void StopRecoil()
    {
        // Reset the weapon's position to the original position
        _weaponTransform.localPosition = _originalPosition;

        // Reset recoil variables
        recoilTimer = 0f;
        isRecoiling = false;
    }

    #endregion
    #region Get & Set
    public float GetFireTimer()
    {
        return _fireTimer;
    }
    public void SetFireTimer(float fireTimer)
    {
        _fireTimer = fireTimer;
    }
    public bool GetAlternateFireMode()
    {
        return _alternateFire;
    }
    public bool GetHasOverheated()
    {
        return _hasOverheated;
    }
    public float GetCurrentOverheatValue()
    {
        return _currentOverheatValue;
    }
    public float GetMaxOverheatValue()
    {
        return _maxOverheatValue;
    }
    #endregion
}
