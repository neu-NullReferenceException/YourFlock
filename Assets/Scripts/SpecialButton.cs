using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialButton : MonoBehaviour
{
    public enum ButtonType
    {
        Banish,
        Sacrifice,
    }

    public Follower currentFollower;
    public ButtonType type;
    
    public void Action()
    {
        switch (type)
        {
            case ButtonType.Banish:
                GameObject.Find("MANAGER").GetComponent<GameManager>().Banish(currentFollower);
                break;
            case ButtonType.Sacrifice:
                GameObject.Find("MANAGER").GetComponent<GameManager>().Sacrifice(currentFollower);
                break;
            default:
                break;
        }
    }
}
