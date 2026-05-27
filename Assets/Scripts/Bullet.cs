using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : ProjectileBase
{
    private Vector3 direction = Vector3.up;
    private Rigidbody rb;

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

    private void Start()
    {
        var col = GetComponent<Collider>();
        Debug.Log($"[BULLET START] isTrigger = {col.isTrigger}");
        Debug.Log($"[BULLET START] Rigidbody = {GetComponent<Rigidbody>() != null}");

    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    private void FixedUpdate()
    {
        Tick(Time.fixedDeltaTime);
    }

    protected override void Tick(float dt)
    {
        // physics-safe kinematic motion
        rb.MovePosition(rb.position + direction * speed * dt);
    }

    private bool hasHit = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (hasHit) return;
        hasHit = true;

        Debug.Log($"[BULLET COLLISION] {collision.gameObject.name}");

        CombatResolver.DealDirectDamage(gameObject, collision.gameObject, Damage);

        Destroy(gameObject);
    }

    protected override void OnHit(Collider other)
    {
        CombatResolver.DealDirectDamage(gameObject, other.gameObject, Damage);
        Destroy(gameObject);
    }

}