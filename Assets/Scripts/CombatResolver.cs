using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public static class CombatResolver
{
 
    // SINGLE TARGET DAMAGE
    public static void DealDirectDamage(GameObject attacker, GameObject target, float damage)
    {
        if (target == null || attacker == null) return;

        if (!target.TryGetComponent<IDamageable>(out var dmg))
            return;

        dmg.TakeDamage(damage);
    }


    // AREA DAMAGE
    public static void DealAreaDamage(GameObject attacker, Vector3 position, float radius, float damage)
    {
        Collider[] hits = Physics.OverlapSphere(position, radius);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<IDamageable>(out var dmg))
            {
                dmg.TakeDamage(damage);
            }
        }
    }
}