using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class EnemyBullet : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float lifetime = 5f;

    public GameObject owner;
    public float damage = 1f;

    private Vector3 moveDir;
    private float timer;
    private Rigidbody rb;
    private Collider col;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        if (col != null)
            col.isTrigger = true;
    }

    private void Start()
    {
        
    }

    public void Init(Vector3 direction, float overrideSpeed)
    {
        moveDir = direction.normalized;
        speed = overrideSpeed;

        float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void FixedUpdate()
    {
        if (rb != null)
        {
            rb.velocity = moveDir * speed;
        }
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        if (rb == null)
        {
            transform.position += moveDir * speed * dt;
        }

        timer += dt;

        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == owner) return;

        CombatResolver.DealDirectDamage(owner, other.gameObject, damage);

        Destroy(gameObject);
    }
}
