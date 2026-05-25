using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeDilationFX : MonoBehaviour
{
    [SerializeField] private Transform ripplePrefab;
    [SerializeField] private Transform player;

    private void Update()
    {
        if (TimeDilationSystem.Instance == null)
            return;

        float world = TimeDilationSystem.Instance.WorldTimeScale;

        if (world < 0.95f)
        {
            // simple placeholder pulse
            if (Random.value < 0.02f)
            {
                Instantiate(ripplePrefab, player.position, Quaternion.identity);
            }
        }
    }
}