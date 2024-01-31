using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestController : MonoBehaviour
{
    public bool isOpenned = false;
    void Start()
    {
        if (isOpenned) 
        {
            open();
        }
    }

    public void open() 
    {    
        isOpenned = true;
        this.GetComponent<SpriteRenderer>().sortingOrder = 0;
    }
}
