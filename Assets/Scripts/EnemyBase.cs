using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyBase : MonoBehaviour
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
    private bool hasTakenHit = false;

    

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    protected virtual void Update()
    {
        Move();
        CheckOffscreenDestroy();
    }

    protected virtual void Move()
    {
        transform.position += Vector3.down * speed * Time.deltaTime;
    }

    //====================================================
    // DAMAGE SYSTEM (UNIVERSAL)
    //====================================================


    protected virtual void OnTriggerEnter(Collider other)
    {
        if (isDying || hasTakenHit)
            return;

        TryApplyDamage(other.gameObject);
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (isDying || hasTakenHit)
            return;

        PlayerShipController player = collision.gameObject.GetComponent<PlayerShipController>();

        if (player != null)
        {
            float ramDamage = player.RamDamage;

            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();

            TakeDamage(ramDamage);
            playerHealth.TakeDamage(ramDamage);
        }
    }


    private void TryApplyDamage(GameObject other)
    {
        if (other.TryGetComponent<IDamageDealer>(out var damageDealer))
        {
            TakeDamage(damageDealer.Damage);
        }
    }

    public virtual void TakeDamage(float damage)
    {
        if (isDying) return;

        hasTakenHit = true;

        currentHealth -= damage;
        OnHit(damage);

        if (currentHealth <= 0f)
            Die();
    }


    protected virtual void OnHit(float damage)
    {
        // optional: flash, sound, particles
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

    private void OnBecameVisible()
    {
        hasBeenVisible = true;
    }

    private void CheckOffscreenDestroy()
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