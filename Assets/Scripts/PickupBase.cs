using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PickupBase : MonoBehaviour
{
    [Header("Attraction")]
    [SerializeField] private float attractionSpeed = 5f;

    [Header("Visuals")]
    [SerializeField] private bool rotatePickup = true;
    [SerializeField] private float rotationSpeed = 90f;

    private bool collected;

    protected virtual void Update()
    {
        HandleRotation();
    }

    protected virtual void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        transform.position = Vector3.Lerp(
            transform.position,
            other.transform.position,
            attractionSpeed * Time.deltaTime
        );
    }

    public void TriggerCollect(GameObject player)
    {
        if (collected)
            return;

        collected = true;

        Collect(player);

        Destroy(gameObject);
    }

    public abstract void Collect(GameObject player);

    private void HandleRotation()
    {
        if (!rotatePickup)
            return;

        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}