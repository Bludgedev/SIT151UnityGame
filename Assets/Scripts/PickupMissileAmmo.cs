using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PickupMissileAmmo : PickupBase
{
    [SerializeField] private AmmoType ammoType = AmmoType.Missile;
    [SerializeField] private int amount = 4;

    public override void Collect(GameObject player)
    {
        var inv = player.transform.root.GetComponent<PlayerWeaponInventory>();

        if (inv == null)
        {
            Debug.LogError("No PlayerWeaponInventory found on " + player.transform.root.name);
            return;
        }

        inv.AddAmmo(ammoType, amount);

        Debug.Log($"Added {amount} {ammoType} ammo to {player.transform.root.name}");
    }
}