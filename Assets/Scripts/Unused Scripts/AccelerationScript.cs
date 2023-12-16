using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccelerationScript : MonoBehaviour
{
    public GameObject PlayerController;
    private float Acc, EnginePow;
    public int AccClass;


    public Text AccTxt;

    void Awake()
    {
        AccClass = 1;
        AccStatsListed(AccClass);
        EnginePow = PlayerController.GetComponent<CarController>().GetEnginePower();
    }



    public void AccelerationLevels()//---------
    {
        if (AccClass < 3)
            AccClass += 1;
        else
            AccClass = 1;

        AccStatsListed(AccClass);
    }

    void Update()
    {
        AccTxt.text = "Acceleration lvl: " + AccClass.ToString();

    }

    /// <summary>
    /// REAL DOWN STATS HERE->
    /// </summary>
    /// 

    void AccStatsListed(int level)
    {
        Acc = 0f;

        //EnginePow = PlayerController.GetComponent<CarController>().enginePower;

        if (level == 1)
        {
            Acc = 0.84f;
        }

        if (level == 2)
        {
            Acc = 0.85f;
        }

        if (level == 3)
        {
            Acc = 0.865f;
        }

        //PlayerController.GetComponent<CarController>().acc = Acc;
    }
}
