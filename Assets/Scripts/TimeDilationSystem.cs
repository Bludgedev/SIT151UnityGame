using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeDilationSystem : MonoBehaviour
{
    public static TimeDilationSystem Instance;

    [Header("Current Time Scales")]
    [SerializeField] private float worldTimeScale = 1f;
    [SerializeField] private float bulletTimeScale = 1f;
    [SerializeField] private float playerTimeScale = 1f;

    [Header("Game Clock")]
    [SerializeField] private float gameTime;

    public float WorldTimeScale => worldTimeScale;
    public float BulletTimeScale => bulletTimeScale;
    public float PlayerTimeScale => playerTimeScale;
    public float GameTime => gameTime;

    private enum State
    {
        Idle,
        Entering,
        Sustained,
        Exiting
    }

    private State state = State.Idle;

    private float duration;
    private float timer;

    private float sustainStart;
    private float sustainEnd;

    private float startWorld, startBullet, startPlayer;
    private float targetWorld, targetBullet, targetPlayer;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        HandleState(dt);

        gameTime += dt * worldTimeScale;
    }

    private void HandleState(float dt)
    {
        if (state == State.Idle)
            return;

        timer += dt;

        float t = timer / duration;

        switch (state)
        {
            case State.Entering:
                ApplyLerp(startWorld, targetWorld, startBullet, targetBullet, startPlayer, targetPlayer, t);

                if (timer >= sustainStart)
                    BeginSustain();
                break;

            case State.Sustained:
                ApplyHold();

                if (timer >= sustainEnd)
                    BeginExit();
                break;

            case State.Exiting:
                float exitT = Mathf.InverseLerp(sustainEnd, duration, timer);

                ApplyLerp(targetWorld, 1f, targetBullet, 1f, targetPlayer, 1f, exitT);

                if (timer >= duration)
                    Reset();
                break;
        }
    }

    private void ApplyLerp(
        float w0, float w1,
        float b0, float b1,
        float p0, float p1,
        float t)
    {
        worldTimeScale = Mathf.Lerp(w0, w1, t);
        bulletTimeScale = Mathf.Lerp(b0, b1, t);
        playerTimeScale = Mathf.Lerp(p0, p1, t);
    }

    private void ApplyHold()
    {
        worldTimeScale = targetWorld;
        bulletTimeScale = targetBullet;
        playerTimeScale = targetPlayer;
    }

    public void Trigger(
        float duration,
        float worldScale,
        float bulletScale,
        float playerScale)
    {
        this.duration = duration;
        this.timer = 0f;

        this.sustainStart = duration * 0.1f;   // ~1s if 10s total
        this.sustainEnd = duration * 0.875f;   // your 8.75s idea

        startWorld = worldTimeScale;
        startBullet = bulletTimeScale;
        startPlayer = playerTimeScale;

        targetWorld = worldScale;
        targetBullet = bulletScale;
        targetPlayer = playerScale;

        state = State.Entering;
    }

    private void BeginSustain()
    {
        state = State.Sustained;
    }

    private void BeginExit()
    {
        state = State.Exiting;
    }

    private void Reset()
    {
        state = State.Idle;

        worldTimeScale = 1f;
        bulletTimeScale = 1f;
        playerTimeScale = 1f;
    }
}