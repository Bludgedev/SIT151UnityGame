using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

        if (weaponController.Inventory != null)
            weaponController.Inventory.OnAmmoChanged += HandleAmmoChanged;
    }

    private void OnDisable()
    {
        weaponController.OnWeaponChanged -= UpdateUI;

        if (weaponController.Inventory != null)
            weaponController.Inventory.OnAmmoChanged -= HandleAmmoChanged;
    }

    private void Start()
    {
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

    private void UpdateAmmo(IWeapon weapon)
    {
        if (!weapon.UsesAmmo)
        {
            weaponAmmoCountText.text = "Infinite";
            return;
        }

        int current = weaponController.Inventory.GetCurrentAmmo(weapon.AmmoType);
        int max = weaponController.Inventory.GetMaxAmmo(weapon.AmmoType);

        weaponAmmoCountText.text = $"{current} / {max}";
    }

    private void HandleAmmoChanged()
    {
        if (weaponController.CurrentWeapon != null)
        {
            UpdateAmmo(weaponController.CurrentWeapon);
        }
    }


}