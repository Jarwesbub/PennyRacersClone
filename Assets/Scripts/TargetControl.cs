using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TargetControl : MonoBehaviour
{
    //public GameObject[] Target;
    public TMP_Text WrongWayTxt;
    public List<GameObject> Target;
    public int TargetCount,PlayerTargetNumber;
    [SerializeField]//DEBUG
    private GameObject Player, PlayerGround, PlayerController, TargetParent, BotCarsParent, BotController, LapController;
    [SerializeField]
    private List<Vector3> targetPosList;
    public List<GameObject> playerTargets;
    public float playerPos, aiPos;
    public int Laps, MaxLaps, PlayerPosition;
    //[SerializeField]
    private bool IsWrongWay = false;
    private int PlayerCurrentTarget;
    // Start is called before the first frame update
    void Awake()
    {
        IsWrongWay = false;

        Laps=1;
        //PlayerCurrentTarget = 0;
        WrongWayTxt.text = " ";
        Player = GameObject.FindWithTag("Player");
        PlayerGround = GameObject.FindWithTag("PlayerGround");
        PlayerController = GameObject.FindWithTag("PlayerController");
        TargetParent = GameObject.FindWithTag("TargetParent");
        BotCarsParent = GameObject.FindWithTag("AICars");
        BotController = GameObject.FindWithTag("aicontroller");
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
            BotController.GetComponent<BotController>().LoadAllTargets(targetpos);
            Player.GetComponent<CarGroundControl>().LoadAllTargets(targetpos);
            PlayerGround.GetComponent<CarTargetTrigger>().LoadAllTargets(targetpos);
            //AIController.GetComponent<AIController>().TargetCount = TargetCount;
        }
    }

    private void TargetList(Vector3 targetpos)
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
                IsWrongWay = true;
                //Debug.Log("Removed");
            }
            else
            {
                playerTargets.Add(objects);
                IsWrongWay = false;
                //Debug.Log("Added");
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

        //if(PlayerCurrentTarget-1 > playerTargets.Count/* && playerTargets.Count!=0*/)
        if(IsWrongWay)
        {
            StartCoroutine(WrongWay());
        }

        //PlayerCurrentTarget = playerTargets.Count;

        return playerTargets.Count;
    }
    private IEnumerator WrongWay()
    {
        //while (PlayerCurrentTarget > playerTargets.Count)
        if(IsWrongWay)
        {
            float wait = 0.3f;
            WrongWayTxt.text = "Wrong way";
            yield return new WaitForSeconds(wait);
            WrongWayTxt.text = " ";
            yield return new WaitForSeconds(0.1f);
            WrongWayTxt.text = "Wrong way";
            yield return new WaitForSeconds(wait);
            WrongWayTxt.text = " ";
            yield return new WaitForSeconds(0.2f);
        }

    }

}
