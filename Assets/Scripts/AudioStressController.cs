using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class AudioStressController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private AudioMixer mixer;

    [Header("Heartbeat")]
    [SerializeField] private float heartbeatStartThreshold = 0.5f;
    [SerializeField] private float minDelay = 0.25f;
    [SerializeField] private float maxDelay = 1.2f;

    [Header("Lowpass")]
    [SerializeField] private AudioLowPassFilter musicFilter;
    [SerializeField] private float normalCutoff = 22000f;
    [SerializeField] private float stressedCutoff = 500f;
    
    [Header("Game Over")]
    [SerializeField] private float postDeathHeartbeatTime = 2.5f;

    private AudioSource heartbeatSource;
    [SerializeField] private AudioClip heartbeatClip;
    private Coroutine heartbeatRoutine;
    private float intensity;
    private bool isDeadSequence;
    public static AudioStressController Instance;


    private void Awake()
    {
        heartbeatSource = gameObject.AddComponent<AudioSource>();
        heartbeatSource.playOnAwake = false;
        heartbeatSource.loop = false;

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        playerHealth.OnHealthPercentChanged += HandleHealth;
        playerHealth.OnDeath += HandleDeath;
        playerHealth.OnHealed += HandleHeal;
    }

    private void OnDisable()
    {
        playerHealth.OnHealthPercentChanged -= HandleHealth;
        playerHealth.OnDeath -= HandleDeath;
        playerHealth.OnHealed -= HandleHeal;
    }

    //====================================================
    // HEALTH INPUT
    //====================================================

    private void HandleHealth(float percent)
    {
        if (isDeadSequence)
            return;

        if (percent <= heartbeatStartThreshold)
        {
            intensity = Mathf.InverseLerp(heartbeatStartThreshold, 0f, percent);

            if (heartbeatRoutine == null)
                heartbeatRoutine = StartCoroutine(HeartbeatLoop());
        }
        else
        {
            StopHeartbeat();
        }

        ApplyStressToMusic(percent);
    }

    private void HandleHeal()
    {
        StopHeartbeat();
        UpdateLowPass(1f);
    }

    //====================================================
    // HEARTBEAT LOOP
    //====================================================

    private IEnumerator HeartbeatLoop()
    {
        while (!isDeadSequence)
        {
            float volume = Mathf.Lerp(0.2f, 1f, intensity);

            heartbeatSource.PlayOneShot(heartbeatClip, volume);

            float delay = Mathf.Lerp(maxDelay, minDelay, intensity);

            yield return new WaitForSeconds(delay);
        }

        heartbeatRoutine = null;
    }

    private void StopHeartbeat()
    {
        if (heartbeatRoutine != null)
        {
            StopCoroutine(heartbeatRoutine);
            heartbeatRoutine = null;
        }
    }

    //====================================================
    // LOW PASS FILTER
    //====================================================

    private void UpdateLowPass(float percent)
    {
        if (musicFilter == null)
            return;

        float t = Mathf.InverseLerp(heartbeatStartThreshold, 0f, percent);
        float cutoff = Mathf.Lerp(normalCutoff, stressedCutoff, t);

        musicFilter.cutoffFrequency = cutoff;
    }

    private void ApplyStressToMusic(float percent)
    {
        MusicManager.Instance.SetStress(1f - percent);
    }

    //====================================================
    // DEATH SEQUENCE
    //====================================================

    private void HandleDeath()
    {
        if (isDeadSequence)
            return;

        isDeadSequence = true;

        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        // keep heartbeat for cinematic tension
        float timer = 0f;

        while (timer < postDeathHeartbeatTime)
        {
            float volume = Mathf.Lerp(1f, 0.3f, timer / postDeathHeartbeatTime);

            heartbeatSource.PlayOneShot(heartbeatClip, volume);

            yield return new WaitForSeconds(0.3f);

            timer += 0.3f;
        }

       
    }

    public void ResetHeartbeat()
    {
        if (heartbeatRoutine != null)
        {
            StopCoroutine(heartbeatRoutine);
            heartbeatRoutine = null;
        }

        intensity = 0f;

        if (heartbeatSource != null)
            heartbeatSource.Stop();
    }

    public void SetActive(bool active)
    {
        if (!active)
        {
            ResetHeartbeat();
            StopHeartbeat();
        }
    }


}