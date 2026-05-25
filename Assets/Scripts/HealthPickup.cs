using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : PickupBase
{
    [SerializeField] private float healAmount = 25f;

    public override void Collect(GameObject player)
    {
        if (player.TryGetComponent<PlayerHealth>(out var health))
        {
            health.Heal(healAmount);
        }
        else
        {
            Debug.LogError("No PlayerHealth found on player");
        }
    }
}