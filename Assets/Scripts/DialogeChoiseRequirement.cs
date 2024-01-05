using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogeChoiseRequirement
{
    public ConditionType conditionType;
    public int parameter;

    public bool IsMet()
    {
        switch (conditionType)
        {   
            case ConditionType.RadicalityGreaterThan:
                return (StaticDataProvider.radicality > parameter);
            case ConditionType.RadicalityLowerThan:
                return (StaticDataProvider.radicality < parameter);
            case ConditionType.LawPassed:
                return true;
            default:
                return true;
        }
    }
}

public enum ConditionType
{
    None,
    RadicalityGreaterThan,
    RadicalityLowerThan,
    LawPassed,  // x id vel ellátott law beiktatva (book of laws a frostpunkból)
}