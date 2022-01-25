using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

//[System.Serializable]
public class SaveData : MonoBehaviour
{
    /*
    public SaveData data;

    public string file = "player.txt";

    public int m_EngineClass;

    public void SaveJson()
    {
        string json = JsonUtility.ToJson(data);
        WriteToFile(file, json);
    }

    public void LoadJson()
    {
        data = new PlayerData();
        string json = ReadFromFile(file);
        JsonUtility.FromJsonOverwrite(json, data);
    }

    private void WriteToFile(string fileName, string json)
    {
        string path = GetFilePath(fileName);
        FileStream fileStream = new FileStream(path, FileMode.Create);

        using(StreamWriter writer = new StreamWriter(fileStream))
        {
            writer.Write(json);
        }


    }
    
    private void string ReadFromFile(string fileName)
    {
        string path = GetFilePath(fileName);
        if (file.Exists(path))
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string json = reader.ReadToEnd();
                return json;
            }

        }
        else
        {
            Debug.LogWarning("File not found!");
        }

        return "";

    }
    
    private void GetFilePath(string fileName)
    {
        return Application.persistentDataPath + "/" + fileName;

    }
    */

}





