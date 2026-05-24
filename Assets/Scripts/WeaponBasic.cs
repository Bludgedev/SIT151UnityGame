using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBasic : MonoBehaviour, IWeapon
{
    [Header("UI Identity")]
    [SerializeField] private string weaponName = "Basic Blaster";
    [SerializeField] private Sprite weaponIcon;


    [Header("Firing")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.2f;

    [Header("Ammo")]
    [SerializeField] private bool usesAmmo = false;
    [SerializeField] private int maxAmmo = -1;


    private float nextFireTime;
    private int currentAmmo;

    // =========================
    // IWeapon (UI DATA)
    // =========================
    public string WeaponName => weaponName;
    public Sprite WeaponIcon => weaponIcon;
    public bool UsesAmmo => usesAmmo;
    public int CurrentAmmo => currentAmmo;
    public int MaxAmmo => maxAmmo;
    public bool CanFire
    {
        get
        {
            if (Time.time < nextFireTime)
                return false;

            if (usesAmmo && currentAmmo <= 0)
                return false;

            return true;
        }
    }

    private void Awake()
    {
        currentAmmo = maxAmmo;
    }


    // =========================
    // IWeapon (BEHAVIOUR)
    // =========================
    public void Fire()
    {
        if (!CanFire) return;

        nextFireTime = Time.time + fireRate;

        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }

    public void Tick()
    {
        // optional later (charge weapons, cooldown visuals, etc.)
    }
}