using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Requirement : MonoBehaviour
{
    public Measure requiredLaw;

    public bool IsMet()
    {
        if (requiredLaw)
        {
            if (StaticDataProvider.passedLaws.Contains(requiredLaw))
            {
                return true;
            }
        }
        return false;
    }

}
