using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveLoader : MonoBehaviour
{
    IFormatter formatter = new BinaryFormatter();
    // To save an object give it the object as the first parameter and then the filename with a file extension like .obj
    public void Save(object obj, String filename) 
    {
        Type myType = obj.GetType();
        string dataPath = Application.persistentDataPath + "/" + myType.Name + filename;
        Stream stream = new FileStream(dataPath, FileMode.Create, FileAccess.Write);
        Debug.Log(dataPath);

        string json = JsonUtility.ToJson(obj);
        using (var streamWriter = new StreamWriter(stream)) 
        {
            streamWriter.Write(json);
            stream.Flush();
        }
        stream.Close();
        Debug.Log("Saved "+ myType.Name+"!");
    }

    // To load an object state give it the object as the first parameter and then the filename with a file extension
    public object Load(object obj,String filename)
    {
        Type myType = obj.GetType();
        string dataPath = Application.persistentDataPath + "/" + myType.Name + filename;
        Stream stream = new FileStream(dataPath, FileMode.Open, FileAccess.Read);
        Debug.Log(dataPath);
        using (var streamReader = new StreamReader(stream)) 
        { 
            string json = streamReader.ReadToEnd();
            JsonUtility.FromJsonOverwrite(json, obj);
            return obj;
        }
    }
}