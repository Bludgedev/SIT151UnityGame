using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAmmoReceiver
{
    AmmoType AmmoType { get; }
    void AddAmmo(int amount);
}
