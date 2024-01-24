using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestory : MonoBehaviour
{
    public float time = 5f;

    void Awake()
    {
        Destroy(gameObject, time);
    }
}
