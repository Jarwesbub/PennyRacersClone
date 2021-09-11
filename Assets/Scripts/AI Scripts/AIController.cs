using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public GameObject[] AINumb;
    public GameObject[] Target;
    public List<Vector3> targetPosList = new List<Vector3>();
    //public int TargetNumb;
    //public float TEST;
    [SerializeField]
    private float AISpeed, MaxSpeed, Acc, AccLerp, CurrentAcc, TurnSpeed;
    
    public int AICount, TargetCount, AIEngineClass;

    void Start() //Awake wont check AIEngineClass
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
            AINumb[i].GetComponent<AIGroundControl>().AIControllerStartUp(i, AICount, AIEngineClass, AccLerp, TurnSpeed, MaxSpeed);
            //AINumb[i].GetComponent<AIGroundControl>().AIControllerAllTargets(Target[i], i);
            AINumb[i].GetComponent<AIGroundControl>().AllTargets(targetPosList, TargetCount);
        }
        

    }

    public void GetAccLevels(int engineclass) // ALL AI STATS ARE HERE!
    {
        AIEngineClass = engineclass;

        switch (engineclass) //if (engineclass == case 0)
        {
            case 0:
                MaxSpeed = 100f;
                AccLerp = 1f;
                TurnSpeed = 1.5f;
                break;

            case 1:
                MaxSpeed = 120f;
                AccLerp = 3.2f;
                TurnSpeed = 1.5f;
                break;

            case 2:
                MaxSpeed = 160f;
                AccLerp = 2.2f;
                TurnSpeed = 1.5f;
                break;

            case 3:
                MaxSpeed = 200f;
                AccLerp = 1.8f;
                TurnSpeed = 3f;
                break;

            case 4:
                MaxSpeed = 240f;
                AccLerp = 1.40f;
                TurnSpeed = 4f;
                break;

            case 5:
                MaxSpeed = 280f;
                AccLerp = 1.15f;
                TurnSpeed = 5f;
                break;
        }

    }

}
