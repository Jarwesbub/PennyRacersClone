using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PosControl : MonoBehaviour
{
    //0 = 1 in player/ai positions!
    public TMP_Text PlayerPosTxt;
    public TMP_Text LapsTxt, PosTxt, WrongWayTxt;
    public GameObject AICars;
    public List<GameObject> botsList;
    public GameObject Player, PlayerGround, TargetController;
    public GameObject nextAI, prevAI;
    public int playerPos, playerNextTarget, nextAITarget, prevAITarget;
    //public int nextAINumb,prevAINumb;
    public int lap, nextAILap, prevAILap;
    public float playerTargetDistance, nextAITargetDist/*, prevAITargetDist*/;
    public int botCount;
    public bool playerIsLast; //debugging can be deleted later
    private float steps, waitSteps;

    // Start is called before the first frame update
    void Awake()
    {
        waitSteps = 0.6f; //seconds

        lap = 1;
        if (AICars == null)
            AICars = GameObject.FindWithTag("AICars");

        Player = GameObject.FindWithTag("Player");
        PlayerGround = GameObject.FindWithTag("PlayerGround");

        if (TargetController == null)
            TargetController = GameObject.FindWithTag("TargetController");



        int i;
        for (i = 0; i < AICars.transform.childCount; i++)
        {
            GameObject ai = AICars.transform.GetChild(i).gameObject;
            botsList.Add(ai);
        }
        botCount = i - 1;
        playerPos = i; //Last place
        //PLAYER START LAST ->
        nextAI = botsList[botCount];
        prevAI = botsList[botCount];
    }


    void Update()
    {
        steps += Time.deltaTime;
        string th = "th";
        if (playerPos == 0)
            th = "st";
        else if (playerPos == 1)
            th = "nd";
        else if (playerPos == 2)
            th = "rd";
        int playerposadd = playerPos + 1;
        PlayerPosTxt.text = "Pos: " + playerposadd.ToString() + th;

        lap = TargetController.GetComponent<TargetControl>().laps;
        nextAITarget = nextAI.GetComponent<BotGroundControl>().nextTarget;
        nextAILap = nextAI.GetComponent<BotGroundControl>().lap;
        prevAITarget = prevAI.GetComponent<BotGroundControl>().nextTarget;
        playerNextTarget = PlayerGround.GetComponent<CarTargetTrigger>().nextTarget;

        if (lap == nextAILap) //SAME LAP
        {
            if (playerNextTarget > nextAITarget)
            {
                playerPos = CalculatePosition(playerPos - 1, playerPos, true);

            }
            else if (playerNextTarget < prevAITarget)
            {
                playerPos = CalculatePosition(playerPos - 1, playerPos, false);
            }
            else if (nextAITarget == playerNextTarget || prevAITarget == playerNextTarget) //calculate distance from the next target
            {
                if (steps > waitSteps)
                {
                    steps = 0f;
                    GetTargetDistances();
                }
            }

        }
        else if (lap > nextAILap) //PLAYER AHEAD
        {
            playerPos = CalculatePosition(playerPos - 1, playerPos, true);

        }
        else if (lap < prevAILap) //AI AHEAD
        {
            playerPos = CalculatePosition(playerPos - 1, playerPos, false);
        }

    }


    private int CalculatePosition(int nextai, int prevai, bool playerGoUP)
    {
        int playerpos = playerPos;
        if (playerGoUP)
        {
            if (playerpos > 0)
            {
                playerpos--;
            }
            else
                playerpos = 0;
        }
        else if (!playerGoUP)
        {
            if (playerpos < botCount + 1)
            {
                playerpos++;
            }
            else
                playerpos = botCount + 1;
        }


        if (playerpos > 0 && playerpos < botCount + 1)
        {
            nextai = playerpos - 1;
            prevai = playerpos;
            playerIsLast = false;
        }
        else if (prevai > botCount)
        {
            nextai = botCount - 1;
            prevai = botCount;
            playerIsLast = true;
        }
        if (nextai < 0)
        {
            nextai = 0;
            prevai = 0;
            playerIsLast = false;
        }

        nextAI = botsList[nextai];
        prevAI = botsList[prevai];

        return playerpos;
    }


    private void GetTargetDistances()
    {
        nextAITargetDist = nextAI.GetComponent<BotGroundControl>().targetDistance;
        playerTargetDistance = PlayerGround.GetComponent<CarTargetTrigger>().targetDistance;

        if (playerTargetDistance < nextAITargetDist)
        {
            playerPos = CalculatePosition(playerPos - 1, playerPos, true);
        }

    }

}
