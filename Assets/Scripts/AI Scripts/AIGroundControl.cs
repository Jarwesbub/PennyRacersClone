using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIGroundControl : MonoBehaviour
{
    public float DrunkLevel,AccNerfer;
    private int AINumber, AICount, EngineClass, speedLimiter;
    private int nextTarget, allTargets;
    [SerializeField] //DEBUGGING
    private float AccLerp, TurnSpeed, AISpeed, MaxSpeedHolder, MaxSpeed;
    private GameObject Ground;

    Rigidbody rb;
    private Vector3 oldPosition;
    private Vector3 randomizeTargetPos;

    private GameObject AIController;
    private GameObject Target;
    //private List SpawnPoints = array.ToList(Target);
    private static List<Vector3> targetPosList;

    private bool IsGrounded;
    public bool GameStart = false;

    // Start is called before the first frame update
    void Awake()
    {
        GameStart = true;

        rb = GetComponent<Rigidbody>();

        nextTarget = 0;
        IsGrounded = false;

        if (AIController == null)
            AIController = GameObject.FindWithTag("aicontroller");

        Ground = this.gameObject.transform.GetChild(AINumber).gameObject;
        
    }



    public void AIControllerStartUp(int AInumber, int AIcount, int engineclass, float accLerp, float turnspeed, float maxspeed)
    {
        AINumber = AInumber;
        AICount = AIcount;
        EngineClass = engineclass;

        AccLerp = accLerp;
        TurnSpeed = turnspeed;
        //MaxSpeed = maxSpeedBaseStat - ((float)AINumber * 2f); //Makes number 0 the fastest and 1 slower and so on
        MaxSpeed = maxspeed;
        MaxSpeedHolder = MaxSpeed;
        //TurnSpeed = (float)EngineClass;

        //AIEngineClassCheck(EngineClass);
    }

    public void AllTargets(List<Vector3> targetposlist, int targetcount)
    {

        targetPosList = targetposlist;
        allTargets = targetcount;

        targetPosRandomizer(AINumber);

    }



    void FixedUpdate()
    {
        AISpeed = Vector3.Distance(oldPosition, gameObject.transform.position) * 200f; // Original = * 100f
        oldPosition = gameObject.transform.position;

        if (IsGrounded == true && GameStart == true)
        {
            float spd = 0f;
            float maxspeed = MaxSpeed;
            float aispeed = AISpeed;
            float turnspeed = TurnSpeed;

            float singleStep = AISpeed * Time.deltaTime;
            Vector3 targetpos = targetPosList[nextTarget] + randomizeTargetPos;
            Vector3 newDirection = Vector3.RotateTowards(transform.position, targetpos, singleStep, 0.0f);

            Vector3 targetDirection = targetpos - transform.position;
            Vector3 relativePos = targetpos - transform.position;

            Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);

            Vector3 turningCloseTarget = targetpos + new Vector3(10f, 10f, 10f);

            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * turnspeed);

            if (aispeed < (MaxSpeed / 2f)) //AI's speed 10f = 80km/h
            {
                float acc = 0.005f; //This MUST BE 0.005f
                float speedvalue = aispeed * AccLerp - AccNerfer; //1f

                float lerpTime = speedvalue * acc;

                if (lerpTime < 0.2f)
                    lerpTime = 0.2f;


                spd = MaxSpeedHolder * 10f;


                float CurrentAcceleration = Mathf.Lerp(0.1f, spd, lerpTime * Time.deltaTime);

                rb.AddRelativeForce(new Vector3(0, 0, CurrentAcceleration/* * 10f * Time.deltaTime*/));


            }


            if (Vector3.Distance(transform.position, targetpos) < 5.0f) //5f normal
            {
                // Swap the position of the cylinder.

                if (nextTarget < allTargets)
                {
                    nextTarget += 1;

                }
                else
                    nextTarget = 0;

                targetPosRandomizer(AINumber);

            }
            
            if (Vector3.Distance(transform.position, targetpos) < 10.0f) //if enemy gets hit close to the target
            {
                turnspeed = 10f;

            }    

        }
    }
    private void targetPosRandomizer(int AInumber)
    {
        float pos = DrunkLevel;


        for (int i = 0; i < AICount; i++)
        {
            if (i == 0)
            {
                randomizeTargetPos.x = Random.Range(-pos, pos);
                randomizeTargetPos.z = Random.Range(-pos, pos);

            }
        }

    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            //Debug.Log("Hit Player");
        }
        else if (other.gameObject.layer == 6)// WALLS
        {
            MaxSpeed = 50f;
            Debug.Log("AI HIT WALL!");
        }
        else
        {
            string numb = " ";
            for (int i = 0; i < AINumber; i++)
            {
                numb = i.ToString();
                string aiNumb = "ai" + numb;

                if (other.gameObject.tag == aiNumb)
                {
                    if (i < AINumber)
                    {
                        MaxSpeed -= 20f;
                    }

                    break;
                }
            }

        }

    }

    void OnTriggerEnter(Collider other)
    {
        
        if (other.tag == "ground")
        {
            IsGrounded = true;
            
        }

        float speednerfer = MaxSpeedHolder / 1000f;

        if (EngineClass <= 2) //AI levels 0-2
        {
            if (other.tag == "slowtarget") //Red Target
            {
                MaxSpeed = MaxSpeedHolder * (0.95f - speednerfer); //0.6f
                MaxSpeed -= DrunkLevel * 2f;
            }
            else if (other.tag == "normaltarget") //Grey Target
            {
                MaxSpeed = MaxSpeedHolder * (1f/* - speednerfer*/); //0.8f
                MaxSpeed -= DrunkLevel * 2f;
            }
            else if (other.tag == "fasttarget") //Green Target
            {
                MaxSpeed = MaxSpeedHolder;
                MaxSpeed -= DrunkLevel * 5f;
            }
        }
        else //AI levels 3-5
        {
            if (other.tag == "slowtarget") //Red Target
            {
                MaxSpeed = MaxSpeedHolder * (0.9f - speednerfer); //0.6f
                MaxSpeed -= DrunkLevel * 2f;
            }
            else if (other.tag == "normaltarget") //Grey Target
            {
                MaxSpeed = MaxSpeedHolder * (1.0f - speednerfer); //0.8f
                MaxSpeed -= DrunkLevel * 2f;
            }
            else if (other.tag == "fasttarget") //Green Target
            {
                MaxSpeed = MaxSpeedHolder;
                MaxSpeed -= DrunkLevel * 5f;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        
        if (other.tag == "ground")
        {
            IsGrounded = false;

        }

    }
}
