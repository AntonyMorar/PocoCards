using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SerializationManager
{
    public static bool Save(object saveData)
    {
        string savePath = Application.persistentDataPath + "/saves";
        BinaryFormatter formatter = GetBinaryFormatter();

        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        string saveFile = savePath + "/Store1.save";
        FileStream file = File.Create(saveFile);
        formatter.Serialize(file, saveData);
        file.Close();
        return true;
    }

    public static object Load()
    {
        string savePath = Application.persistentDataPath + "/saves/Store1.save";
        if (!File.Exists(savePath)) return null;

        BinaryFormatter formatter = GetBinaryFormatter();
        FileStream file = File.Open(savePath, FileMode.Open);

        try
        {
            object save = formatter.Deserialize(file);
            file.Close();
            return save;
        }
        catch
        {
            Debug.LogErrorFormat("Failed to load file at {0}", savePath);
            file.Close();
            return null;
        }
    }

    private static BinaryFormatter GetBinaryFormatter()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        return formatter;
    }
}