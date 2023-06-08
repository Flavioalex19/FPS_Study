using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    [SerializeField]float _lifeTime = 10f;
    [SerializeField]float damage = 120f;

    [SerializeField] ParticleSystem _vfx_explosion;
    bool _canExplode = false;

    

    private void Start()
    {
        //_vfx_explosion.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (_lifeTime > 0)
        {
            _lifeTime -= Time.deltaTime;
        }
        else 
        {
            Explosion(transform);
            Destroy(gameObject);
        }
       
        
    }

    public float BulletDamage()
    {
        
        return damage;
    }
    public void Explosion(Transform hitPosition)
    {
        //_canExplode=canExplode;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 20f);
        foreach (var hitCollider in hitColliders)
        {

            if (hitCollider.tag == "Enemy") hitCollider.GetComponent<Enemy>().TakeDamage(480f);
            
        }
        /*
        Instantiate(_vfx_explosion, hitPosition.position, Quaternion.identity);
        _vfx_explosion.gameObject.SetActive(true);
        _vfx_explosion.Play();
        */
        ParticleSystem explosion = Instantiate(_vfx_explosion, hitPosition.position, Quaternion.identity);
        _vfx_explosion.Play();
    }
}
