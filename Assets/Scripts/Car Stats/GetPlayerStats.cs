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

        switch(engineClass)
        {
            case 1:
                enginePower = 2.48f;//2.48f//1: 90km/h, 3: 100km/h
                break;

            case 2:
                enginePower = 2.9f;//2.90f//1: 100km/h, 3: 110km/h
                break;

            case 3:
                enginePower = 3.4f;//3.27f//1: 120km/h, 3: 130km/h
                break;

            case 4:
                enginePower = 4.2f;//4.2f//1: 140km/h, 3: 150km/h
                break;

            case 5:
                enginePower = 5.2f;//5.2f//1: 160km/h, 3: 160km/h
                break;
            case 6:
                enginePower = 6.66f;//5.2f//1: 180km/h
                break;
        }

        dataManager.data.EnginePower = enginePower;
        dataManager.Save();

    }

    private void GetCarAccStats()
    {
        int accLevel = dataManager.data.AccLvl;


        if (accLevel == 1 || accLevel == 0)
        {
            acc = 0.850f; //0.845f
        }

        else if (accLevel == 2)
        {
            acc = 0.865f; //0.855f
        }

        else if (accLevel == 3)
        {
            acc = 0.88f; //0.87f
        }

        dataManager.Save();

    }
    /*
    private void OLDGetCarEngineStats()
    {

        engineClass = dataManager.data.EngineClass;

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
    */
}
