using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameVictoryMenu : MonoBehaviour
{
    [Header("Scene Names")]
    public string gameplaySceneName = "GameScene";
    public string mainMenuSceneName = "MainMenu";

    [Header("UI References")]
    [SerializeField] private GameObject rootPanel;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI killsText;
    [SerializeField] private TextMeshProUGUI pickupsText;
    [SerializeField] private TextMeshProUGUI timeText;

    

    private void Awake()
    {

    }

    public void Show(LevelResults results)
    {
        rootPanel.SetActive(true);

        scoreText.text = $"Score: {results.score}";
        killsText.text = $"Kills: {results.kills}";
        pickupsText.text = $"Pickups: {results.pickups}";
        timeText.text = $"Time: {results.survivalTime:F1}s";
    }

    public void Hide()
    {
        rootPanel.SetActive(false);
    }

    // -------------------------
    // Button Handlers
    // -------------------------

    public void NextLevel()
    {
        Debug.Log("[VictoryScreen] Next Level pressed");
        //OnNextLevel?.Invoke();
    }

    public void Restart()
    {
        Debug.Log("[VictoryScreen] Restart pressed");
        //ShipGameMode.Instance?.RestartLevel();
    }

    public void SaveGame()
    {
        Debug.Log("[VictoryScreen] Save Game pressed");
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;

        MusicManager.Instance.ResetAudioState();

        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }


}