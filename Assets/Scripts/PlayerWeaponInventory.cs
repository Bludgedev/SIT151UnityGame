using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponInventory : MonoBehaviour
{
    [System.Serializable]
    public class AmmoData
    {
        public AmmoType type;
        public int current;
        public int max;
    }

    private Dictionary<AmmoType, AmmoData> ammoDict;

    [SerializeField] private List<AmmoData> ammoList = new();

    public event Action OnAmmoChanged;

    private void Awake()
    {
        ammoDict = new Dictionary<AmmoType, AmmoData>();

        foreach (var a in ammoList)
        {
            if (ammoDict.ContainsKey(a.type))
            {
                Debug.LogWarning($"Duplicate ammo type detected: {a.type}");
                continue;
            }

            a.current = Mathf.Clamp(a.current, 0, a.max);
            ammoDict.Add(a.type, a);
        }
    }

    private AmmoData Get(AmmoType type)
    {
        ammoDict.TryGetValue(type, out var data);
        return data;
    }

    public int GetCurrentAmmo(AmmoType type)
    {
        return Get(type)?.current ?? 0;
    }

    public int GetMaxAmmo(AmmoType type)
    {
        return Get(type)?.max ?? 0;
    }

    public void AddAmmo(AmmoType type, int amount)
    {
        var data = Get(type);
        if (data == null)
        {
            Debug.LogWarning($"Ammo type {type} not defined in inventory.");
            return;
        }

        data.current = Mathf.Clamp(data.current + amount, 0, data.max);

        OnAmmoChanged?.Invoke();
    }

    public bool ConsumeAmmo(AmmoType type, int amount)
    {
        var data = Get(type);

        if (data == null || data.current < amount)
            return false;

        data.current -= amount;

        OnAmmoChanged?.Invoke();
        return true;
    }

    public bool HasAmmo(AmmoType type, int amount)
    {
        var data = Get(type);
        return data != null && data.current >= amount;
    }
}