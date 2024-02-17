using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveLoader
{
    static IFormatter formatter = new BinaryFormatter();
    // To save an object give it the object as the first parameter and then the filename with a file extension like .obj
    public static void Save(object obj, String filename) 
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
    public static object Load(object obj,String filename)
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

    public static bool isSaveFile(object obj, String filename) 
    {
        Type myType = obj.GetType();
        string dataPath = Application.persistentDataPath + "/" + myType.Name + filename;
        obj = null;
        return File.Exists(dataPath);
    }

    public static void DeleteFile(object obj, String filename)
    {
        if (isSaveFile(obj, filename))
        {
            Type myType = obj.GetType();
            string dataPath = Application.persistentDataPath + "/" + myType.Name + filename;
            obj = null;
            File.Delete(dataPath);
        }
    }
}
