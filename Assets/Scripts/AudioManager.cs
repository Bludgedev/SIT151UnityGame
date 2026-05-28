using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Mixer Groups")]
    [SerializeField] private AudioMixerGroup sfxGroup;
    [SerializeField] private AudioMixerGroup uiGroup;

    [Header("Explosions")]
    [SerializeField] private AudioClip[] explosionClips;

    private const string SFX_KEY = "SFXVolume";
    private const string UI_KEY = "UIVolume";

    private float sfxVolume = 1f;
    private float uiVolume = 1f;

    //===========
    // INIT
    //===========

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadSettings();
    }

    private void LoadSettings()
    {
        sfxVolume = PlayerPrefs.GetFloat(SFX_KEY, 1f);
        uiVolume = PlayerPrefs.GetFloat(UI_KEY, 1f);
    }

    //====================================================
    // SFX
    //====================================================

    public void PlaySFX(AudioClip clip, float volumeMultiplier = 1f)
    {
        Debug.Log($"[AUDIO] Playing SFX: {clip.name} volume {sfxVolume}");
        if (clip == null) return;

        GameObject obj = new GameObject("SFX_" + clip.name);
        obj.transform.SetParent(transform);

        AudioSource src = obj.AddComponent<AudioSource>();

        src.outputAudioMixerGroup = sfxGroup;
        src.clip = clip;
        src.volume = sfxVolume * volumeMultiplier;
        src.spatialBlend = 0f;
        src.playOnAwake = false;

        src.Play();

        Destroy(obj, clip.length);
    }

    public void PlayExplosion()
    {
        if (explosionClips == null || explosionClips.Length == 0)
        {
            Debug.LogWarning("[AudioManager] No explosion clips assigned!");
            return;
        }

        AudioClip clip = explosionClips[Random.Range(0, explosionClips.Length)];

        PlaySFX(clip, Random.Range(0.9f, 1.1f));
    }

    //====================================================
    // UI
    //====================================================

    public void PlayUISound(AudioClip clip)
    {
        if (clip == null) return;

        GameObject obj = new GameObject("UI_" + clip.name);
        obj.transform.SetParent(transform);

        AudioSource src = obj.AddComponent<AudioSource>();

        src.outputAudioMixerGroup = uiGroup;
        src.clip = clip;
        src.volume = uiVolume;
        src.spatialBlend = 0f;
        src.playOnAwake = false;

        src.Play();

        Destroy(obj, clip.length);
    }

    //====================================================
    // VOLUME SETTINGS
    //====================================================

    public void SetSFXVolume(float value)
    {
        sfxVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(SFX_KEY, sfxVolume);
    }

    public void SetUIVolume(float value)
    {
        uiVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(UI_KEY, uiVolume);
    }

    public void SaveSettings()
    {
        PlayerPrefs.Save();
    }

    //====================================================
    // CLEANUP
    //====================================================

    public void StopAllAudio()
    {
        StopAllCoroutines();

        foreach (var src in GetComponentsInChildren<AudioSource>())
        {
            src.Stop();
            Destroy(src.gameObject);
        }
    }
}