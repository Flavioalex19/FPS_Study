using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public float bulletSpeed = 10f;

    public float fireRate = 0.5f;

    private float fireTimer = 0f;
    private bool canShoot = true;

    [SerializeField] ParticleSystem _muzzleFlash;

    private void Start()
    {
        _muzzleFlash.gameObject.SetActive(false);
    }

    private void Update()
    {
        fireTimer += Time.deltaTime;

        if (fireTimer >= fireRate)
        {
            canShoot = true;
        }

    }

    public void Shoot()
    {
        /*
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
        bulletRigidbody.velocity = bulletSpawnPoint.forward * bulletSpeed;
        */
        Muzzle();
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
        bulletRigidbody.velocity = bulletSpawnPoint.forward * bulletSpeed;

        fireTimer = 0f;
        canShoot = false;
    }
    void Muzzle()
    {
        _muzzleFlash.gameObject.SetActive(true);
        ParticleSystem.MainModule mainModule = _muzzleFlash.main;
        mainModule.stopAction = ParticleSystemStopAction.None;
        _muzzleFlash.Play();
    }
    #region Get & Set
    public bool GetCanShoot()
    {
        return canShoot;
    }
    #endregion
}
