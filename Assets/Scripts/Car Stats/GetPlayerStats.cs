using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetPlayerStats : MonoBehaviour
{
    public DataManager dataManager;
    public GameObject PlayerController;
    public GameObject AIController;

    public Text t_playername;

    private int engineClass;
    private float enginePower, acc;

    //void EngineClassesListed(int level)
    void Awake()
    {
        dataManager.Load();

        t_playername.text = ("Player: ") + dataManager.data.name;

        GetCarEngineStats();
        GetCarAccStats();

        PlayerController.GetComponent<CarController>().EnginePower = enginePower;
        PlayerController.GetComponent<CarController>().Acceleration = acc;
        int aiengineclass = dataManager.data.AIEngineClass;
        AIController.GetComponent<AIController>().AIEngineClass = aiengineclass;//AI 

    }


    private void GetCarEngineStats()
    {
        
        engineClass = dataManager.data.EngineClass;
        /*
        if (engineClass == 0)
        {
            enginePower = 1.6f;
            dataManager.data.EngineClass = 1;
        }
        */
        if (engineClass == 0)
        {
            enginePower = 1.6f;
        }

        else if (engineClass == 1)
        {
            enginePower = 2.08f;
        }

        else if (engineClass == 2)
        {
            enginePower = 2.48f;
        }

        else if (engineClass == 3)
        {
            enginePower = 2.9f;
        }

        else if (engineClass == 4)
        {
            enginePower = 3.27f;
        }

        else if (engineClass == 5)
        {
            enginePower = 4.2f;
        }

        dataManager.data.EnginePower = enginePower;
        dataManager.Save();

    }

    private void GetCarAccStats()
    {
        int accLevel = dataManager.data.AccLvl;


        if (accLevel == 1 || accLevel == 0)
        {
            acc = 0.845f; //0.84f
        }

        else if (accLevel == 2)
        {
            acc = 0.855f; //0.85f
        }

        else if (accLevel == 3)
        {
            acc = 0.87f; //0.86f
        }

        dataManager.Save();

    }

}
