using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBasic : MonoBehaviour, IWeapon
{
    [Header("UI Identity")]
    [SerializeField] private string weaponName = "Basic Blaster";
    [SerializeField] private Sprite weaponIcon;
    [SerializeField] private AmmoType ammoType;

    [Header("Firing")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.2f;

    [Header("Ammo")]
    [SerializeField] private bool usesAmmo = false;
    //[SerializeField] private int maxAmmo = -1;

    public AmmoType AmmoType => ammoType;

    private float nextFireTime;
    

    // =========================
    // IWeapon (UI DATA)
    // =========================
    public string WeaponName => weaponName;
    public Sprite WeaponIcon => weaponIcon;
    public bool UsesAmmo => usesAmmo;
    public bool CanFire
    {
        get
        {
            if (Time.time < nextFireTime)
                return false;


            return true;
        }
    }

    private void Awake()
    {
       
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