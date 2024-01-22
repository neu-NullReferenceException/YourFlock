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
    [Tooltip("Ha image helyére kell bejelyettesíteni példányosítás helyett")]
    public Sprite itemSprite;
    public GameObject prefab;
    public AudioClip audio;

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