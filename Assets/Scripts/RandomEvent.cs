using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRandomEvent", menuName = "NewRandomEvent")]
public class RandomEvent : ScriptableObject
{
    public DialogeElement dialoge;
    //public OutcomeEvent outcome;
}

//[CreateAssetMenu(fileName = "NewOutcomeEvent", menuName = "New Outcome Event")]
[System.Serializable]
public class OutcomeEvent //: ScriptableObject
{
    public OutcomeEventType type;
}

public enum OutcomeEventType
{
    None,
    SacrificeTheWeakest,
    SacrificeTheYungest,
    BanishTheWeakest,
}