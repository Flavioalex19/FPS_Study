using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitScanShooting : MonoBehaviour
{

    [SerializeField] float damage = 10f;
    [SerializeField] float fireRate = 0.1f;
    [SerializeField] float range = 100f;
    [SerializeField] LayerMask _hitLayers;

    public Camera PlayerCamera;
    private float _fireTimer = 0f;


    void Update()
    {
        _fireTimer += Time.deltaTime;

        /*
        if (Input.GetButton("Fire1") && fireTimer >= fireRate)
        {
            Shoot();
            fireTimer = 0f;
        }
        */
    }

    public void Shoot()
    {
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
