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

    protected virtual void Update()
    {
        HandleRotation();
    }


    protected virtual void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            transform.position = Vector3.Lerp(
                transform.position,
                other.transform.position,
                attractionSpeed * Time.deltaTime
            );
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Collect(other.gameObject);
        }
    }

    protected abstract void Collect(GameObject player);

    private void HandleRotation()
    {
        if (!rotatePickup)
            return;

        transform.Rotate(
            0f,
            0f,
            rotationSpeed * Time.deltaTime
        );
    }

}