using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 3f;
    private float currentHealth;

    [Header("Debug")]
    [SerializeField] private bool verbose = true;

    private void Awake()
    {
        currentHealth = maxHealth;

        //Debug.Log($"[ENEMY] SPAWN: {name} | HP={currentHealth}");
        //Debug.Log($"[ENEMY] Layer={LayerMask.LayerToName(gameObject.layer)} | Tag={tag}");
        //Debug.Log($"[ENEMY] Scene={gameObject.scene.name} | Active={gameObject.activeInHierarchy}");
    }

    // IDamageable contract ONLY
    public void TakeDamage(float damage)
    {
        if (verbose)
            Debug.Log($"[ENEMY] HIT: {name} taking {damage}");

        currentHealth -= damage;

        if (verbose)
            Debug.Log($"[ENEMY] HP UPDATE: {name} -> {currentHealth}/{maxHealth}");

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"[ENEMY] DEATH: {name}");

        // Optional: visual confirmation even if pooling/physics is weird
        transform.position = new Vector3(9999, 9999, 9999);

        Destroy(gameObject);
    }
}