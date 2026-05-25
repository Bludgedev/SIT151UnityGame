using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{

    string WeaponName { get; }
    Sprite WeaponIcon { get; }
    AmmoType AmmoType { get; }
    bool UsesAmmo { get; }
    bool CanFire { get; }
    void Fire();
    void Tick();


}