using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Missile : ProjectileBase, IDamageDealer
{
    [Header("Movement")]
    //[SerializeField] private float speed = 8f;
   // [SerializeField] private float turnSpeed = 200f;
    [SerializeField] private float trackingStrength = 5f;
    [SerializeField] private float maxTurnAngle = 30f;
    [SerializeField] private float lifetime = 5f;

    private float lifeTimer;

    [Header("Damage")]
    //[SerializeField] private float damage = 5f;
    [SerializeField] private float explosionRadius = 2.5f;
    [SerializeField] private RuntimeAnimatorController explosionController;
    [SerializeField] private float explosionLifetime = 1f;

    private Transform target;
    private Vector3 moveDir;

    //public float Damage => damage;

    private bool hasExploded;
    private Collider col;
    private Rigidbody rb;

    private void Start()
    {
        moveDir = transform.up; // treat UP as forward (2D style)
        AcquireTarget();
    }

    private void Awake()
    {
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
    }

    protected override void Tick(float dt)
    {
        if (target == null)
            AcquireTarget();

        if (target != null)
        {
            Vector3 toTarget = target.position - transform.position;
            toTarget.z = 0f;

            Vector3 desiredDir = toTarget.normalized;
            Vector3 currentDir = moveDir.normalized;

            moveDir = Vector3.Slerp(
                currentDir,
                desiredDir,
                trackingStrength * dt
            );
        }

        // Move forward
        Vector3 movement = moveDir.normalized * speed * dt;
        movement.z = 0f;

        rb.MovePosition(rb.position + movement);

        // LOCK rotation to Z only
        float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        lifeTimer += dt;

        if (lifeTimer >= lifetime)
        {
            Explode();
        }
    }

   
    protected override void OnHit(Collider other)
    {
        // intentionally empty
    }


    protected override void OnTriggerEnter(Collider other)
    {
        if (hasExploded) return;

        Debug.Log($"TRIGGER HIT: {name}  {GetInstanceID()} -> {other.name}");
        Explode();
    }

    private void AcquireTarget()
    {
        EnemyBase[] enemies = FindObjectsOfType<EnemyBase>();

        float closest = Mathf.Infinity;
        Transform best = null;

        foreach (var e in enemies)
        {
            Vector3 diff = e.transform.position - transform.position;
            diff.z = 0f;

            float dist = diff.sqrMagnitude;

            if (dist < closest)
            {
                closest = dist;
                best = e.transform;
            }
        }

        target = best;
    }



    private void Explode()
    {
        if (hasExploded) return;

        hasExploded = true;
        col.enabled = false;

        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.detectCollisions = false;
            rb.velocity = Vector3.zero;
        }

        SpawnExplosionFX();
        DealDamage();

        Destroy(gameObject);
    }

    private void DealDamage()
    {
        CombatResolver.DealAreaDamage(
            gameObject,
            transform.position,
            explosionRadius,
            Damage
        );
    }

    private void SpawnExplosionFX()
    {
        GameObject fx = new GameObject("ExplosionFX");

        fx.transform.position = transform.position;
        fx.transform.rotation = Quaternion.identity;
        fx.transform.localScale = transform.localScale;

        SpriteRenderer sr = fx.AddComponent<SpriteRenderer>();
        Animator anim = fx.AddComponent<Animator>();

        anim.runtimeAnimatorController = explosionController;
        sr.sortingOrder = 10;

        Destroy(fx, explosionLifetime);
    }


    

}