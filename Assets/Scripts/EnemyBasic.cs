using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//  With the implementation of an enemy base super class
//  this means the script for the basic enemy can be condensed down.
public class EnemyBasic : EnemyBase
{
    
    //  this is tied to the slowmo Time dilation system.
    protected override void Tick(float dt)
    {
        transform.position += transform.forward * speed * dt;
    }

    //  Defines the movement of the ship, straight down 
    protected override void Move()
    {
        transform.position += Vector3.down * speed * Time.deltaTime;
    }

    private void Start()
    {
        var rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError($"{name}: NO RIGIDBODY");
            return;
        }

        Debug.Log(
            $"{name} RB STATE:\n" +
            $"isKinematic={rb.isKinematic}\n" +
            $"useGravity={rb.useGravity}\n" +
            $"constraints={rb.constraints}\n" +
            $"mass={rb.mass}"
        );
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"[COLLISION ENTER] {name} hit {collision.gameObject.name}");
    }

    private void OnCollisionStay(Collision collision)
    {
        Debug.Log($"[COLLISION STAY] {name} touching {collision.gameObject.name}");
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log($"[COLLISION EXIT] {name} left {collision.gameObject.name}");
    }


}
