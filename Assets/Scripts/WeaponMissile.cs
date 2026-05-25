using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WeaponMissile : MonoBehaviour, IWeapon, IInventoryUser
{
    [Header("UI")]
    [SerializeField] private string weaponName = "Seeker Missile";
    [SerializeField] private Sprite weaponIcon;

    [Header("Ammo")]
    [SerializeField] private AmmoType ammoType = AmmoType.Missile;

    public AmmoType AmmoType => ammoType;
    public bool UsesAmmo => true;

    [Header("Firing")]
    [SerializeField] private GameObject missilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.6f;

    private float nextFireTime;
    private PlayerWeaponInventory inventory;

    public string WeaponName => weaponName;
    public Sprite WeaponIcon => weaponIcon;

    public bool CanFire =>
        Time.time >= nextFireTime &&
        inventory != null &&
        inventory.GetCurrentAmmo(ammoType) > 0;

    public void SetInventory(PlayerWeaponInventory inv)
    {
        inventory = inv;
    }

    public void Fire()
    {
        if (!CanFire) return;

        nextFireTime = Time.time + fireRate;

        if (!inventory.ConsumeAmmo(ammoType, 1))
            return;

        Instantiate(missilePrefab, firePoint.position, firePoint.rotation);
    }

    public void Tick() { }
}