using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnginePowerScript : MonoBehaviour
{
    public GameObject PlayerController;
    public GameObject AIController;
    private float EnginePower, Acc;
    public int EngineClass, AccLvl;


    public Text EnginePowTxt;
    public Text AccTxt;

    void Awake()
    {
        EngineClass = 1;
        AccLvl = 1;
        EngineClassesListed(EngineClass);
        AccStatsListed(AccLvl);
    }



    public void EngineClasses()//---------
    {
        if (EngineClass < 6)
            EngineClass += 1;
        else
            EngineClass = 1;

        EngineClassesListed(EngineClass);
    }

    public void AccelerationAdd()
    {
        if (AccLvl < 3)
            AccLvl += 1;
        else
            AccLvl = 1;

        AccStatsListed(AccLvl);

    }

    void Update()
    {
        EnginePowTxt.text = "Engine Class: " + EngineClass.ToString();
        AccTxt.text = "Acceleration lvl: " + AccLvl.ToString();
    }

    /// <summary>
    /// REAL DOWN STATS HERE->
    /// </summary>
    /// 
    void AccStatsListed(int level)
    {
        //Acc = 0f + (level / 100f);

        AccLvl = level;

        if (level == 1)
        {
            Acc = 0.845f; //0.84f
        }

        if (level == 2)
        {
            Acc = 0.855f; //0.85f
        }

        if (level == 3)
        {
            Acc = 0.87f; //0.86f
        }
        PlayerController.GetComponent<CarController>().Acceleration = Acc;
    }

    void EngineClassesListed(int level)
    {
        AccStatsListed(1);

        if (level == 1)
        {
            EnginePower = 1.6f; //1.6f
        }

        if (level == 2)
        {
            EnginePower = 2.08f; //2.0f
        }

        if (level == 3)
        {
            EnginePower = 2.48f; //2.4f
        }

        if (level == 4)
        {
            EnginePower = 2.9f; //2.8f
        }

        if (level == 5)
        {
            EnginePower = 3.27f; //3.2f
        }

        if (level == 6)
        {
            EnginePower = 4.2f; //3.65f
        }


        PlayerController.GetComponent<CarController>().EnginePower = EnginePower;
        PlayerController.GetComponent<CarController>().Acceleration = Acc;
        AIController.GetComponent<AIController>().GetAccLevels(EngineClass);
        
    }

}
