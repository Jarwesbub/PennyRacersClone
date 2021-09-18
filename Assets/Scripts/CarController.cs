using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarController : MonoBehaviour
{
    public GameObject Car;
    public GameObject PlayerData;
    public Text SpeedTxt;

    Rigidbody rb;
    Collider col;

    private Quaternion originalRotation;
    private Quaternion currentRotation;

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

    //[SerializeField] //FOR DEBUGGING
    private float CarMass, CurrentAcceleration, DriftVal, DriftAccBuff, PosZ; //CurAcc/DriftCalc = show FixedUpdate values; PosZ = where Car is facing before spawning

    private float AddSpeed, ReverseSpeed, ReverseMaxSpeed, DriftValToAxis, CurrRotation; //AddSpeed must be 750f!
    Vector3 oldPosition; //Spawning
    private float LoseSpeed, AccLerpTime, HoldTurningValue, FrictionLevel;
    private float CarRbDrag, CarRbDragOnAir = 0f; //Fixes gravity inaccuracy when car is on ground/air (Changes rigidbody's "Drag" value)

    [SerializeField] //FOR DEBUGGING
    private bool IsTurning = false, IsAcc = false, IsReverse = false, IsDrifting = false;
    public bool IsBraking = false, IsGrounded, IsOnGrass = false;
    private bool CooldownWait = false, ClutchWait = false;

    public float horizontalInput, verticalInput; //Turning values from axis (between values of -1 to 1)
    private bool horizontalInputIsNegative;


    void Awake()
    {
        Speed = 0f;
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
            Debug.Log("HIT");
            col.material = normalFriction;
        }
        else if (FricLvl == 3) //Wall hit
        {
            Debug.Log("HIT");
            horizontalInput = 0f;
            col.material = normalFriction;
        }
        else if (FricLvl == -1) //Car is upsidedown
        {
            IsOnGrass = false;
            col.material = normalFriction;
        }

    }

    void DriftController()
    {

        if (IsReverse == false && IsGrounded == true && Speed > 30f / 2f)
            //if (DriftCalculator > MinimumDriftValue || -DriftCalculator > MinimumDriftValue)
            {
            float input = horizontalInput;
            bool isnegative = horizontalInputIsNegative;
            float maxDriftVal = 2f;//5f normal

            if (isnegative)
                input -= 0.5f;
            else
                input += 0.5f;

            input = Mathf.Lerp(0f, input, 100f * Time.deltaTime);

            float min = 1f; //0.1f
            float turn = Turning;
            min = MinimumDriftValue;

                if (isnegative == true)
                    turn -= Turning * 2; //-- makes value positive


                if (isnegative == true && input < 0f || isnegative == false && input > 0f)
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
                
                if (DriftVal < min && DriftVal > -min)
                {
                    DriftVal = 0f;
                    DriftValToAxis = 0f;
                    IsDrifting = false;

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
            if (IsOnGrass == true)
                GripControl = Grip * 0.25f; //How much grip on grass
                

                if (GripControl < DriftVal || -GripControl > DriftVal)
                {
                    float SlidePower = -300f;//-300f How much drifting gives horizontal power
                    float SpeedNerf = -100f;//-100f Balances SlidePower
                float horizontaldriftpower = -180f;
                float verticaldriftpower = -20f;

                    SlidePower = horizontaldriftpower; //-72f -100
                    SpeedNerf = EnginePower * verticaldriftpower; //-24f -20
                /*
                    if (IsBraking == true)
                    {
                       SlidePower = SlidePower * 1.1f;
                       SpeedNerf = SlidePower * 1.1f;
                    }
                */
                    rb.AddRelativeForce(new Vector3(input * Mathf.Lerp(0, SlidePower, 1f * Time.deltaTime), -1f, SpeedNerf * Time.deltaTime));
                    IsDrifting = true;
                    //LoseSpeed = AddSpeed;

                    DriftValToAxis = DriftVal;

                       GetFrictionValues(0); //Drift
                }

        }
        else
        {
            IsDrifting = false;
            
        }
        
        if (IsDrifting == false && IsOnGrass == false)
        {
            GetFrictionValues(1); //Normal

        }
        


    }


    void FixedUpdate()
    {
        Steps += Time.deltaTime;

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");


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
        

        Speed = Vector3.Distance(oldPosition, Car.transform.position) * 200f; // Original = * 100f
        oldPosition = Car.transform.position;

    }

    private void CarAllTurningInputs(float driftvalue)
    {

        if (Speed > 0)
        {

            if (Speed > 0.1f && IsGrounded == true)
            {

                float rotValue = Turning;
                bool IsNegative = false;

                if (horizontalInputIsNegative == true)
                {
                    IsNegative = true;
                }
                else
                {
                    IsNegative = false;
                }

                if (IsBraking == true) //BACKWADS TURNING
                {
                    float TurnValue = horizontalInput;
                    

                    if (IsNegative == true)
                    {
                        TurnValue = -TurnValue;
                    }

                    rotValue = (Turning * BrakeTurn) * TurnValue;
                }
                else
                {
                    rotValue = Turning;

                }

                //CONTROLS LEFT TO RIGHT

                //if (horizontalInput < 0.1f && horizontalInput > -0.1f)
                if (horizontalInput == 0f)
                {
                    //driftvalue = 0f;
                    DriftVal = 0f;
                    
                    DriftValToAxis -= 0.1f;

                    if (DriftValToAxis < 0.1f)
                        DriftValToAxis = 0f;


                        IsTurning = false;
                    IsDrifting = false;
                    rotValue = 0f;
                }
               

                if (horizontalInput > 0.01f) //TURN RIGHT
                {
                    IsTurning = true;
                    rotValue *= horizontalInput; //Turning variable
                       
                    if (driftvalue < rotValue)
                    {
                        driftvalue = +driftvalue;
                        
                    }

                    //rotValue = Mathf.Lerp(0f, rotValue, 100f * Time.deltaTime);

                    rotValue += driftvalue;
                    

                    horizontalInputIsNegative = false;

                }
                if (horizontalInput < -0.01f) //TURN LEFT
                {
                    IsTurning = true;
                    rotValue *= horizontalInput; //Turning variable

                    if (driftvalue > rotValue)
                    {
                        driftvalue = +driftvalue;
                        
                    }
                    
                    rotValue -= driftvalue;

                    horizontalInputIsNegative = true;

                }
                else
                {
                    //Car.transform.Rotate(new Vector3(0f, 1f, 0f), rotValue*0.99f);


                }
                
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
                

                if (horizontalInput != 0f)
                    Car.transform.Rotate(new Vector3(0f,1f, 0f), rotValue);
                else if (driftvalue!=0f&&IsDrifting)
                    Car.transform.Rotate(new Vector3(0f, 1f, 0f), driftvalue);

                Debug.Log(driftvalue);


                //DriftValToAxis = 0f;

            }

        }
    }

    private void CarGoForwardInputs()
    {
        if (Input.GetKey("w") || Input.GetKey("up")) //GAS GAS GAS/////////////////////////////
        {
            IsAcc = true;
            //LoseSpeed = AddSpeed;
            IsReverse = false;
            IsBraking = false;
            ClutchWait = true;

            if (Speed < MaxSpeed && IsGrounded == true)
            {
                float addspeed = AddSpeed;


                addspeed = 750f + (Speed * EnginePower); // 5f -> Speed = 360f // 1.5f = 100f
                

                float EnginePowerNerfer = 0.6f;
                float Spd = 10f;
                float AccBuffer = Acceleration;
                float turn = Turning;
                //Turning = HoldTurningValue;

                if (IsDrifting == true && IsOnGrass == false)
                {
                    addspeed = addspeed * (0.8f + (EnginePower / 20f));

                }

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

                if (IsOnGrass == true && Speed > 5f)
                {
                    //AccLerpTime *= 0.75f;
                    AccBuffer *= 0.8f;
                }

                CurrentAcceleration = Mathf.Lerp(0.1f, AccBuffer, AccLerpTime * Time.deltaTime);

                //CurrentAcc = AccBuffer;

                rb.AddRelativeForce(new Vector3(0, -1f, CarMass * CurrentAcceleration * addspeed * Time.deltaTime)); //CarMass = rb.mass*10f
            }

        }
        else
        {
            IsAcc = false;
        }

    }

    private void CarGoBackwardsBrakingInputs()
    {
        if (Input.GetKey("s") || Input.GetKey("down")/* && IsDrifting == false*/) //BREAKING
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
                    rb.AddRelativeForce(new Vector3(0, -1f, CarMass * ReverseSpeed * Time.deltaTime));//CarMass = rb.mass*10f
                }
            }



        }
    }

    private void StopAcceleratingForward(bool NoReverse) //GROUND
    {
        if (NoReverse == true)
        {
            if (IsBraking == false)
            {
                LoseSpeed -= 1.8f; // How much car loses speed when not accelerating (per fixed frame)
                rb.AddRelativeForce(new Vector3(0, -1f, CarMass * CurrentAcceleration * LoseSpeed * Time.deltaTime));//CarMass = rb.mass*10f
            }
            else
            {
                LoseSpeed -= Brake*(Brake/5); //
                rb.AddRelativeForce(new Vector3(0, -1f, CarMass * CurrentAcceleration * LoseSpeed * Time.deltaTime));//CarMass = rb.mass*10f

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

        SpeedTxt.text = "Speed: " + SpeedDecimal.ToString();


    }


    // TIMED STUFF ->

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
