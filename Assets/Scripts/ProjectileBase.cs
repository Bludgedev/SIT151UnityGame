using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectileBase : MonoBehaviour
{
    [SerializeField] protected float damage = 1f;
    [SerializeField] protected float speed = 10f;

    public float Damage => damage;

    protected abstract void Tick(float dt);

    protected virtual void Update()
    {
        Tick(Time.deltaTime);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        Debug.Log($"HIT: {name} -> {other.name}");

        var dmg = other.GetComponentInParent<IDamageable>();

        Debug.Log($"HAS IDAMAGEABLE? {(dmg != null)}");

        if (dmg != null)
        {
            dmg.TakeDamage(1f);
            Debug.Log("DAMAGE APPLIED");
        }
    }



    protected abstract void OnHit(Collider other);
}