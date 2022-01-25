using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public DataManager dataManager;
    //public GameObject[] AINumb;
    //public GameObject[] Target;
    public List<GameObject> AINumb;
    private GameObject TargetController, AICars, Player, PlayerController;
    public List<Vector3> targetPosList = new List<Vector3>();
    //public int TargetNumb;
    [SerializeField]
    private float AISpeed, MaxSpeed, Acc, AccLerp, CurrentAcc, TurnSpeed;
    
    public int MaxLaps, AICount, AIEngineClass, TargetCount;
    //public bool PLAYERAUTOPILOT = false; //TESTING
    void Awake()
    {
        if(dataManager == null)
            dataManager = GameObject.FindWithTag("DataManager").GetComponent<DataManager>();

        TargetController = GameObject.FindWithTag("TargetController");
        AICars = GameObject.FindWithTag("AICars");
        Player = GameObject.FindWithTag("Player");
        PlayerController = GameObject.FindWithTag("PlayerController");

        dataManager.Load();
        MaxLaps = dataManager.data.MaxLaps;

        if (AINumb.Count == 0)
        {
            for (int i = 0; i < AICars.transform.childCount; i++)
            {
                GameObject ai = AICars.transform.GetChild(i).gameObject;
                AINumb.Add(ai);
            }
        }
        
    }

    private void LoadPlayerAutopilot()
    {
        Player.GetComponent<AIGroundControl>().AIControllerStartUp(0, AICount, AIEngineClass, AccLerp, TurnSpeed, MaxSpeed);
        Player.GetComponent<AIGroundControl>().AllTargets(targetPosList, TargetCount);
        Player.GetComponent<AIGroundControl>().enabled = false;

    }
    public void StartPlayerAutopilot()
    {
        Player.GetComponent<CarGroundControl>().enabled = false;
        Player.GetComponent<AIGroundControl>().enabled = true;
        PlayerController.SetActive(false);

    }


    public void LoadAllTargets(Vector3 targetpos)
    {
        targetPosList.Add(targetpos);
    }

    void Start() //Awake wont check AIEngineClass
    {
        TargetCount = targetPosList.Count - 1; //-1 because 0 is there too
        GetAccLevels(AIEngineClass);

        LoadPlayerAutopilot();
        //Do specific actions to specific ai
        for (int i = 0; i < AINumb.Count; i++)
        {
            GameObject List = AINumb[i];
            AICount = AINumb.Count;
            AINumb[i].GetComponent<AIGroundControl>().AIControllerStartUp(i, AICount, AIEngineClass, AccLerp, TurnSpeed, MaxSpeed);
            AINumb[i].GetComponent<AIGroundControl>().AllTargets(targetPosList, TargetCount);
            AINumb[i].GetComponent<AIGroundControl>().MaxLaps = MaxLaps;
        }

    }

    public void GetAccLevels(int engineclass) // ALL AI STATS ARE HERE!
    {
        AIEngineClass = engineclass;

        switch (engineclass) //if (engineclass == case 0)
        {
            case 1: /// ACTUAL START->
                MaxSpeed = 90f;
                AccLerp = 25f;
                TurnSpeed = 5f;
                break;

            case 2:
                MaxSpeed = 110f;
                AccLerp = 18f;
                TurnSpeed = 5f;
                break;

            case 3:
                MaxSpeed = 120f;
                AccLerp = 14f;
                TurnSpeed = 5f;
                break;

            case 4:
                MaxSpeed = 140f;//280f
                AccLerp = 10f;
                TurnSpeed = 6f;
                break;

            case 5:
                MaxSpeed = 160f;//280f
                AccLerp = 8f;
                TurnSpeed = 8f;
                break;
            case 6:
                MaxSpeed = 160f;//SAME AS 5
                AccLerp = 8f;
                TurnSpeed = 8f;
                break;
        }

    }

}
