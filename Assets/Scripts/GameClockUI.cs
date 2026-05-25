using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class GameClockUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI clockText;

    void Update()
    {
        if (TimeDilationSystem.Instance == null)
            return;

        float t = TimeDilationSystem.Instance.GameTime;

        int minutes = Mathf.FloorToInt(t / 60f);
        int seconds = Mathf.FloorToInt(t % 60f);

        clockText.text = $"{minutes:00}:{seconds:00}";
    }
}

