using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioStateController : MonoBehaviour
{
    public static AudioStateController Instance;

    public bool IsPaused { get; private set; }
    public bool IsGameOver { get; private set; }

    public bool IsLowHealth { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void SetPaused(bool value)
    {
        IsPaused = value;
        ApplyState();
    }

    public void SetGameOver(bool value)
    {
        IsGameOver = value;
        ApplyState();
    }

    public void SetLowHealth(bool value)
    {
        IsLowHealth = value;
        ApplyState();
    }

    private void ApplyState()
    {
        // This is where you coordinate systems
        MusicManager.Instance.SetPaused(IsPaused || IsGameOver);
        AudioStressController.Instance.SetActive(!IsPaused);
    }
}