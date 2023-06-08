using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{

    [SerializeField] float _maxHP;
    [SerializeField] float _hp;
    [SerializeField] float _armor = 1f;

    [SerializeField]Image ui_hp_bar;

    private void Start()
    {
        _hp = _maxHP;
    }
    private void Update()
    {
        if (_hp < 0)
        {
            Destroy(gameObject);
        }

        ui_hp_bar.fillAmount = _hp / _maxHP;


    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Bullet")
        {
            TakeDamage(other.GetComponent<Bullet>().BulletDamage());
            Destroy(other.gameObject);
        }
       
    }

    public void TakeDamage(float damage)
    {
        _hp -= damage * _armor;
    }
}
