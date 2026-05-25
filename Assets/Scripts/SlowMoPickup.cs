using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMoPickup : PickupBase
{
    [SerializeField] private float duration = 5f;
    [SerializeField] private float worldSlow = 0.4f;
    [SerializeField] private float bulletSlow = 0.6f;
    [SerializeField] private float playerSlow = 0.8f;

    public override void Collect(GameObject player)
    {
        if (TimeDilationSystem.Instance != null)
        {
            TimeDilationSystem.Instance.Trigger(duration, worldSlow, bulletSlow, playerSlow);
        }
    }
}
