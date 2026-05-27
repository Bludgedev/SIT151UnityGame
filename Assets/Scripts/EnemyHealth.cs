using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 3f;
    [SerializeField] private RuntimeAnimatorController explosionController;
    private float currentHealth;

    [Header("Debug")]
    [SerializeField] private bool verbose = true;
    [SerializeField] private float speed = 2f;

    private bool isDying = false;

    private void Start()
    {
      
    }

    private void Awake()
    {

        currentHealth = maxHealth;

        //Debug.Log($"[ENEMY] SPAWN: {name} | HP={currentHealth}");
        //Debug.Log($"[ENEMY] Layer={LayerMask.LayerToName(gameObject.layer)} | Tag={tag}");
        //Debug.Log($"[ENEMY] Scene={gameObject.scene.name} | Active={gameObject.activeInHierarchy}");
    }

    private void FixedUpdate()
    {
        Move(Time.fixedDeltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
       // CombatResolver.ProcessRam(gameObject, collision.gameObject, collision);
    }

    

    protected virtual void Move(float dt)
    {
        var rb = GetComponent<Rigidbody>();

        Vector3 move = Vector3.down * speed * dt;

        Debug.DrawRay(transform.position, move, Color.red);

        rb.MovePosition(rb.position + move);
    }

    // IDamageable contract ONLY
    public void TakeDamage(float damage)
    {
        if (isDying)
            return;
        
        if (verbose)
            Debug.Log($"[ENEMY] HIT: {name} taking {damage}");

        currentHealth -= damage;

        if (verbose)
            Debug.Log($"[ENEMY] HP UPDATE: {name} -> {currentHealth}/{maxHealth}");

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"[ENEMY] DEATH: {name}");

        isDying= true;

        AudioManager.Instance?.PlayExplosion();

        GameObject fx = new GameObject("EnemyExplosionFX");
        fx.transform.position = transform.position;

        var sr = fx.AddComponent<SpriteRenderer>();
        var anim = fx.AddComponent<Animator>();

        anim.runtimeAnimatorController = explosionController;
        sr.sortingOrder = 10;

        Destroy(fx, 1f);

        Destroy(gameObject);
    }
}