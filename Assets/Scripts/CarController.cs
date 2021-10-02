using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CarController : MonoBehaviour
{
    public GameObject Car;
    public GameObject PlayerData;
    public GameObject GameController;
    public TMP_Text SpeedTxt;

    Rigidbody rb;
    Collider col;
    float oldEulerAngles; 

    public PhysicMaterial normalFriction, driftFriction,grassFriction;
    //slide power speed nerf
    public float TEST; //DELETE WHEN NOT IN TEST USE!
    public float Steps, driftWaitSteps; //How many steps/fixed frames

    //Non changeable values (so far) horizDP and verNP are rigidbody force -values that gives better drifting experience
    public float MinimumDriftValue;

    //Accessable values from other scripts: (can be changed anytime) all public floats
    public float Speed, EnginePower, Acceleration, MaxSpeed, Brake, Grip;
    public float Turning = 0.8f, BrakeTurn = 1.8f;
    public float DynFriction, StatFriction; //These are Car's current stats for rb physics material //Dynamic- and Static Friction

    [SerializeField] //FOR DEBUGGING
    private float SpeedLimiter, CarMass, CurrentAcceleration, DriftVal, DriftAccBuff, PosZ; //CurAcc/DriftCalc = show FixedUpdate values; PosZ = where Car is facing before spawning

    private float AddSpeed, ReverseSpeed, ReverseMaxSpeed, DriftValToAxis, CurrRotation; //AddSpeed must be 750f!
    Vector3 oldPosition; //Spawning
    private float LoseSpeed, AccLerpTime, HoldTurningValue, FrictionLevel;
    private float CarRbDrag, CarRbDragOnAir = 0f; //Fixes gravity inaccuracy when car is on ground/air (Changes rigidbody's "Drag" value)

    [SerializeField] //FOR DEBUGGING
    private bool /*IsTurning = false, */IsReverse = false;
    public bool IsDrifting = false, IsAcc = false, IsBraking = false, IsGrounded, IsHitting = false, IsOnGrass = false;
    private bool GameStart = false, CooldownWait = false, ClutchWait = false;

    public float horizontalInput, verticalInput; //Turning values from axis (between values of -1 to 1)
    private bool horizontalInputIsNegative;


    void Awake()
    {
        if (GameController == null)
            GameObject.FindWithTag("GameController");


        Speed = 0f;
        SpeedLimiter = 1f;
        AddSpeed = 750f; //MUST BE 750f for this build
        ReverseMaxSpeed = 50f;
        FrictionLevel = 1;

        //IsGrounded = false; //TESTING DELETE THIS

        IsBraking = false;
        IsAcc = false;

        rb = Car.GetComponent<Rigidbody>(); 
        CarRbDrag = rb.drag;
        CarMass= Car.GetComponent<Rigidbody>().mass;
        CarMass *= 10f;//This works very well with current rb.force build! NEVER CHANGE!
        col = Car.GetComponent<Collider>();
        oldEulerAngles = Car.transform.rotation.eulerAngles.y;

        //HoldTurningValue = Turning;
    }

    public void GetFrictionValues(int FricLvl) //These values are easily changeable
    {
        //var DynMat = col.material.dynamicFriction;
        FrictionLevel = FricLvl;

        if (FricLvl == 0) // DRIFT
        {
            col.material = driftFriction;
        }
        else if (FricLvl == 1)
        {
            //DynFriction = 0.6f; normal values
            //StatFriction = 0.6f;
            col.material = normalFriction;
        }
        else if (FricLvl == 2) //Enemy hit
        {
            IsHitting = true;
            Debug.Log("HIT");
            col.material = normalFriction;
            StartCoroutine(SpeedLimitTimer(1.0f,0.2f)); //SpeedLimit value, time
        }
        else if (FricLvl == 3) //Wall hit
        {
            Debug.Log("HIT");
            col.material = normalFriction;
            StartCoroutine(SpeedLimitTimer(0.1f,0.4f)); //SpeedLimit value, time
        }
        else if (FricLvl == -1) //Car is upsidedown
        {
            IsOnGrass = false;
            col.material = normalFriction;
        }

    }

    void DriftController()
    {

        if (IsReverse == false && IsGrounded == true && Speed > 30f / 2f && IsHitting == false)
        {
            float input = horizontalInput;
            float maxDriftVal = 4f;//5f normal

            if(IsAcc)
                input *= 4f;
            //input = Mathf.Lerp(0f, input, 100f * Time.deltaTime);
            float turn = Turning;

                if (horizontalInputIsNegative == true)
                    turn -= Turning * 2; //-- makes value negative


                if (horizontalInputIsNegative == true && input < 0f || horizontalInputIsNegative == false && input > 0f)
                {
                    DriftVal += input * (Speed / 10000f) * turn; //5000f

                //Mathf.Lerp(0f, DriftVal, TEST * Time.deltaTime);
                    DriftAccBuff = 1f;

                    if (IsBraking == true)
                    {
                        DriftVal *= 1.05f;
                        DriftAccBuff = 1f;
                    }

                    if (IsAcc == true)
                    {
                        DriftAccBuff *= (0.005f / Grip); //perfect so far!
                        DriftVal += DriftAccBuff;
                    }   


                }

                if (DriftVal > maxDriftVal)
                {
                    DriftVal = maxDriftVal;
                }
                else if (DriftVal < -maxDriftVal)
                {
                    DriftVal = -maxDriftVal;
                }
            
                float GripControl = Grip;
                

                if (GripControl < DriftVal || -GripControl > DriftVal)
                {
                /*
                    float SlidePower = -300f;//-300f How much drifting gives horizontal power
                    float SpeedNerf = -100f;//-100f Balances SlidePower
                float horizontaldriftpower = -180f;
                float verticaldriftpower = -20f;

                    SlidePower = horizontaldriftpower; //-72f -100
                    SpeedNerf = EnginePower * verticaldriftpower; //-24f -20

                    //rb.AddRelativeForce(new Vector3(input * Mathf.Lerp(0, SlidePower, 1f * Time.deltaTime), -1f * Time.deltaTime, SpeedNerf * Time.deltaTime));
                */
                    IsDrifting = true;

                    DriftValToAxis = DriftVal;

                    GetFrictionValues(0); //Drift
                }

        }
        else
        {
            IsDrifting = false;
            
        }
        
        if (IsDrifting == false)
        {
            GetFrictionValues(1); //Normal

        }
        


    }


    void FixedUpdate()
    {
        Steps += Time.deltaTime;

        if (GameStart)
        {
            if (IsHitting == false)
            {
                horizontalInput = Input.GetAxis("Horizontal");
                verticalInput = Input.GetAxis("Vertical");
            }
            else
            {
                horizontalInput = 0f;
            }
            float newEulerAngles = Car.transform.rotation.eulerAngles.y;

            if (oldEulerAngles < newEulerAngles - 0.2f)//When rotating RIGHT
            {
                horizontalInputIsNegative = false;
            }
            else if (oldEulerAngles > newEulerAngles + 0.2f)//When rotating LEFT
            {
                horizontalInputIsNegative = true;
            }

            oldEulerAngles = Car.transform.rotation.eulerAngles.y;

            if (IsGrounded == true)
            {
                rb.drag = CarRbDrag;
                CarRbDrag = rb.drag; //DELETE THIS! ONLY FOR TESTING IN GAMEPLAY SCREEN
            }
            else
            {
                rb.drag = CarRbDragOnAir;
            }

            if (Car.GetComponent<CarGroundControl>().CarIsGrounded == true)
            {

                IsGrounded = true;

                DriftController();
                CarAllTurningInputs(DriftValToAxis);


                CarGoForwardInputs();

                CarGoBackwardsBrakingInputs();

            }
            else
            {
                IsGrounded = false;

                if (Speed < 0.5 && CooldownWait == false)
                {
                    CooldownWait = true;
                    StartCoroutine(ReSpawnCooldownWait());
                }
            }


            if (IsAcc == false && IsGrounded == true)
            {
                if (Speed > 1f && LoseSpeed > 0f && IsReverse == false)
                {
                    StopAcceleratingForward(true);
                }
                else if (Speed > 1f && LoseSpeed < 0f && IsReverse == true)
                {
                    StopAcceleratingForward(false);

                }

            }
        }
        else
        {
            GameStart = GameController.GetComponent<RaceControl>().GameStart;
        }

        Speed = Vector3.Distance(oldPosition, Car.transform.position) * 200f * SpeedLimiter; // Original = * 100f

        oldPosition = Car.transform.position;

    }

    private void CarAllTurningInputs(float driftvalue)
    {

        if (Speed > 0)
        {

            if (Speed > 0.1f && IsGrounded == true)
            {

                float rotValue = Turning;


                if (IsBraking == true) //BACKWADS TURNING
                {
                    float TurnValue = horizontalInput;
                    /*
                    if (IsNegative == true)
                    {
                        TurnValue = -TurnValue;
                    }
                    */
                    rotValue = (Turning * BrakeTurn)/* * TurnValue*/;
                }

                //CONTROLS LEFT TO RIGHT

                //if (horizontalInput < 0.1f && horizontalInput > -0.1f)
                if (horizontalInput == 0f)
                {
                    //driftvalue = 0f;
                    DriftVal = 0f;
                    
                    DriftValToAxis -= 0.1f;
                    
                    if (DriftValToAxis < 0.5f || !IsGrounded)
                        DriftValToAxis = 0f;
                    
                    IsDrifting = false;
                    rotValue = 0f;
                }
               

                if (horizontalInput > 0.01f) //TURN RIGHT
                {
                    //rotValue *= horizontalInput; //Turning variable
                       /*
                    if (driftvalue < rotValue)
                    {
                        driftvalue = +driftvalue;
                        
                    }
                       */
                    //rotValue = Mathf.Lerp(0f, rotValue, 100f * Time.deltaTime);

                    rotValue *= driftvalue+1f;
                    

                    //horizontalInputIsNegative = false;

                }
                if (horizontalInput < -0.01f) //TURN LEFT
                {
                    //rotValue *= horizontalInput; //Turning variable
                    /*
                    if (driftvalue > rotValue)
                    {
                        driftvalue = +driftvalue;
                        
                    }
                    */
                    rotValue *= driftvalue+1f;

                    //horizontalInputIsNegative = true;

                }
                /*
                if (IsAcc&&IsDrifting)
                {
                    if (horizontalInputIsNegative)
                    {
                        rotValue -= 0.1f;
                    }
                    else
                    {
                        rotValue += 0.1f;
                    }

                }
                */
                /*
                if (horizontalInput != 0f)
                    Car.transform.Rotate(new Vector3(0f,1f, 0f), rotValue);
                else if (driftvalue!=0f&&IsDrifting)
                    Car.transform.Rotate(new Vector3(0f, 1f, 0f), driftvalue);
                */

                if (IsHitting == false && IsGrounded)
                {
                    rotValue *= horizontalInput;
                    Vector3 m_EulerAngleVelocity = new Vector3(0, rotValue * 30f, 0);
                    Quaternion deltaRotation = Quaternion.Euler(m_EulerAngleVelocity * Time.fixedDeltaTime);
                    rb.MoveRotation(rb.rotation * deltaRotation);

                }

                //DriftValToAxis = 0f;

            }

        }
    }

    private void CarGoForwardInputs()
    {
        //if (Input.GetKey("w") || Input.GetKey("up")) //GAS GAS GAS/////////////////////////////
        if(verticalInput > 0) // GAS
        {
            IsAcc = true;
            //LoseSpeed = AddSpeed;
            IsReverse = false;

            ClutchWait = true;

            if (Speed < MaxSpeed && IsGrounded)
            {
                float addspeed = AddSpeed;


                addspeed = 750f + (Speed * EnginePower); // 5f -> Speed = 360f // 1.5f = 100f
                

                float EnginePowerNerfer = 0.6f;
                float Spd = 10f;
                float AccBuffer = Acceleration;
                float turn = Turning;
                //Turning = HoldTurningValue;

                LoseSpeed = addspeed;

                if (Spd < Speed && Speed < MaxSpeed)
                {
                    do
                    {
                        EnginePowerNerfer -= 0.025f; //Perfect value for current EnginePower levels
                        Spd += 10f;
                        //AddSpeed += 1f;
                        AccBuffer += 0.01f; //0.01f
                        turn -= 0.05f;
                    }
                    while (Spd < Speed);
                }

                AccLerpTime = Speed * EnginePowerNerfer;

                CurrentAcceleration = Mathf.Lerp(0.1f, AccBuffer, AccLerpTime * Time.deltaTime);

                if (IsBraking)
                    CurrentAcceleration *= 0.9f;

                rb.AddRelativeForce(new Vector3(0, -1f, DriftVal + CarMass * CurrentAcceleration * addspeed * Time.deltaTime)); //CarMass = rb.mass*10f
            }

        }
        else
        {
            IsAcc = false;
        }

    }

    private void CarGoBackwardsBrakingInputs()
    {
        //if (Input.GetKey("s") || Input.GetKey("down")/* && IsDrifting == false*/) //BREAKING
        if(verticalInput < 0)
        {


            if (Speed > 1f)
            {

                IsBraking = true;

                rb.AddRelativeForce(new Vector3(0, -1f, Brake * Time.deltaTime));



                //Vec.x += -1 * Time.deltaTime * MoveSpeed;
            }
            else
            {
                IsReverse = true;

            }

            if (Speed < ReverseMaxSpeed && IsReverse == true)
            {
                if (ClutchWait == true)
                {
                    StartCoroutine(ClutchWaitTime());
                }
                else
                {
                    ReverseSpeed = -(150f + (EnginePower * 10f));
                    rb.AddRelativeForce(new Vector3(0, -1f, SpeedLimiter * CarMass * ReverseSpeed * Time.deltaTime));//CarMass = rb.mass*10f
                }
            }



        }
        else
        {
            IsBraking = false;
        }
    }

    private void StopAcceleratingForward(bool NoReverse) //GROUND
    {
        if (NoReverse == true)
        {
            if (IsBraking == false)
            {
                LoseSpeed -= 1.8f; // How much car loses speed when not accelerating (per fixed frame)
                rb.AddRelativeForce(new Vector3(0, -1f, SpeedLimiter * CarMass * CurrentAcceleration * LoseSpeed * Time.deltaTime));//CarMass = rb.mass*10f
            }
            else
            {
                LoseSpeed -= Brake*(Brake/5); //
                rb.AddRelativeForce(new Vector3(0, -1f, SpeedLimiter * CarMass * CurrentAcceleration * LoseSpeed * Time.deltaTime));//CarMass = rb.mass*10f

            }

        }
        else
        {
            LoseSpeed += 0.8f; // REVERSE

            rb.AddRelativeForce(new Vector3(0, -1f, LoseSpeed * Time.deltaTime));

        }
    }



    void Update() // UI STUFF
    {
        float SpeedDecimal = Mathf.Round(Speed * 1f);

        //SpeedTxt.text = "Speed: " + SpeedDecimal.ToString();
        SpeedTxt.text = SpeedDecimal.ToString();

    }

    // TIMED STUFF ->

    IEnumerator SpeedLimitTimer(float speedlimiter, float waitTime) //Little "wait time gap" between backwards- and forwards driving
    {
        SpeedLimiter = speedlimiter;
        yield return new WaitForSeconds(waitTime);
        SpeedLimiter = 1f;
        IsHitting = false;
    }

    IEnumerator ClutchWaitTime() //Little "wait time gap" between backwards- and forwards driving
    {
        yield return new WaitForSeconds(0.4f);
        ClutchWait = false;

    }

    IEnumerator ReSpawnCooldownWait() //When player cant move -> respawn in given time
    {

        yield return new WaitForSeconds(2f);

        if (IsGrounded == false)
        {
            PosZ = Car.transform.rotation.z;
            Car.transform.rotation = Quaternion.Euler(0, PosZ + 0f, 0); //+90f works too

            Speed = 0f;
            rb.AddRelativeForce(new Vector3(0f, 0f, 0f));
            IsBraking = true;
        }

        CooldownWait = false;
    }

}
