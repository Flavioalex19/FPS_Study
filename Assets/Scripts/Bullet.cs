using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    [SerializeField]float _lifeTime = 10f;
    [SerializeField]float damage = 120f;
    

    // Update is called once per frame
    void Update()
    {
        if (_lifeTime>0)
        {
            _lifeTime -= Time.deltaTime;
        }
        else Destroy(gameObject);
    }

    public float BulletDamage()
    {
        return damage;
    }
}
