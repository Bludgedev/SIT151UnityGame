using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MusicManager;
using UnityEngine.SceneManagement;


public class PauseController : MonoBehaviour
{
    private OptionsMenu optionsMenu;

    private enum MenuState { Gameplay, Pause, Options }
    private MenuState currentMenu = MenuState.Gameplay;

    public static bool IsPaused => MusicManager.Instance.CurrentState == GameState.Pause;

    [Header("UI References")]
    public GameObject pauseMenuPanel;
    public GameObject hudPanel;

    void Update()
    {
        if (InputManager.Instance == null) return;

        if (!InputManager.Instance.PausePressed) return;

        if (currentMenu == MenuState.Options)
        {
            optionsMenu?.Back();
            return;
        }

        TogglePause();
        
    }

    private void Awake()
    {
        optionsMenu = FindAnyObjectByType<OptionsMenu>();
        if(optionsMenu != null)
        {
            optionsMenu.OnBackPressedEvent += HandleOptionsBack;
            optionsMenu.OnSavePressedEvent += HandleOptionsSave;
        }
    }

    private void TogglePause()
    {
        if (currentMenu == MenuState.Pause) Resume();
        else Pause();
    }

    public void OnResumePressed()
    {
        if (Time.timeScale == 0f)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    public void OnOptionsPressed()
    {
        currentMenu = MenuState.Options;
        pauseMenuPanel?.SetActive(false);
        optionsMenu?.gameObject.SetActive(true);
    }

    private void HandleOptionsBack()
    {
        currentMenu = MenuState.Pause;
        optionsMenu?.gameObject.SetActive(false);
        pauseMenuPanel?.SetActive(true);
    }

    private void HandleOptionsSave()
    {

    }

    public void OnQuitPressed()
    {
        Time.timeScale = 1f; // important reset

    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }

    public void Pause()
    {
        MusicManager.Instance.ApplyState(GameState.Pause);
        currentMenu = MenuState.Pause;

        Time.timeScale = 0f;

        pauseMenuPanel?.SetActive(true);
        optionsMenu?.gameObject.SetActive(false);
        hudPanel?.SetActive(false);
         
    }


    public void Resume()
    {
        MusicManager.Instance.ApplyState(GameState.Gameplay);
        currentMenu = MenuState.Gameplay;
        Time.timeScale = 1f;

        pauseMenuPanel?.SetActive(false);
        optionsMenu?.gameObject.SetActive(false);
        hudPanel?.SetActive(true);
               
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f; // IMPORTANT: reset time first

        // optional cleanup
        MusicManager.Instance.ResetAudioState();

        SceneManager.LoadScene("MainMenu");
    }



    private enum InputMode
    {
        Gameplay,
        UI
    }

    private void SetPaused(bool paused)
    {
        currentMenu = paused ? MenuState.Pause : MenuState.Gameplay;

        Time.timeScale = paused ? 0f : 1f;

        pauseMenuPanel?.SetActive(paused);
        optionsMenu?.gameObject.SetActive(false);
        hudPanel?.SetActive(!paused);

        if (paused)
        {
            MusicManager.Instance.ApplyState(GameState.Pause);
        }
        else
        {
            MusicManager.Instance.StopAllSecondaryMusic();
            MusicManager.Instance.ApplyState(GameState.Gameplay);
        }
    }


}