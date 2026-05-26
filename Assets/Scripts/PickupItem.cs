using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    private PickupBase pickup;

    private void Awake()
    {
        pickup = GetComponentInParent<PickupBase>();

        if (pickup == null)
        {
            Debug.LogError("No PickupBase attached to: " + gameObject.name);
        }

        //Debug.Log("ROOT: " + transform.root.name);

        var comps = transform.root.GetComponents<MonoBehaviour>();
        foreach (var c in comps)
        {
            //Debug.Log("ROOT COMPONENT: " + c.GetType().Name);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("TouchSphere triggered with: " + other.name);

        if (!other.CompareTag("Player"))
            return;

        if (pickup == null)
        {
            Debug.LogError("PickupBase reference missing on " + gameObject.name);
            return;
        }

        //Debug.Log("Collecting " + gameObject.name);

        pickup.TriggerCollect(other.gameObject);
    }
}