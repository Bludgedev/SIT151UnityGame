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
        float dt = TimeDilationSystem.Instance == null
            ? Time.deltaTime
            : Time.deltaTime * TimeDilationSystem.Instance.WorldTimeScale;

        Tick(dt);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[PROJECTILE BASE] HIT: {name} -> {other.name}");

    }



    protected abstract void OnHit(Collider other);
}