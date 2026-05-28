using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySnake : MonoBehaviour, IDamageable
{
    [Header("Segments")]
    public Transform head;
    [SerializeField] private int bodySegments = 5;

    [Header("Prefabs")]
    [SerializeField] private GameObject bodyPrefab;
    [SerializeField] private GameObject tailPrefab;

    private Transform tail;

    [Header("Movement")]
    public int moveDirection = 1;
    public float waveAmplitude = 2f;
    public float waveFrequency = 2f;
    public float segmentSpacing = 0.5f;
    public float speed = 2f;

    [Header("Enemy Stats")]
    [SerializeField] private int headBounty = 25;
    [SerializeField] private bool verbose = false;
    [SerializeField] private RuntimeAnimatorController explosionController; 

    private bool isDying = false;

    [Header("Segment HP")]
    public float headHP = 12;
    public float bodyHP = 6;

    private List<Transform> allSegments = new List<Transform>();
    private List<Vector3> positionHistory = new List<Vector3>();

    private float baseY;

    private void Start()
    {
        if (moveDirection < 0)
        {
            head.localScale = new Vector3(-1, 1, 1);
        }

        BuildSnake();
        SetupSegments();
        baseY = head.position.y;

        positionHistory.Add(head.position);
    }

    private void FixedUpdate()
    {
        Move(Time.fixedDeltaTime);
    }

    public void Move(float dt)
    {
        MoveHead(dt);
        UpdateHistory();
        FollowSegments();
    }


    public void SetDirection(int dir)
    {
        moveDirection = Mathf.Clamp(dir, -1, 1);
    }

    private void MoveHead(float dt)
    {
        Vector3 pos = head.position;

        pos.x += moveDirection * speed * dt;

        pos.y = baseY + Mathf.Sin(Time.fixedTime * waveFrequency) * waveAmplitude;

        Rigidbody rb = head.GetComponent<Rigidbody>();
        rb.MovePosition(pos);
    }

    private void UpdateHistory()
    {
        positionHistory.Insert(0, head.position);

        int maxHistory = allSegments.Count * 15;

        if (positionHistory.Count > maxHistory)
        {
            positionHistory.RemoveAt(positionHistory.Count - 1);
        }
    }

    private void FollowSegments()
    {
        float spacing = segmentSpacing;

        for (int i = 1; i < allSegments.Count; i++)
        {
            Transform prev = allSegments[i - 1];
            Transform seg = allSegments[i];

            Vector3 dir = (seg.position - prev.position).normalized;
            Vector3 desiredPos = prev.position - dir * spacing;

            Rigidbody rb = seg.GetComponent<Rigidbody>();
            rb.MovePosition(desiredPos);
        }
    }

    private void BuildSnake()
    {
        allSegments.Clear();

        allSegments.Add(head);

        Transform previous = head;

        // BODY
        for (int i = 0; i < bodySegments; i++)
        {
            GameObject body = Instantiate(
                bodyPrefab,
                previous.position - new Vector3(segmentSpacing, 0f, 0f),
                Quaternion.identity
            );

            allSegments.Add(body.transform);
            previous = body.transform;
        }

        // TAIL (after body loop)
        CreateTail(previous);
    }

    private void CreateTail(Transform previous)
    {
        GameObject tailObj = Instantiate(
            tailPrefab,
            previous.position - new Vector3(segmentSpacing, 0f, 0f),
            Quaternion.identity
        );

        allSegments.Add(tailObj.transform);
        tail = tailObj.transform;
    }

    private void SetupSegments()
    {
        for (int i = 0; i < allSegments.Count; i++)
        {
            SnakeSegment seg =
                allSegments[i].GetComponent<SnakeSegment>();

            if (seg != null)
            {
                seg.owner = this;
                seg.segmentIndex = i;

                // Head HP
                if (i == 0)
                    seg.maxHP = 5;
                else
                    seg.maxHP = 3;
            }
        }
    }


    public void TakeDamage(float damage)
    {
        if (isDying) return;

        // Apply to head segment first (entry point damage)
        SnakeSegment headSeg = head.GetComponent<SnakeSegment>();

        if (headSeg != null)
        {
            headSeg.TakeDamage((int)damage);
            return;
        }
    }


    public void DestroyFromIndex(int index)
    {
        for (int i = allSegments.Count - 1; i >= index; i--)
        {
            Destroy(allSegments[i].gameObject);
        }

        allSegments.RemoveRange(
            index,
            allSegments.Count - index
        );

        // Entire snake dead
        if (allSegments.Count == 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"[ENEMY] DEATH: {name}");

        isDying = true;
        ScoreManager.Instance?.AddScore(headBounty);

        AudioManager.Instance?.PlayExplosion();

        GameObject fx = new GameObject("EnemyExplosionFX");
        fx.transform.position = transform.position;

        var sr = fx.AddComponent<SpriteRenderer>();
        var anim = fx.AddComponent<Animator>();

        anim.runtimeAnimatorController = explosionController;
        sr.sortingOrder = 10;

        Destroy(fx, 1f);

        DestroyFromIndex(0);

    }

}