using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 3f;
    [SerializeField] private RuntimeAnimatorController explosionController;
    [SerializeField] private float currentHealth;

    [Header("Debug")]
    [SerializeField] private bool verbose = true;
    [SerializeField] private float speed = 2f;

    [Header("Shooting")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 6f;
    [SerializeField] private float damage = 3f;

    [SerializeField] private float fireRateMin = 0.5f;
    [SerializeField] private float fireRateMax = 2.5f;

    [Header("Accuracy")]
    [SerializeField] private float maxSpreadAngle = 25f; // degrees

    private Transform player;
    private float fireTimer;
    private float nextFireTime;

    [Header("Score")]
    [SerializeField] private int bounty = 10;

    private bool isDying = false;
    private Vector3 lastPos;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        SetNextFireTime();
    }

    private void Awake()
    {

        currentHealth = maxHealth;

        //Debug.Log($"[ENEMY] SPAWN: {name} | HP={currentHealth}");
        //Debug.Log($"[ENEMY] Layer={LayerMask.LayerToName(gameObject.layer)} | Tag={tag}");
        //Debug.Log($"[ENEMY] Scene={gameObject.scene.name} | Active={gameObject.activeInHierarchy}");
    }

    private void FixedUpdate()
    {
        Move(Time.fixedDeltaTime);
        if (player == null) return;

        fireTimer += Time.deltaTime;

        if (fireTimer >= nextFireTime)
        {
            fireTimer = 0f;
            Fire();
            SetNextFireTime();
        }

        lastPos = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
       // CombatResolver.ProcessRam(gameObject, collision.gameObject, collision);
    }

    

    protected virtual void Move(float dt)
    {
        var rb = GetComponent<Rigidbody>();

        Vector3 move = Vector3.down * speed * dt;

        Debug.DrawRay(transform.position, move, Color.red);

        rb.MovePosition(rb.position + move);
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

    private void SetNextFireTime()
    {
        nextFireTime = Random.Range(fireRateMin, fireRateMax);
    }



    // IDamageable contract ONLY
    public void TakeDamage(float damage)
    {
        if (isDying)
            return;
        
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

        isDying= true;
        ScoreManager.Instance?.AddScore(bounty);

        //AudioManager.Instance?.PlayExplosion();

        GameObject fx = new GameObject("EnemyExplosionFX");
        fx.transform.position = transform.position;

        var sr = fx.AddComponent<SpriteRenderer>();
        var anim = fx.AddComponent<Animator>();

        anim.runtimeAnimatorController = explosionController;
        sr.sortingOrder = 10;

        Destroy(fx, 1f);

        Destroy(gameObject);
    }
}