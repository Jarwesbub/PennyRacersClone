using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotGroundControl : MonoBehaviour
{
    public string botName;
    public GameObject GameController, Ground;
    GameObject LapController;
    public float drunkLevel, targetDistance;
    public int lap, nextTarget, allTargets, maxLaps;
    private int botID, botCount, engineClass;
    [SerializeField] //DEBUGGING
    private float mass, turnSpeed, botSpeed, maxSpeedHolder, maxSpeed, loseSpeed;
    private float accLerp, accNerfer, steps, waitSteps;

    Rigidbody rb;
    private Vector3 oldPosition;
    private Vector3 randomizeTargetPos;

    private GameObject AIController;

    private static List<Vector3> targetPosList;
    public bool IsGrounded, ForceRespawn;
    [SerializeField]//DEBUGGING
    private bool gameStart = false, isFinished=false, isABot=true;
    public bool raceIsOver;

    // Start is called before the first frame update
    void Awake()
    {
        raceIsOver = false;
        gameStart = false;
        rb = GetComponent<Rigidbody>();
        mass = rb.mass;
        mass *=4f; //Original mass was 0.2f
        loseSpeed = 1f;
        nextTarget = 0;
        ForceRespawn = false;

        if (AIController == null)
            AIController = GameObject.FindWithTag("aicontroller");

        if (GameController == null)
            GameController = GameObject.FindWithTag("GameController");

        LapController = GameObject.FindWithTag("LapController");
        lap = 1;
        
        if (isABot) //Hide Unity's "VALUE NEVER USED" -message
            isABot = true;
    }
    void OnEnable()
    {
        if(gameObject.tag == "Player")
        {
            isABot = false;
            Ground = GameObject.FindWithTag("PlayerGround");
            botName = "NotAItoday";
        }
        else
        {
            isABot = true;
        }


    }


    public void AIControllerStartUp(int botID, int botCount, int engineClass, float accLerp, float turnSpeed, float maxSpeed)
    {
        this.botID = botID;
        this.botCount = botCount;
        this.engineClass = engineClass;
        this.accLerp = accLerp;
        this.turnSpeed = turnSpeed;
        this.maxSpeed = maxSpeed;
        this.maxSpeed -= (float)this.botID;
        maxSpeedHolder = this.maxSpeed;
        this.maxSpeed -= (float)this.botID;

    }

    public void AllTargets(List<Vector3> targetposlist, int targetcount)
    {
        targetPosList = targetposlist;
        allTargets = targetcount;

        targetPosRandomizer(botID);

    }

    void FixedUpdate()
    {
        steps += Time.deltaTime;
        botSpeed = Vector3.Distance(oldPosition, gameObject.transform.position) * 200f; // Original = * 100f
        oldPosition = gameObject.transform.position;

        float gravityBuff = 500f;
        rb.AddRelativeForce(Vector3.down * Time.deltaTime * gravityBuff);

        if (gameStart)
        {
            if(!ForceRespawn)
            {
                float speed = 1f;
                float maxspeed = maxSpeed;
                float botspeed = botSpeed;
                float turnspeed = turnSpeed;

                if (raceIsOver)
                {
                    maxspeed *= 0.7f;
                }

                Vector3 targetpos = targetPosList[nextTarget] + randomizeTargetPos;
                Vector3 relativePos = targetpos - transform.position;
                Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);

                if (Vector3.Distance(transform.position, targetpos) < 10.0f) //bot wont drive around the target
                {
                    turnspeed += 5f;

                }

                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * turnspeed);

                if (botspeed < (maxspeed)) //Bots speed 10f = 80km/h
                {
                    float acc = 0.001f; // Base value
                    float speedvalue = botspeed * accLerp - accNerfer; //1f

                    float lerpTime = speedvalue * acc;

                    if (lerpTime < 0.2f)
                        lerpTime = 0.2f;

                    speed = maxSpeed * 10f * 2f;

                    float CurrentAcceleration = Mathf.Lerp(0.1f, speed, lerpTime * Time.deltaTime);

                    if (IsGrounded)
                        loseSpeed = 1f;
                    else if(loseSpeed > 0.0f)
                        loseSpeed -= 0.002f;

                    rb.AddRelativeForce(new Vector3(0, -2f, mass * loseSpeed * CurrentAcceleration));

                }
                targetDistance = Vector3.Distance(transform.position, targetpos);

                if (targetDistance < 5.0f) //5f normal
                {
                    // Swap the position of the cylinder.
                    if (nextTarget < allTargets)
                    {
                        nextTarget += 1;

                    }
                    else
                    {
                        nextTarget = 0;
                        lap++;
                    }
                    targetPosRandomizer(botID);

                }

            }
            if (!IsGrounded || ForceRespawn) // BUGI PAIKKAAAA!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            {
                if (botSpeed < 30f)
                {
                    StartCoroutine(BotRespawn());
                }

            }
        }
        else if (!gameStart)
        {
            gameStart = GameController.GetComponent<RaceControl>().gameStart;

        }
        if(lap>maxLaps && !isFinished)
        {
            if(!raceIsOver)
                LapController.GetComponent<LapControl>().FinishedPlayers(botID, botName);

            isFinished = true;


        }
    }
    private void targetPosRandomizer(int botID)
    {
        float pos = drunkLevel;


        for (int i = 0; i < botCount; i++)
        {
            if (i == 0)
            {
                randomizeTargetPos.x = Random.Range(-pos, pos);
                randomizeTargetPos.z = Random.Range(-pos, pos);

            }
        }

    }

    IEnumerator BotRespawn()
    {
        yield return new WaitForSeconds(2f);

        if(!IsGrounded || ForceRespawn)
        {
            int prevtarget = nextTarget - 1;
            int nexttarget = nextTarget;

            if (nextTarget <= 0)
            {
                prevtarget = 0;
                nexttarget = 1;

            }

            transform.rotation = Quaternion.Euler(0f, transform.rotation.y, 0f);
            transform.position = targetPosList[prevtarget];

            transform.LookAt(targetPosList[nexttarget]);
            ForceRespawn = false;
            IsGrounded = true;
        }

    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "ground" || other.gameObject.tag == "Asphalt" || other.gameObject.tag == "Grass")
        {
            IsGrounded = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "ground" || other.gameObject.tag == "Asphalt" || other.gameObject.tag == "Grass")
        {
            IsGrounded = true;
        }
        else if (other.gameObject.tag == "ForceRespawn")
        {
            ForceRespawn = true;

        }

        float speednerfer = maxSpeedHolder / 1000f;

        if (engineClass <= 2) //Bot levels 0-2
        {
            if (other.tag == "slowtarget") //Red Target
            {
                maxSpeed = maxSpeedHolder * (0.95f - speednerfer); //0.95f
            }
            else if (other.tag == "normaltarget") //Grey Target
            {
                maxSpeed = maxSpeedHolder * (1f/* - speednerfer*/); //0.8f
            }
            else if (other.tag == "fasttarget") //Green Target
            {
                maxSpeed = maxSpeedHolder;
            }
        }
        else //Bot levels 3-5
        {
            if (other.tag == "slowtarget") //Red Target
            {
                maxSpeed = maxSpeedHolder * (0.9f/* - speednerfer*/); //0.6f
            }
            else if (other.tag == "normaltarget") //Grey Target
            {
                maxSpeed = maxSpeedHolder * (1.0f/* - speednerfer*/); //0.8f
            }
            else if (other.tag == "fasttarget") //Green Target
            {
                maxSpeed = maxSpeedHolder;
            }
        }
    }

}
