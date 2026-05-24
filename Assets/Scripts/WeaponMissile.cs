using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMissile : MonoBehaviour, IWeapon
{
    [Header("UI")]
    [SerializeField] private string weaponName = "Missile Launcher";
    [SerializeField] private Sprite weaponIcon;

    [Header("Ammo")]
    [SerializeField] private int maxAmmo = 30;
    [SerializeField] private bool usesAmmo = true;

    [Header("Firing")]
    [SerializeField] private GameObject missilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.6f;

    private int currentAmmo;
    private float nextFireTime;

    public string WeaponName => weaponName;
    public Sprite WeaponIcon => weaponIcon;

    public bool UsesAmmo => usesAmmo;
    public int CurrentAmmo => currentAmmo;
    public int MaxAmmo => maxAmmo;

    public bool CanFire =>
        Time.time >= nextFireTime &&
        (!usesAmmo || currentAmmo > 0);


    private void Awake()
    {
        currentAmmo = maxAmmo;
    }

    public void Fire()
    {
        if (!CanFire) return;

        nextFireTime = Time.time + fireRate;

        if (usesAmmo)
            currentAmmo--;

        Instantiate(missilePrefab, firePoint.position, firePoint.rotation);
    }

    public void Tick() { }

}
