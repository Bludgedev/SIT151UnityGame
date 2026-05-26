using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : ProjectileBase
{
    private Vector3 direction = Vector3.up;

    protected float dt;

    protected virtual float GetDeltaTime()
    {
        return TimeDilationSystem.Instance == null
            ? Time.deltaTime
            : Time.deltaTime * TimeDilationSystem.Instance.WorldTimeScale;
    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

    protected override void Tick(float dt)
    {
        dt = GetDeltaTime();
        transform.position += direction * speed * dt;

        if (transform.position.y > 8f || transform.position.y < -8f)
            Destroy(gameObject);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        Debug.Log($"TRIGGER HIT: {name} -> {other.name}");
        CombatResolver.DealDirectDamage(gameObject, other.gameObject, Damage);
        Destroy(gameObject);
    }

    protected override void OnHit(Collider other)
    {
        CombatResolver.DealDirectDamage(gameObject, other.gameObject, Damage);
        Destroy(gameObject);
    }

    
}