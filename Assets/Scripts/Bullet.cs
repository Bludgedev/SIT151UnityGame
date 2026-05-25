using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : ProjectileBase, IDamageDealer
{
    [Header("Movement")]
    [SerializeField] private float speed = 10f;

    [Header("Damage")]
    [SerializeField] private float damage = 1f;

    private Vector3 direction = Vector3.up;

    public float Damage => damage;

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }


    
    private void OnTriggerEnter(Collider other)
    {
        EnemyBase enemy = other.GetComponentInParent<EnemyBase>();

        if (enemy != null)
        {
            enemy.TakeDamage(Damage);
            Destroy(gameObject);
        }
    }


    protected override void Tick(float dt)
    {
        transform.position += direction * speed * dt;

        if (transform.position.y > 8f || transform.position.y < -8f)
        {
            Destroy(gameObject);
        }
    }

}
