using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{

    string WeaponName { get; }
    Sprite WeaponIcon { get; }
    bool UsesAmmo { get; }
    int CurrentAmmo { get; }
    int MaxAmmo { get; }
    bool CanFire { get; }
    void Fire();
    void Tick();


}