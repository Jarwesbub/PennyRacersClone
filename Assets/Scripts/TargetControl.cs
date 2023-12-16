using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TargetControl : MonoBehaviour
{
    public TMP_Text WrongWayTxt;
    public List<GameObject> Target;
    public int targetCount, playerTargetNumber;
    [SerializeField]//DEBUG
    private GameObject Player, PlayerGround, PlayerController, TargetParent, BotCarsParent, BotController, LapController;
    [SerializeField]
    private List<Vector3> targetPosList;
    public List<GameObject> playerTargets;
    public float playerPos, aiPos;
    public int laps, maxLaps, playerPosition;
    private bool isWrongWay = false;



    void Awake()
    {
        isWrongWay = false;
        laps = 1;
        WrongWayTxt.text = " ";
        Player = GameObject.FindWithTag("Player");
        PlayerGround = GameObject.FindWithTag("PlayerGround");
        PlayerController = GameObject.FindWithTag("PlayerController");
        TargetParent = GameObject.FindWithTag("TargetParent");
        BotCarsParent = GameObject.FindWithTag("AICars");
        BotController = GameObject.FindWithTag("aicontroller");
        LapController = GameObject.FindWithTag("LapController");
        maxLaps = LapController.GetComponent<LapControl>().maxLaps;

        if (Target.Count == 0)
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
            targetCount = i;
            Vector3 targetpos = Target[i].transform.position;
            targetPosList.Add(targetpos);
            BotController.GetComponent<BotController>().LoadAllTargets(targetpos);
            Player.GetComponent<CarGroundControl>().LoadAllTargets(targetpos);
            PlayerGround.GetComponent<CarTargetTrigger>().LoadAllTargets(targetpos);
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
                isWrongWay = true;
            }
            else
            {
                playerTargets.Add(objects);
                isWrongWay = false;
            }
        }
        else //DESTROY ALL OBJECTS
        {
            if (playerTargets.Count > targetCount - 5)
            {
                laps++;
                LapController.GetComponent<LapControl>().DrawLaps(laps);
                if (laps > maxLaps) //FINISHED
                {
                    string name = "player";
                    LapController.GetComponent<LapControl>().FinishedPlayers(-1, name);
                }

            }

            playerTargets.Clear();
        }

        if (isWrongWay)
        {
            StartCoroutine(WrongWay());
        }

        return playerTargets.Count;
    }
    private IEnumerator WrongWay()
    {
        if (isWrongWay)
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
