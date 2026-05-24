using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponUI : MonoBehaviour
{
    [SerializeField] private WeaponController weaponController;

    [Header("UI")]
    [SerializeField] private Image weaponIcon;
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [SerializeField] private TextMeshProUGUI weaponAmmoCountText;
    private void OnEnable()
    {
        weaponController.OnWeaponChanged += UpdateUI;
    }

    private void OnDisable()
    {
        weaponController.OnWeaponChanged -= UpdateUI;
    }

    private void Start()
    {
        // safety sync in case event fired before UI enabled
        if (weaponController.CurrentWeapon != null)
            UpdateUI(weaponController.CurrentWeapon);
    }

    private void UpdateUI(IWeapon weapon)
    {
        if (weapon == null)
            return;

        weaponNameText.text = weapon.WeaponName;

        weaponIcon.sprite = weapon.WeaponIcon;
        weaponIcon.enabled = weapon.WeaponIcon != null;

        UpdateAmmo(weapon);
    }

    private void Update()
    {
        if (weaponController.CurrentWeapon != null)
        {
            UpdateAmmo(weaponController.CurrentWeapon);
        }
    }

    private void UpdateAmmo(IWeapon weapon)
    {
        if (weapon.UsesAmmo)
        {
            weaponAmmoCountText.text = $"{weapon.CurrentAmmo} / {weapon.MaxAmmo}";
        }
        else
        {
            weaponAmmoCountText.text = "Infinite";
        }
    }

}