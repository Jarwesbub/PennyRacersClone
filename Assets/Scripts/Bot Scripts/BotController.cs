using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotController : MonoBehaviour
{
    public DataManager dataManager;
    public List<GameObject> botIDs;
    private GameObject AICars, Player, PlayerController;
    public List<Vector3> targetPosList = new List<Vector3>();
    [SerializeField]
    private float botSpeed, maxSpeed, acc, accLerp, currentAcc, turnSpeed;
    
    public int maxLaps, botCount, botEngineClass, targetCount;

    void Awake()
    {
        if(dataManager == null)
            dataManager = GameObject.FindWithTag("DataManager").GetComponent<DataManager>();

        AICars = GameObject.FindWithTag("AICars");
        Player = GameObject.FindWithTag("Player");
        PlayerController = GameObject.FindWithTag("PlayerController");

        dataManager.Load();
        maxLaps = dataManager.data.maxLaps;

        if (botIDs.Count == 0)
        {
            for (int i = 0; i < AICars.transform.childCount; i++)
            {
                GameObject ai = AICars.transform.GetChild(i).gameObject;
                botIDs.Add(ai);
            }
        }
        
    }

    private void LoadPlayerAutopilot()
    {
        Player.GetComponent<BotGroundControl>().AIControllerStartUp(0, botCount, botEngineClass, accLerp, turnSpeed, maxSpeed);
        Player.GetComponent<BotGroundControl>().AllTargets(targetPosList, targetCount);
        Player.GetComponent<BotGroundControl>().enabled = false;

    }
    public void StartPlayerAutopilot()
    {
        Player.GetComponent<CarGroundControl>().enabled = false;
        Player.GetComponent<BotGroundControl>().enabled = true;
        PlayerController.SetActive(false);

    }


    public void LoadAllTargets(Vector3 targetpos)
    {
        targetPosList.Add(targetpos);
    }

    void Start() //Awake wont check AIEngineClass
    {
        targetCount = targetPosList.Count - 1; //-1 because 0 is there too
        GetAccLevels(botEngineClass);

        LoadPlayerAutopilot();
        //Do specific actions to specific ai
        for (int i = 0; i < botIDs.Count; i++)
        {
            GameObject List = botIDs[i];
            botCount = botIDs.Count;
            botIDs[i].GetComponent<BotGroundControl>().AIControllerStartUp(i, botCount, botEngineClass, accLerp, turnSpeed, maxSpeed);
            botIDs[i].GetComponent<BotGroundControl>().AllTargets(targetPosList, targetCount);
            botIDs[i].GetComponent<BotGroundControl>().maxLaps = maxLaps;
        }

    }

    public void GetAccLevels(int engineclass) // ALL AI STATS ARE HERE!
    {
        botEngineClass = engineclass;

        switch (engineclass) //if (engineclass == case 0)
        {
            case 1: /// ACTUAL START->
                maxSpeed = 90f;
                accLerp = 25f;
                turnSpeed = 5f;
                break;

            case 2:
                maxSpeed = 110f;
                accLerp = 18f;
                turnSpeed = 5f;
                break;

            case 3:
                maxSpeed = 120f;
                accLerp = 14f;
                turnSpeed = 5f;
                break;

            case 4:
                maxSpeed = 140f;//280f
                accLerp = 10f;
                turnSpeed = 6f;
                break;

            case 5:
                maxSpeed = 160f;//280f
                accLerp = 8f;
                turnSpeed = 8f;
                break;
            case 6:
                maxSpeed = 160f;//SAME AS 5
                accLerp = 8f;
                turnSpeed = 8f;
                break;
        }

    }

}
