using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponShotgun : MonoBehaviour, IWeapon, IInventoryUser
{
    [Header("UI")]
    [SerializeField] private string weaponName = "Scatter Cannon";
    [SerializeField] private Sprite weaponIcon;

    [Header("Ammo")]
    [SerializeField] private AmmoType ammoType = AmmoType.Shell;
    

    public AmmoType AmmoType => ammoType;
    public bool UsesAmmo => true;

    [Header("Firing")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private ParticleSystem muzzleFlash;

    [SerializeField] private int pelletCount = 8;
    [SerializeField] private float spreadAngle = 12f;
    [SerializeField] private float range = 80f;
    [SerializeField] private float fireRate = 0.8f;

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

        Debug.Log("[SHOTGUN] FIRE");

        // visual only
        if (muzzleFlash != null)
            muzzleFlash.Play();

        Vector3 origin = firePoint.position;
        Vector3 forward = firePoint.up;

        for (int i = 0; i < pelletCount; i++)
        {
            Vector3 dir = ApplySpread(forward, spreadAngle);

            if (Physics.Raycast(origin, dir, out RaycastHit hit, range))
            {
                Debug.Log($"[SHOTGUN] HIT {hit.collider.name}");

                var dmg = hit.collider.GetComponentInParent<IDamageable>();
                if (dmg != null)
                {
                    dmg.TakeDamage(1f);
                }
            }
        }
    }

    private Vector3 ApplySpread(Vector3 forward, float angle)
    {
        float z = Random.Range(-angle, angle);
        return Quaternion.Euler(0f, 0f, z) * forward;
    }

    public void Tick() { }
}