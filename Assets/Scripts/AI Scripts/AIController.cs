using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{

    public GameObject[] AINumb;
    public GameObject[] Target;
    public List<Vector3> targetPosList = new List<Vector3>();
    public int TargetNumb;
    public float TEST;
    
    public float AISpeed, MaxSpeed, Acc, CurrentAcc;

    public int AICount, TargetCount, AIEngineClass;

        
    void Awake()
    {
        //Make the list of all targets
        for (int i = 0; i < Target.Length; i++)
        {
            //GameObject List = Target[i];
            TargetCount = i;
            
            Vector3 targetPos = Target[i].transform.position;
 
            targetPosList.Add(targetPos);
            
        }

        

        GetAccLevels(AIEngineClass);

        //Do specific actions to specific ai
        for (int i = 0; i < AINumb.Length; i++)
        {
            GameObject List = AINumb[i];
            AICount = AINumb.Length;

            AINumb[i].GetComponent<AIGroundControl>().AIControllerStartUp(i, AICount, AIEngineClass, Acc, MaxSpeed);
            //AINumb[i].GetComponent<AIGroundControl>().AIControllerAllTargets(Target[i], i);
            
            AINumb[i].GetComponent<AIGroundControl>().AllTargets(targetPosList, TargetCount);
        }
        

    }
    
    public void GetAccLevels(int EngineClass)
    {
        AIEngineClass = EngineClass;

        if (EngineClass == 1 || EngineClass == 0)
        {
            Acc = 0.03f;
            MaxSpeed = 80f;
        }
        else if (EngineClass == 2)
        {
            Acc = 0.02f;
            MaxSpeed = 100f;
        }
        else if (EngineClass == 3)
        {
            Acc = 0.02f;
            MaxSpeed = 120f;
        }
        else if (EngineClass == 4)
        {
            Acc = 0.02f;
            MaxSpeed = 120f;
        }
        else if (EngineClass == 5)
        {
            Acc = 0.0075f;
            MaxSpeed = 220f;
        }
        else if (EngineClass == 6) //6 is currently same as 5
        {
            Acc = 0.0075f;
            MaxSpeed = 220f;
        }

    }



}
