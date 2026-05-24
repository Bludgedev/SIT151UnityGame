using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [Header("Audio Sliders")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider uiSlider;

    [Header("Value Texts")]
    public TMP_Text masterValueText;
    public TMP_Text musicValueText;
    public TMP_Text sfxValueText;
    public TMP_Text uiValueText;

    public AudioMixer baseMixer;

    [Header("Dialog")]
    public GameObject dialogPanel;

    private float initialMaster;
    private float initialMusic;
    private float initialSFX;
    private float initialUI;

    public event System.Action OnBackPressedEvent;
    public event System.Action OnSavePressedEvent;


    // -----------------------
    // Mixer parameter names
    // -----------------------
    private const string MASTER_PARAM = "Master";
    private const string MUSIC_PARAM = "Music";
    private const string SFX_PARAM = "Sound";
    private const string UI_PARAM = "UI";

    void Start()
    {
       
    }

    
    private void Awake()
    {
        // Attach listeners
        if (masterSlider != null) masterSlider.onValueChanged.AddListener(OnMasterChanged);
        if (musicSlider != null) musicSlider.onValueChanged.AddListener(OnMusicChanged);
        if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(OnSFXChanged);
        if (uiSlider != null) uiSlider.onValueChanged.AddListener(OnUIChanged);
    }

    private void OnEnable()
    {
        // Load saved values or defaults
        float masterVal = PlayerPrefs.GetFloat(MASTER_PARAM, 1f);
        float musicVal = PlayerPrefs.GetFloat(MUSIC_PARAM, 0.3f);
        float sfxVal = PlayerPrefs.GetFloat(SFX_PARAM, 0.5f);
        float uiVal = PlayerPrefs.GetFloat(UI_PARAM, 0.5f);

        // Store initial values for change detection
        initialMaster = masterVal;
        initialMusic = musicVal;
        initialSFX = sfxVal;
        initialUI = uiVal;

        // Apply to sliders
        if (masterSlider != null) { masterSlider.value = masterVal; UpdateMasterText(masterVal); ApplyMasterVolume(masterVal); }
        if (musicSlider != null) { musicSlider.value = musicVal; UpdateMusicText(musicVal); ApplyMusicVolume(musicVal); }
        if (sfxSlider != null) { sfxSlider.value = sfxVal; UpdateSFXText(sfxVal); ApplySFXVolume(sfxVal); }
        if (uiSlider != null) { uiSlider.value = uiVal; UpdateUIText(uiVal); ApplyUIVolume(uiVal); }
    }

    // -----------------------
    // Slider Callbacks
    // -----------------------
    public void OnMasterChanged(float value)
    {
        UpdateMasterText(value);
        ApplyMasterVolume(value);
        PlayerPrefs.SetFloat(MASTER_PARAM, value);
    }

    public void OnMusicChanged(float value)
    {
        UpdateMusicText(value);
        ApplyMusicVolume(value);
        PlayerPrefs.SetFloat(MUSIC_PARAM, value);
    }

    public void OnSFXChanged(float value)
    {
        UpdateSFXText(value);
        ApplySFXVolume(value);
        PlayerPrefs.SetFloat(SFX_PARAM, value);
    }

    public void OnUIChanged(float value)
    {
        UpdateUIText(value);
        ApplyUIVolume(value);
        PlayerPrefs.SetFloat(UI_PARAM, value);
    }

    // -----------------------
    // Apply to Mixer
    // -----------------------
    private void ApplyMasterVolume(float value) => SetMixerVolume(MASTER_PARAM, value);
    private void ApplyMusicVolume(float value) => SetMixerVolume(MUSIC_PARAM, value);
    private void ApplySFXVolume(float value) => SetMixerVolume(SFX_PARAM, value);
    private void ApplyUIVolume(float value) => SetMixerVolume(UI_PARAM, value);

    private void SetMixerVolume(string param, float value)
    {
        if (baseMixer != null)
        {
            // Convert 0–1 to decibels
            float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
            baseMixer.SetFloat(param, dB);
        }
    }

    // -----------------------
    // Update Text
    // -----------------------
    private void UpdateMasterText(float value) { if (masterValueText != null) masterValueText.text = Mathf.RoundToInt(value * 100) + "%"; }
    private void UpdateMusicText(float value) { if (musicValueText != null) musicValueText.text = Mathf.RoundToInt(value * 100) + "%"; }
    private void UpdateSFXText(float value) { if (sfxValueText != null) sfxValueText.text = Mathf.RoundToInt(value * 100) + "%"; }
    private void UpdateUIText(float value) { if (uiValueText != null) uiValueText.text = Mathf.RoundToInt(value * 100) + "%"; }

    //-------------------------
    //   Save settings
    //-------------------------
    public void Save()
    {
        PlayerPrefs.SetFloat(MASTER_PARAM, masterSlider.value);
        PlayerPrefs.SetFloat(MUSIC_PARAM, musicSlider.value);
        PlayerPrefs.SetFloat(SFX_PARAM, sfxSlider.value);
        PlayerPrefs.SetFloat(UI_PARAM, uiSlider.value);
        PlayerPrefs.Save();

        // Apply immediately
        ApplyMasterVolume(masterSlider.value);
        ApplyMusicVolume(musicSlider.value);
        ApplySFXVolume(sfxSlider.value);
        ApplyUIVolume(uiSlider.value);

        // Update initial values
        initialMaster = masterSlider.value;
        initialMusic = musicSlider.value;
        initialSFX = sfxSlider.value;
        initialUI = uiSlider.value;

        // calls the save event
        OnSavePressedEvent?.Invoke();
    }


    public void Back()
    {
        if (HasChanges())
        {
            dialogPanel.SetActive(true); // Show prompt to save or quit without changes
        }
        else
        {
            OnBackPressedEvent?.Invoke(); // No changes, just close
        }
    }

    private bool HasChanges()
    {
        return masterSlider.value != initialMaster ||
               musicSlider.value != initialMusic ||
               sfxSlider.value != initialSFX ||
               uiSlider.value != initialUI;
    }

    public void OnDialogSaveAndBack()
    {
        Save();
        gameObject.SetActive(false);
    }

    public void OnDialogBackWithoutSaving()
    {
        gameObject.SetActive(false);
    }

    public void OnDialogCancel()
    {
        dialogPanel.SetActive(false);
    }

}