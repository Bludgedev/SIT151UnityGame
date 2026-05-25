using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Weapons")]
    [SerializeField] private List<MonoBehaviour> weaponBehaviours;
    [SerializeField] private PlayerWeaponInventory inventory;

    public PlayerWeaponInventory Inventory => inventory;

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

        foreach (var w in weaponBehaviours)
        {
            if (w is IWeapon weapon)
            {
                weapons.Add(weapon);

                // Inject inventory immediately
                if (weapon is IInventoryUser user)
                {
                    user.SetInventory(inventory);
                }
            }
            else
            {
                Debug.LogError($"{w.name} does not implement IWeapon");
            }
        }
    }

    private void Start()
    {
        if (weapons.Count > 0)
            OnWeaponChanged?.Invoke(CurrentWeapon);
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