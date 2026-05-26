using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : ProjectileBase
{
    private Vector3 direction = Vector3.up;

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

    protected override void Tick(float dt)
    {
        transform.position += direction * speed * dt;

        if (transform.position.y > 8f || transform.position.y < -8f)
            Destroy(gameObject);
    }

    protected override void OnHit(Collider other)
    {
        CombatResolver.DealDirectDamage(gameObject, other.gameObject, Damage);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"[COLLISION ENTER] {name} hit {collision.gameObject.name}");
    }

    private void OnCollisionStay(Collision collision)
    {
        Debug.Log($"[COLLISION STAY] {name} touching {collision.gameObject.name}");
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log($"[COLLISION EXIT] {name} left {collision.gameObject.name}");
    }


}