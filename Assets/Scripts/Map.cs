using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "NewMap", menuName = "New Map Data")]
public class Map : ScriptableObject
{
    public string name;
    public int sceneBuildIndex;
    [Multiline]
    public string description;
    public Sprite image;
}
