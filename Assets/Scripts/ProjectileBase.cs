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
        OnHit(other);
    }

    protected abstract void OnHit(Collider other);
}