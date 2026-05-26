using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PhysicsSceneDebug : MonoBehaviour
{
    [SerializeField] private bool logOnAwake = true;
    [SerializeField] private bool logOnStart = false;

    private void Awake()
    {
        if (logOnAwake)
            LogState("AWAKE");
    }

    private void Start()
    {
        if (logOnStart)
            LogState("START");
    }

    private void LogState(string phase)
    {
        var scene = gameObject.scene;
        var ps = scene.GetPhysicsScene();

        Debug.Log($"========== [PHYS DEBUG {phase}] ==========");
        Debug.Log($"Object: {name} (ID: {GetInstanceID()})");
        Debug.Log($"Scene: {scene.name}");
        Debug.Log($"PhysicsScene: {ps}");
        Debug.Log($"PhysicsScene Hash: {ps.GetHashCode()}");
        Debug.Log($"PhysicsScene Valid: {ps.IsValid()}");
        Debug.Log($"Is Default Physics Scene: {scene == UnityEngine.SceneManagement.SceneManager.GetActiveScene()}");

        var rb = GetComponent<Rigidbody>();
        var col = GetComponent<Collider>();

        Debug.Log($"Rigidbody: {(rb ? "YES" : "NO")}");
        Debug.Log($"Collider: {(col ? "YES" : "NO")}");

        if (rb)
        {
            Debug.Log($"RB Kinematic: {rb.isKinematic}");
            Debug.Log($"RB Use Gravity: {rb.useGravity}");
        }

        if (col)
        {
            Debug.Log($"Collider Enabled: {col.enabled}");
            Debug.Log($"Collider IsTrigger: {col.isTrigger}");
        }

        Debug.Log($"Layer: {LayerMask.LayerToName(gameObject.layer)} ({gameObject.layer})");
        Debug.Log($"===========================================");
    }
}