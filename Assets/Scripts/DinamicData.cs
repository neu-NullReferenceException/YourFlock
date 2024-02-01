using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DinamicData : MonoBehaviour
{
    public int material = 10;
    public int radicality = 0;
    public int ammo;
    public int food = 12000;
    public int defaultFoodConsumption = 1000;
    public int defaultDeathMentalChange = 5;
    public int defaultDaylyRadicalityChange = 5;
    public int daysPassed = 1;
    public List<InventoryItem> inventoryItems = new List<InventoryItem>();
    public List<InventoryItem> craftingQueue = new List<InventoryItem>();
    public List<Follower> followers = new List<Follower>();
    public List<Follower> banishedPeople = new List<Follower>();
    public List<Follower> strikeTeam = new List<Follower>();
    public bool[] HostilityMatrix = {
        false,
        false,
        true,
        false,
        true,
    };

    public List<Measure> passedLaws = new List<Measure>();
    public bool lastStandAbilityUnlocked;
    public bool emergencyRationsUnlocked;
    public bool isLawAvailable = true;
    public float musicVolume = 1f;

    public DinamicData(int initialMaterial, int initialRadicality, int initialAmmo, int initialFood, int initialDaysPassed,
        int initialFoodConsumption, int initialMentalChange, int initialDaylyRadicalityChange, List<InventoryItem> initialInventoryItems,
        List<InventoryItem> initialCraftingQueue, List<Follower> initialFollowers, List<Follower> initialBanishedPeople, List<Follower> initialStriketeam,
        bool[] initialHostilityMatrix, List<Measure> initialPassedLaws, float initialMusicVolume, bool initialLastStandState, bool initialEmergencyRationsState)
    {
        material = initialMaterial;
        radicality = initialRadicality;
        ammo = initialAmmo;
        food = initialFood;
        daysPassed = initialDaysPassed;

        defaultFoodConsumption = initialFoodConsumption;
        defaultDeathMentalChange = initialMentalChange;
        defaultDaylyRadicalityChange = initialDaylyRadicalityChange;

        // Initialize other lists and arrays as needed
        inventoryItems = initialInventoryItems;
        craftingQueue = initialCraftingQueue;
        followers = initialFollowers;
        banishedPeople = initialBanishedPeople;
        strikeTeam = initialStriketeam;
        HostilityMatrix = initialHostilityMatrix;
        passedLaws = initialPassedLaws;

        // Initialize other boolean variables
        lastStandAbilityUnlocked = initialLastStandState;
        emergencyRationsUnlocked = initialEmergencyRationsState;
        isLawAvailable = true;
        musicVolume = initialMusicVolume;
    }

    public DinamicData() { }
}
