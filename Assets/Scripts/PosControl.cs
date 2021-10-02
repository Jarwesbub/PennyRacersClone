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
    public List<GameObject> AI;
    public GameObject Player, TargetController;
    public GameObject NextAI, PrevAI;
    public int playerPos, playerNextTarget, nextAITarget, prevAITarget;
    //public int nextAINumb,prevAINumb;
    public int Lap, nextAILap, prevAILap;
    public float playerTargetDistance, nextAITargetDist/*, prevAITargetDist*/;
    public int AICount;
    public bool PlayerIsLast, IsWrongWay; //debugging can be deleted later
    private float Steps, waitSteps;

    // Start is called before the first frame update
    void Awake()
    {
        waitSteps = 0.6f; //seconds

        Lap = 1;
        if (AICars == null)
            AICars = GameObject.FindWithTag("AICars");

        if (Player == null)
            Player = GameObject.FindWithTag("Player");

        if (TargetController == null)
            TargetController = GameObject.FindWithTag("TargetController");



        int i;
        for (i = 0; i < AICars.transform.childCount; i++)
        {
            GameObject ai = AICars.transform.GetChild(i).gameObject;
            AI.Add(ai);
        }
        AICount = i-1;
        playerPos = i; //Last place
        //PLAYER START LAST ->
        NextAI = AI[AICount];
        PrevAI = AI[AICount];
        IsWrongWay = false;
    }



    // Update is called once per frame
    void Update()
    {
        Steps += Time.deltaTime;
        string th = "th";
        if (playerPos == 0)
            th = "st";
        else if (playerPos == 1)
            th = "nd";
        else if (playerPos == 2)
            th = "rd";
        int playerposadd = playerPos + 1;
        PlayerPosTxt.text = "Pos: "+playerposadd.ToString()+th;
        
        Lap = TargetController.GetComponent<TargetControl>().Laps;    
        nextAITarget = NextAI.GetComponent<AIGroundControl>().nextTarget;
        nextAILap = NextAI.GetComponent<AIGroundControl>().Lap;
        prevAITarget = PrevAI.GetComponent<AIGroundControl>().nextTarget;
        playerNextTarget = Player.GetComponent<CarGroundControl>().nextTarget;

        {
            if (Lap == nextAILap) //SAME LAP
            {
                if (playerNextTarget > nextAITarget)
                {
                    //if (playerPos > 1)
                    playerPos = CalculatePosition(playerPos-1, playerPos, true);

                }
                else if (playerNextTarget < prevAITarget)
                {
                    //if (playerPos < LastPosition)
                    playerPos = CalculatePosition(playerPos - 1, playerPos, false);
                }
                else if (nextAITarget == playerNextTarget || prevAITarget == playerNextTarget) //calculate distance from the next target
                {
                    if (Steps > waitSteps)
                    {
                        Steps = 0f;
                        GetTargetDistances();
                    }
                }
                
            }
            
            else if(Lap > nextAILap) //PLAYER AHEAD
            {
                   playerPos = CalculatePosition(playerPos - 1, playerPos, true);

            }
            else if (Lap < prevAILap) //AI AHEAD
            {
                playerPos = CalculatePosition(playerPos - 1, playerPos, false);
            }
            
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
            if (playerpos < AICount + 1)
            {
                playerpos++;
            }
            else
                playerpos = AICount + 1;
        }


        if(playerpos > 0 && playerpos < AICount+1)
        {
            nextai = playerpos - 1;
            prevai = playerpos;
            PlayerIsLast = false;
        }
        else if (prevai > AICount)
        {
            nextai = AICount-1;
            prevai = AICount;
            PlayerIsLast = true;
        }
        if(nextai < 0)
        {
            nextai = 0;
            prevai = 0;
            PlayerIsLast = false;
        }



        //nextAINumb = nextai;
        //prevAINumb = prevai;
        NextAI = AI[nextai];
        PrevAI = AI[prevai];


        return playerpos;


    }


    private void GetTargetDistances()
    {
        nextAITargetDist = NextAI.GetComponent<AIGroundControl>().TargetDistance;
        playerTargetDistance = Player.GetComponent<CarGroundControl>().TargetDistance;
        //nextAITargetDist = Mathf.Round(nextAITargetDist * 100f) / 100f;
        //prevAITargetDist = Mathf.Round(prevAITargetDist * 100f) / 100f;

        if (playerTargetDistance < nextAITargetDist)
        {
            playerPos = CalculatePosition(playerPos-1, playerPos, true);
        }
       /*
        else
        {
            prevAITargetDist = PrevAI.GetComponent<AIGroundControl>().TargetDistance;
            if (playerTargetDistance > prevAITargetDist+val)
            {
                playerPos = CalculatePosition(playerPos - 1, playerPos, false);
            }
        }
        */


    }

}
