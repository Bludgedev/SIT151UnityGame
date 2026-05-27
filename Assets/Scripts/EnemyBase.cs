using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    [Header("Movement")]
    public float speed = 2f;

    [Header("Health")]
    public float maxHealth = 1f;
    protected float currentHealth;

    [Header("Death")]
    public RuntimeAnimatorController explosion;
    public float deathDelay = 2f;

    [Header("Cleanup")]
    [SerializeField] private bool destroyWhenOffscreen = true;

    protected bool hasBeenVisible = false;
    protected bool isDying = false;

    protected float dt;
    private Rigidbody rb;


    protected virtual void Awake()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody>();
    }

    protected virtual void Update()
    {
        CheckOffscreenDestroy();   
    }

    protected virtual void FixedUpdate()
    {
        float dt = GetDeltaTime();
        Tick(dt);
        Move();
    }

    protected virtual void Move()
    {
        rb.MovePosition(rb.position + Vector3.down * speed * dt);
    }

    //====================================================
    // SlowMo mechanics
    //====================================================
    protected virtual float GetDeltaTime()
    {
        float scale = TimeDilationSystem.Instance != null
            ? TimeDilationSystem.Instance.WorldTimeScale
            : 1f;

        return Time.fixedDeltaTime * scale;
    }

    protected abstract void Tick(float dt);

    //====================================================
    // DAMAGE SYSTEM (UNIVERSAL)
    //====================================================
    // NOTE:
    // All damage is handled externally via CombatResolver.

    public virtual void TakeDamage(float damage)
    {
        if (isDying)
            return;

        currentHealth -= damage;

        Debug.Log($"{name} took {damage}, HP = {currentHealth}");

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    //====================================================
    // DEATH
    //====================================================

    public virtual void Die()
    {
        if (isDying)
            return;

        isDying = true;

        AudioManager.Instance.PlayExplosion();

        StartCoroutine(DeathRoutine());
    }

    protected virtual IEnumerator DeathRoutine()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        Animator anim = GetComponent<Animator>();
        if (anim != null && explosion != null)
            anim.runtimeAnimatorController = explosion;

        yield return new WaitForSecondsRealtime(deathDelay);

        Destroy(gameObject);
    }

    //====================================================
    // OFFSCREEN CLEANUP
    //====================================================

    protected virtual void OnBecameVisible()
    {
        hasBeenVisible = true;
    }

    protected virtual void CheckOffscreenDestroy()
    {
        if (!destroyWhenOffscreen)
            return;

        Renderer renderer = GetComponent<Renderer>();

        if (renderer != null &&
            hasBeenVisible &&
            !renderer.isVisible)
        {
            Destroy(gameObject);
        }
    }

    

}