using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIGroundControl : MonoBehaviour
{
    public int AINumber, AICount, EngineClass, speedLimiter;
    private int nextTarget, allTargets;
    public float AISpeed, Acc, maxSpeedBaseStat, MaxSpeed;
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

    
    public void AIControllerStartUp(int AInumber, int AIcount, int engineclass, float acc, float maxspeed)
    {
        AINumber = AInumber;
        AICount = AIcount;
        EngineClass = engineclass;
        Acc = acc;
        maxSpeedBaseStat = maxspeed;
        MaxSpeed = maxSpeedBaseStat - ((float)AINumber * 2f); //Makes number 0 the fastest and 1 slower and so on

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
            float spd = 30f;
            float maxspeed = MaxSpeed;
            float aispeed = AISpeed;

            if (aispeed < (MaxSpeed / 2f)) //AI's speed 10f = 80km/h
            {
                
                float speedvalue = aispeed/* * 2f*/;
                float acc = Acc;


                float lerpTime = speedvalue * acc;

                if (lerpTime < 0.2f)
                    lerpTime = 0.2f;


                spd = maxSpeedBaseStat * 10f;

                
                float CurrentAcceleration = Mathf.Lerp(0.1f, spd, lerpTime * Time.deltaTime);

                rb.AddRelativeForce(new Vector3(0, 0, CurrentAcceleration/* * 10f * Time.deltaTime*/));


            }


            float singleStep = AISpeed * Time.deltaTime;
            Vector3 targetpos = targetPosList[nextTarget] + randomizeTargetPos;
            //Vector3 newDirection = Vector3.RotateTowards(transform.position, Target.transform.position, singleStep, 0.0f);
            Vector3 newDirection = Vector3.RotateTowards(transform.position, targetpos, singleStep, 0.0f);

            //Debug.DrawRay(transform.position, targetpos, Color.red);

            Vector3 targetDirection = targetpos - transform.position;

            //Debug.DrawRay(transform.position, newDirection, Color.red);

            //transform.rotation = Quaternion.LookRotation(newDirection);
            Vector3 relativePos = targetpos - transform.position;
            //relativePos = Quaternion.Lerp(0f, targetpos.x, 1f * Time.deltaTime);

            Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);

            float TurnSpeed = 1.5f;
            Vector3 turningCloseTarget = targetpos + new Vector3(10f, 10f, 10f);

            if (Vector3.Distance(transform.position, targetpos) < 10.0f) //if enemy gets hit close to the target
            {
                TurnSpeed = 10f;

            }


            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * TurnSpeed);




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
                

        }
    }
    private void targetPosRandomizer(int AInumber)
    {
        
        for (int i = 0; i < AICount; i++)
        {
            if (i == 0)
            {
                randomizeTargetPos.x = Random.Range(-4f, 4f);
                randomizeTargetPos.z = Random.Range(-4f, 4f);

            }
        }

    }



    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Hit Player");
        }


    }

    void OnTriggerEnter(Collider other)
    {
        
        if (other.tag == "ground")
        {
            IsGrounded = true;
            
        }

        float speednerfer = maxSpeedBaseStat / 1000f;

        if (other.tag == "slowtarget") //Red Target
        {
            MaxSpeed = maxSpeedBaseStat * (0.9f - speednerfer); //0.6f
        }
        else if (other.tag == "normaltarget") //Grey Target
        {
            MaxSpeed = maxSpeedBaseStat * (1f - speednerfer); //0.8f
        }
        else if (other.tag == "fasttarget") //Green Target
        {
            MaxSpeed = maxSpeedBaseStat;
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
