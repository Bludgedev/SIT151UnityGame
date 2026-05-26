using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public static class CombatResolver
{

    // SINGLE TARGET DAMAGE
    public static void DealDirectDamage(GameObject attacker, GameObject target, float damage)
    {
        if (target == attacker) return;

        var dmg = target.GetComponentInParent<IDamageable>();
        if (dmg == null) return;

        Debug.Log($"[RESOLVER] DIRECT DAMAGE to {target.name}");

        dmg.TakeDamage(damage);
    }


    // AREA DAMAGE
    public static void DealAreaDamage(GameObject attacker, Vector3 position, float radius, float damage)
    {
        Debug.Log("[RESOLVER] ENTER DealAreaDamage");

        var scene = attacker.scene.GetPhysicsScene();

        Collider[] hits = new Collider[32];
        int count = scene.OverlapSphere(
            position,
            radius,
            hits,
            ~0, // layerMask ALL
            QueryTriggerInteraction.Collide // IMPORTANT
        );

       
        Debug.Log($"Hits found: {count}");

        for (int i = 0; i < count; i++)
        {
            var hit = hits[i];

            if (hit == null) continue;

            Debug.Log($"[RESOLVER] Checking: {hit.name}");

            // IMPORTANT: use root first, not just parent chain
            var dmg = hit.GetComponentInParent<IDamageable>();

            
            if (dmg != null)
            {
                Debug.Log($"[RESOLVER] APPLYING DAMAGE to {hit.name}");
                dmg.TakeDamage(damage);
            }
        }
    }
}