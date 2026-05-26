using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class EnemyBase : MonoBehaviour
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

    protected virtual void Awake()
    {
        currentHealth = maxHealth;

        var col = GetComponent<Collider>();

        if (col == null)
        {
            Debug.LogError($"{name}: NO COLLIDER");
        }
        else
        {
            Debug.Log($"{name} Collider: isTrigger={col.isTrigger}");
        }
    }

    protected virtual void Update()
    {
        dt = GetDeltaTime();
        Tick(dt);
        Move();
        CheckOffscreenDestroy();
    }

    protected virtual void Move()
    {
        GetComponent<Rigidbody>().MovePosition(transform.position + Vector3.down * speed * Time.deltaTime);
    }

    //====================================================
    // SlowMo mechanics
    //====================================================
    protected virtual float GetDeltaTime()
    {
        if (TimeDilationSystem.Instance == null)
            return Time.deltaTime;

        return Time.deltaTime * TimeDilationSystem.Instance.WorldTimeScale;
    }

    protected abstract void Tick(float dt);

    //====================================================
    // DAMAGE SYSTEM (UNIVERSAL)
    //====================================================
    // NOTE:
    // All damage is handled externally via CombatResolver.


    private void OnTriggerEnter(Collider other)
    {
        if (isDying) return;

        if (other.TryGetComponent<IDamageDealer>(out var dealer))
        {
            CombatResolver.DealDirectDamage(other.gameObject, gameObject, dealer.Damage);
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