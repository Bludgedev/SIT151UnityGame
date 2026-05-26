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

    protected override void OnTriggerEnter(Collider other)
    {
        Debug.Log("[BULLET] Calling CombatResolver");
        Debug.Log($"TRIGGER HIT: {name} -> {other.name}");
        Debug.Log($"BULLET TRIGGER HIT: {other.name} | Layer: {LayerMask.LayerToName(other.gameObject.layer)}");

        CombatResolver.DealDirectDamage(gameObject, other.gameObject, Damage);

        Destroy(gameObject);
    }

    protected override void OnHit(Collider other)
    {
        CombatResolver.DealDirectDamage(gameObject, other.gameObject, Damage);
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"COLLISION: {collision.gameObject.name}");
    }
}