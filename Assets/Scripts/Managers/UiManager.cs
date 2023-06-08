using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    #region Weapon UI
    [SerializeField] Animator _animator_weaponFireModeUI;
    Weapon _weapon;
    #endregion

    [SerializeField] Image ui_overheatMeter;

    // Start is called before the first frame update
    void Start()
    {
        _weapon = GameObject.FindGameObjectWithTag("Weapon").GetComponent<Weapon>();
    }

    // Update is called once per frame
    void Update()
    {
        ui_overheatMeter.fillAmount = _weapon.GetCurrentOverheatValue()/_weapon.GetMaxOverheatValue();

        _animator_weaponFireModeUI.SetBool("isFullAutoOn",_weapon.GetAlternateFireMode());
    }
}
