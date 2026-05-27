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

    //  RAMMING DAMAGE
    //private static readonly HashSet<int> recentRamPairs = new();

    public static void ProcessPlayerRam(GameObject player, GameObject other)
    {
        if (player == null || other == null)
            return;

        if (!other.CompareTag("EnemyShip"))
            return;

        var enemyDmg = other.GetComponentInParent<IDamageable>();
        var playerHealth = player.GetComponent<PlayerHealth>();

        if (enemyDmg == null || playerHealth == null)
            return;

        const float ramDamage = 5f;

        enemyDmg.TakeDamage(ramDamage);
        playerHealth.TakeDamage(ramDamage);

        Debug.Log($"[RAM] Player <-> {other.name} | dmg={ramDamage}");
    }




    /*public static void ProcessRam(GameObject a, GameObject b, Collision collision)
    {
        if (a == null || b == null) return;
        if (a == b) return;

        // stable pair key (order-independent)
        int idA = a.GetInstanceID();
        int idB = b.GetInstanceID();
        int pairKey = idA < idB
            ? idA * 73856093 ^ idB * 19349663
            : idB * 73856093 ^ idA * 19349663;

        if (recentRamPairs.Contains(pairKey))
            return;

        recentRamPairs.Add(pairKey);

        const float ramDamage = 5f;

        var dmgA = a.GetComponentInParent<IDamageable>();
        var dmgB = b.GetComponentInParent<IDamageable>();

        dmgA?.TakeDamage(ramDamage);
        dmgB?.TakeDamage(ramDamage);

        Debug.Log($"[RAM] {a.name} <-> {b.name} | dmg={ramDamage}");

        // clear later
        CoroutineRunner.Instance.StartCoroutine(RemovePairAfterDelay(pairKey, 0.2f));
    }

    private static IEnumerator RemovePairAfterDelay(int key, float delay)
    {
        yield return new WaitForSeconds(delay);
        recentRamPairs.Remove(key);
    }
    */
}