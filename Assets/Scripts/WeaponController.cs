using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Weapons")]
    [SerializeField] private List<MonoBehaviour> weaponBehaviours;

    public event Action<IWeapon> OnWeaponChanged;

    private List<IWeapon> weapons = new List<IWeapon>();
    private int currentIndex = 0;

    public IWeapon CurrentWeapon
    {
        get
        {
            if (weapons == null || weapons.Count == 0)
                return null;

            return weapons[currentIndex];
        }
    }

    private void Awake()
    {
        weapons.Clear();

        if (weaponBehaviours == null || weaponBehaviours.Count == 0)
        {
            Debug.LogError("WeaponController: No weapons assigned in Inspector!");
            return;
        }

        foreach (var w in weaponBehaviours)
        {
            if (w is IWeapon weapon)
            {
                weapons.Add(weapon);
            }
            else
            {
                Debug.LogError($"{w.name} does NOT implement IWeapon!");
            }
        }

        if (weapons.Count > 0)
        {
            currentIndex = 0;
            OnWeaponChanged?.Invoke(CurrentWeapon);
        }
    }

    private void Update()
    {
        if (weapons.Count == 0)
            return;

        CurrentWeapon?.Tick();
    }

    public void FireCurrent()
    {
        if (CurrentWeapon == null)
            return;

        if (CurrentWeapon.CanFire)
            CurrentWeapon.Fire();
    }

    public void SwitchWeapon(int direction)
    {
        if (weapons.Count == 0)
            return;

        currentIndex += direction;

        if (currentIndex < 0)
            currentIndex = weapons.Count - 1;

        if (currentIndex >= weapons.Count)
            currentIndex = 0;

        OnWeaponChanged?.Invoke(CurrentWeapon);
    }
}