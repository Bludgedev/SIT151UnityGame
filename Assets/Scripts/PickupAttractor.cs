using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupAttractor : MonoBehaviour
{
    [SerializeField] private float attractionSpeed = 0.01f;

    private Transform player;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        player = other.transform;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        player = null;
    }

    private void Update()
    {
        if (player == null)
            return;

        transform.parent.position = Vector3.Lerp(
            transform.parent.position,
            player.position,
            attractionSpeed
        );
    }
}