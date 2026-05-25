using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBrute : EnemyBase
{
    protected override void Tick(float dt)
    {
        transform.position += transform.forward * speed * dt;
    }

}
