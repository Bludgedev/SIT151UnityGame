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
}
