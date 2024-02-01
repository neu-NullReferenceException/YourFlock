using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLaw", menuName = "New Measure")]
public class Measure : ScriptableObject
{
    public string name;
    public string description;
    public Measure requiredToUnlock;
    public Measure conflictsWith;

    //effects on pass
    public int radicalityChange;
    public int consumptionChange;
    public bool lastStandAbilityUnlocked;
    public bool emergencyRationsUnlocked;
    public int mentalChangeModifyer;
    public InventoryItem providedRecipe;
}
