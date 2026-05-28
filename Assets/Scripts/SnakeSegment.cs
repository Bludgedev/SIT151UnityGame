using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SnakeSegment : MonoBehaviour, IDamageable
{
    
    public int maxHP;

    [Header("Enemy Stats")]
    [SerializeField] private int bounty = 15;


    private float currentHP;

    [Header("Snake Data")]
    public int segmentIndex;
    public EnemySnake owner;

    [Header("Visuals")]
    public SpriteRenderer mainRenderer;
    public SpriteRenderer outlineRenderer;
    [SerializeField] private RuntimeAnimatorController explosionController;

    public float flashDuration = 0.1f;

    private bool isDead = false;

    private Color originalColor;

    void Start()
    {
        currentHP = maxHP;

        if (mainRenderer != null)
        {
            originalColor = mainRenderer.color;
        }

        if (outlineRenderer != null)
        {
            outlineRenderer.enabled = false;
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        currentHP -= damage;

        if (currentHP > 0)
        {
            StartCoroutine(DamageFlash());
        }
        else
        {
            isDead = true;

            Die();

        }
    }

    private IEnumerator DamageFlash()
    {
        // White flash
        if (mainRenderer != null)
        {
            mainRenderer.color = Color.white;
        }

        // Yellow outline
        if (outlineRenderer != null)
        {
            outlineRenderer.enabled = true;
            outlineRenderer.color = Color.yellow;
        }

        yield return new WaitForSeconds(flashDuration);

        // Restore
        if (mainRenderer != null)
        {
            mainRenderer.color = originalColor;
        }

        if (outlineRenderer != null)
        {
            outlineRenderer.enabled = false;
        }
    }

    private void Die()
    {
        if (isDead) return;
        Debug.Log($"[ENEMY] DEATH: {name}");

        isDead = true;

        ScoreManager.Instance?.AddScore(bounty);

      
        Explosion();


        if (owner != null)
        {
            owner.DestroyFromIndex(segmentIndex);
        }
    }
    private void Explosion() 
    {
        GameObject fx = new GameObject("SegmentExplosionFX");
        fx.transform.position = transform.position;

        var sr = fx.AddComponent<SpriteRenderer>();
        var anim = fx.AddComponent<Animator>();
        AudioManager.Instance?.PlayExplosion();
        anim.runtimeAnimatorController = explosionController;
        sr.sortingOrder = 10;

        Destroy(fx, 1f);
    }


}