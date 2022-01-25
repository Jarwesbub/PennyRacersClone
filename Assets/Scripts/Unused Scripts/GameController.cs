using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour//, ISaveable
{
    /*
    int EngineClass;
    
    private static void SaveJsonData(GameController a_GameController)
    {
        SaveData sd = new SaveData();
        a_GameController.PopulateSaveData(sd);

        
        if (FileManager.WriteToFile("SaveData.dat", sd.ToJson()))
        {
            Debug.Log("Save succesful");

        }
        
    }

    public void PopulateSaveData(SaveData a_SaveData)
    {
        a_SaveData.m_EngineClass = EngineClass;

    }

    private static void LoadJsonData(GameController a_GameController)
    {
        
        if (FileManager.LoadFromFile("SaveData.dat", out var json))
        {
            SaveData sd = new SaveData();
            sd.LoadFromJson(json);

            a_GameController.LoadFromSaveData(sd);
            Debug.Log("Load complete");

        }
        
    }

    public void LoadFromSaveData(SaveData a_SaveData)
    {
        EngineClass = a_SaveData.m_EngineClass;

    }
    public void SaveData()
    {
        JSONString = JsonUtility.ToJson(tiles);
        File.WriteAllText(Application.persistentDataPath + "/MyGame.json", JSONString);
        Debug.Log("Saved " + Application.persistentDataPath);
    }

    public void LoadData()
    {
        tiles = JsonUtility.FromJson<TileItems>(File.ReadAllText(Application.persistentDataPath + "/MyGame.json"));
        Debug.Log("Loaded");
    }

*/
}
