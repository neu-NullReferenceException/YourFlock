using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Follower
{
    public string name;
    public int age;

    public int maxHealth = 100;
    public int health = 100;
    public int strength = 0;        // ennyi * 3 plusz dmg a melee fegyverekhez
    public int perception = 0;      // ennyi * 2 plusz dmg a lõfegyverekhez
    public int mentalState = 100;

    public int food = 3000;
    public int yesterdayFood = 3000;
    public int dayliRation = 1000;

    public Sprite profilePic;
    public GameObject prefab;
    public InventoryItem weapon;

    public string story;

    public int CalculateDamage()
    {
        if (weapon.weaponStats.usesAmmo)
        {
            return weapon.weaponStats.damage + (perception * 2);
        }

        return weapon.weaponStats.damage + (strength * 3);
    }

    public Follower(string name, int age)
    {
        this.name = name;
        this.age = age;
        strength = Random.Range(0, 6);
        strength = Random.Range(0, 6);
    }
}
