using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySnake : MonoBehaviour, IDamageable
{
    [Header("Segments")]
    public Transform head;
    public List<Transform> bodySegments = new List<Transform>();
    public Transform tail;

    [Header("Movement")]
    public int moveDirection = 1;
    public float waveAmplitude = 2f;
    public float waveFrequency = 2f;
    public float segmentSpacing = 0.5f;
    public float speed = 2f;

    [Header("Segment HP")]
    public int headHP = 12;
    public int bodyHP = 6;

    private List<Transform> allSegments = new List<Transform>();
    private List<Vector3> positionHistory = new List<Vector3>();

    private float baseY;

    private void Start()
    {
        BuildSegmentList();
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

    private void MoveHead(float dt)
    {
        Vector3 pos = head.position;

        pos.x += moveDirection * speed * dt;

        pos.y = baseY + Mathf.Sin(Time.time * waveFrequency) * waveAmplitude;

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
        for (int i = 1; i < allSegments.Count; i++)
        {
            int historyIndex = i * 10;

            if (historyIndex < positionHistory.Count)
            {
                Rigidbody rb = allSegments[i].GetComponent<Rigidbody>();
                rb.MovePosition(positionHistory[historyIndex]);
            }
        }
    }

    private void BuildSegmentList()
    {
        allSegments.Add(head);

        foreach (Transform body in bodySegments)
        {
            allSegments.Add(body);
        }

        if (tail != null)
        {
            allSegments.Add(tail);
        }
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
        //  we fix this later
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

    public void Die()
    {
        
    }

}