using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyBasic : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 1f;
    private float currentHealth;

    [Header("Shooting")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 6f;
    [SerializeField] private float damage = 3f;

    [SerializeField] private float fireRateMin = 0.5f;
    [SerializeField] private float fireRateMax = 1.5f;

    [Header("Accuracy")]
    [SerializeField] private float maxSpreadAngle = 25f; // degrees

    private float fireTimer;
    private float nextFireTime;

    private Transform player;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        SetNextFireTime();
    }

    private void Update()
    {
        if (player == null) return;

        fireTimer += Time.deltaTime;

        if (fireTimer >= nextFireTime)
        {
            fireTimer = 0f;
            Fire();
            SetNextFireTime();
        }
    }

    private void SetNextFireTime()
    {
        nextFireTime = Random.Range(fireRateMin, fireRateMax);
    }

    private void Fire()
    {
        if (bulletPrefab == null) return;

        // Direction to player
        Vector3 dir = (player.position - transform.position);
        dir.z = 0f;
        dir.Normalize();

        // Add inaccuracy (weighted toward correct aim)
        float spread = Random.Range(-maxSpreadAngle, maxSpreadAngle);
        spread *= Random.Range(0.3f, 1f); // bias toward center

        dir = Quaternion.Euler(0f, 0f, spread) * dir;

        // Spawn bullet
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        var b = bullet.GetComponent<EnemyBullet>();
        if (b != null)
        {
            b.owner = gameObject;
            b.damage = damage;
            b.Init(dir, bulletSpeed);
        }
    }

    // =========================
    // DAMAGE SYSTEM
    // =========================

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        Debug.Log($"[ENEMY] {name} HP: {currentHealth}");

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"[ENEMY] {name} DIED");

        Destroy(gameObject);
    }
}