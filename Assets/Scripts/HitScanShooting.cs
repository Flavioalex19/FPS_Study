using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class HitScanShooting : MonoBehaviour
{

    [SerializeField] float damage = 10f;
    [SerializeField] float fireRate = 0.1f;
    [SerializeField] float range = 100f;
    [SerializeField] LayerMask _hitLayers;
    [SerializeField] ParticleSystem _muzzleFlash;

    public Camera PlayerCamera;
    private float _fireTimer = 0f;

    private void Start()
    {
        _muzzleFlash.gameObject.SetActive(false);
    }
    void Update()
    {
        _fireTimer += Time.deltaTime;

       
    }

    public void Shoot()
    {

        Muzzle();

        Ray ray = PlayerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, range, _hitLayers))
        {
            // Perform hit logic
            Debug.Log("Hit: " + hit.transform.name);

            /*
            // Apply damage to the hit object if it has a Health component
            Health health = hit.transform.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }*/
        }
    }
    void Muzzle()
    {
        _muzzleFlash.gameObject.SetActive(true);
        ParticleSystem.MainModule mainModule = _muzzleFlash.main;
        mainModule.stopAction = ParticleSystemStopAction.None;
        _muzzleFlash.Play();
    }
    #region Get & Set
    public float GetFireTimer()
    {
        return _fireTimer;
    }
    public void SetFireTimer(float fireTimer)
    {
        _fireTimer = fireTimer;
    }
    #endregion
}
