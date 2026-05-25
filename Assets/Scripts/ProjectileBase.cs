using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectileBase : MonoBehaviour
{
    protected Vector3 velocity;

    protected virtual void Update()
    {
        Tick(GetDeltaTime());
    }

    protected virtual float GetDeltaTime()
    {
        if (TimeDilationSystem.Instance == null)
            return Time.deltaTime;

        return Time.deltaTime * TimeDilationSystem.Instance.BulletTimeScale;
    }

    protected abstract void Tick(float dt);
}
