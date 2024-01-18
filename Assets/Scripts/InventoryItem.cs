using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewInventoryItem", menuName = "NewInventoryItem")]
public class InventoryItem : ScriptableObject
{
    public string name;
    public ItemType type;
    [Multiline]
    public string description;
    public Sprite itemIcon;
    public GameObject prefab;

    public int cost;
    public WeaponStats weaponStats;
}

public enum ItemType
{
    Material,
    Food,
    Weapon,
}

[System.Serializable]
public class WeaponStats
{
    public bool usesAmmo = false;
    public float range = 1f;
    public float attackDelay = 1f;
    public int damage;
}