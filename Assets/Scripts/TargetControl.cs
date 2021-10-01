using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetControl : MonoBehaviour
{
    //public GameObject[] Target;
    public List<GameObject> Target;
    public int TargetCount,PlayerTargetNumber;
    [SerializeField]//DEBUG
    private GameObject Player, PlayerController, TargetParent, AICars, AIController, LapController;
    [SerializeField]
    private List<Vector3> targetPosList;
    public List<GameObject> playerTargets;
    public float playerPos, aiPos;
    public int Laps, MaxLaps, PlayerPosition;
    // Start is called before the first frame update
    void Awake()
    {
        Laps=1;
        Player = GameObject.FindWithTag("Player");
        PlayerController = GameObject.FindWithTag("PlayerController");
        TargetParent = GameObject.FindWithTag("TargetParent");
        AICars = GameObject.FindWithTag("AICars");
        AIController = GameObject.FindWithTag("aicontroller");
        LapController = GameObject.FindWithTag("LapController");
        MaxLaps = LapController.GetComponent<LapControl>().MaxLaps;

        if(Target.Count == 0)
        { 
        for (int i = 0; i < TargetParent.transform.childCount; i++)
        {
            GameObject targets = TargetParent.transform.GetChild(i).gameObject;
            Target.Add(targets);
        }
        }


        //Make the list of all targets
        for (int i = 0; i < Target.Count; i++)
        {
            //GameObject List = Target[i];
            TargetCount = i;

            Vector3 targetpos = Target[i].transform.position;

            targetPosList.Add(targetpos);
            AIController.GetComponent<AIController>().LoadAllTargets(targetpos);
            Player.GetComponent<CarGroundControl>().LoadAllTargets(targetpos);
            //AIController.GetComponent<AIController>().TargetCount = TargetCount;
        }
    }

    public void TargetList(Vector3 targetpos)
    {
        targetPosList.Add(targetpos);
    }

    public int PlayerTargetList(GameObject objects, bool destroy)
    {
        if (!destroy)
        {
            if (playerTargets.Contains(objects))
            {
                playerTargets.Remove(objects);

            }
            else
            {
                playerTargets.Add(objects);
            }
        }
        else //DESTROY ALL OBJECTS
        {
            if (playerTargets.Count > TargetCount - 5)
            {
                Laps++;
                LapController.GetComponent<LapControl>().DrawLaps(Laps);
                if (Laps > MaxLaps) //FINISHED
                {
                    string name = "player";
                    LapController.GetComponent<LapControl>().FinishedPlayers(-1, name);

                }

            }

            playerTargets.Clear();
        }
        return playerTargets.Count;
    }
}
