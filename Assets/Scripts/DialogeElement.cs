using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogElement", menuName = "NewDialogElement")]
public class DialogeElement : ScriptableObject
{
    [Multiline]
    public string dialogText;
    //public DialogOptionModifyer modifyer;

    public DialogFollowUp[] followUps;
}

[System.Serializable]
public class DialogFollowUp
{
    [Multiline]
    public string answer;
    public DialogeElement nextDialogElement;
    public DialogeChoiseRequirement requirement;
    public DialogOptionModifyer modifyer;
}

[System.Serializable]
public class DialogOptionModifyer
{
    public int radicalityChange = 0;
    public int oneTimeMentalChange = 0;
    public int foodChange = 0;
    public int materialChange = 0;
    public OutcomeEvent outcomeEvent;
}