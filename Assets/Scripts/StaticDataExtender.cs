using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticDataExtender : MonoBehaviour
{
    public Sprite[] femalePorfilePics;
    public Sprite[] malePorfilePics;
    public void InjectData()
    {
        StaticDataProvider.profilePicsFemale = femalePorfilePics;
        StaticDataProvider.profilePicsMale = malePorfilePics;   
    }

}
