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
    //slide power speed nerf
    public float TEST; //DELETE WHEN NOT IN TEST USE!
    public float Timer; //ONLY FOR THE SHOW

    //Non changeable values (so far) horizDP and verNP are rigidbody force -values that gives better drifting experience
    public float horizontalDriftPower, verticalNerfPower, MinimumDriftValue;

    //Accessable values from other scripts: (can be changed anytime) all public floats
    public float Speed, EnginePower, Acceleration, MaxSpeed, Brake, Grip;
    public float Turning = 0.8f, BrakeTurn = 1.8f;
    public float DynFriction, StatFriction; //These are Car's current stats for rb physics material //Dynamic- and Static Friction

    [SerializeField] //Only for showing debug
    private float CurrentAcceleration, DriftCalculator, DriftVal, DriftAccBuff, PosZ; //CurAcc/DriftCalc = show FixedUpdate values; PosZ = where Car is facing before spawning

    private float AddSpeed, ReverseSpeed, ReverseMaxSpeed, DriftValToAxis, CurrRotation; //AddSpeed must be 750f!
    Vector3 oldPosition; //Spawning
    private float LoseSpeed, AccLerpTime, HoldTurningValue, GripControl, FrictionLevel;
    private float CarRbDrag, CarRbDragOnAir = 0f; //Fixes gravity inaccuracy when car is on ground/air (Changes rigidbody's "Drag" value)

    [SerializeField] //FOR DEBUGGING
    public bool IsBraking = false, IsAcc = false, IsReverse = false, IsDrifting = false, DriftRecorvering = false;
    public bool IsGrounded, CooldownWait = false, ClutchWait = false;

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

        col = Car.GetComponent<Collider>();


        HoldTurningValue = Turning;
    }

    public void GetFrictionValues(int FricLvl) //These values are easily changeable
    {
        var DynMat = col.material.dynamicFriction;

        FrictionLevel = FricLvl;

        if (FricLvl == -1) //Car is upsidedown
        {
            DynFriction = 0.6f;
            StatFriction = 0.6f;

        }


        if (FricLvl == 0) // DRIFT
        {
            DynFriction = 0f;
            StatFriction = 0f;

            col.material.frictionCombine = PhysicMaterialCombine.Multiply;
        }
        else
        {
            col.material.frictionCombine = PhysicMaterialCombine.Average;
        }

        if (FricLvl == 1)
        {
            DynFriction = 0.6f;
            StatFriction = 0.6f;

        }

        col.material.dynamicFriction = DynFriction;
        col.material.staticFriction = StatFriction;


    }

    void DriftController()
    {
        /*
        if (IsAcc == true)
        {
            DriftCalculator = input * Speed * Turning * EnginePower;
        }
        else
        {
            //DriftCalculator = (input * Speed * Turning * EnginePower) / TEST;

        }
        */
        

        if (IsReverse == false && IsGrounded == true)
            //if (DriftCalculator > MinimumDriftValue || -DriftCalculator > MinimumDriftValue)
            {
            float input = horizontalInput;
            bool isnegative = horizontalInputIsNegative;
            float maxDriftVal = 5f;

            input = Mathf.Lerp(0f, input, 100f * Time.deltaTime);

            float min = 1f; //0.1f
            float turn = Turning;
            min = MinimumDriftValue;

                if (isnegative == true)
                    turn -= Turning * 2; //-- makes value positive


                if (isnegative == true && input < 0f || isnegative == false && input > 0f)
                {
                    DriftVal += input * (Speed / 5000f) * turn;

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
                else
                {
                
                }
               

                if (DriftVal > maxDriftVal)
                {
                    DriftVal = maxDriftVal;
                }
                else if (DriftVal < -maxDriftVal)
                {
                    DriftVal = -maxDriftVal;
                }

                GripControl = Grip;
                

                if (GripControl < DriftVal || -GripControl > DriftVal/* && IsDrifting == true*/)
                {
                    float SlidePower = -300f;//How much drifting gives horizontal power
                    float SpeedNerf = -100f;//Balances SlidePower

                    SlidePower = horizontalDriftPower; //-72f -100
                    SpeedNerf = EnginePower * verticalNerfPower; //-24f -20
                /*
                    if (IsBraking == true)
                    {
                       SlidePower = SlidePower * 1.1f;
                       SpeedNerf = SlidePower * 1.1f;
                    }
                */
                rb.AddRelativeForce(new Vector3(input * Mathf.Lerp(0, SlidePower, 1f * Time.deltaTime), 0, SpeedNerf * Time.deltaTime));
                    IsDrifting = true;
                    //LoseSpeed = AddSpeed;

                    DriftValToAxis = DriftVal;

                       GetFrictionValues(0); //Drift
                }

        }
        //else
        {

            
        }

        if (IsDrifting == false)
        {
            GetFrictionValues(1); //Normal

        }



    }


    void FixedUpdate()
    {
        Timer += Time.deltaTime;

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

                /*
                if (IsDrifting == false && IsAcc == false)
                {

                    driftvalue = 0f;
                }
                */
                
                
                //CONTROLS LEFT TO RIGHT
                
                if (horizontalInput < 0.1f && horizontalInput > -0.1f)
                {
                    driftvalue = 0f;
                    DriftVal = 0f;
                    DriftValToAxis = 0f;
                    IsDrifting = false;
                }
               

                if (horizontalInput > 0.01f) //TURN RIGHT
                {
                    rotValue *= horizontalInput; //Turning variable
                       
                    if (driftvalue < rotValue)
                    {
                        driftvalue = +driftvalue;
                        
                    }
                       
                    rotValue += driftvalue;

                    Car.transform.Rotate(Vector3.up, rotValue);
                    horizontalInputIsNegative = false;

                }
                if (horizontalInput < -0.01f) //TURN LEFT
                {
                    rotValue *= horizontalInput; //Turning variable

                    if (driftvalue > rotValue)
                    {
                        driftvalue = +driftvalue;
                        
                    }
                    
                    rotValue -= driftvalue;

                    Car.transform.Rotate(Vector3.up, rotValue);
                    horizontalInputIsNegative = true;

                }

                //CurrRotation = rotValue;
                DriftValToAxis = 0f;
                //DriftVal = driftvalue;

                //FrictionControl();

            }

        }
    }

    private void CarGoForwardInputs()
    {
        if (Input.GetKey("w") || Input.GetKey("up")) //GAS GAS GAS/////////////////////////////
        {
            IsAcc = true;
            LoseSpeed = AddSpeed;
            IsReverse = false;
            IsBraking = false;
            ClutchWait = true;

            if (Speed < MaxSpeed)
            {
                float addspeed = AddSpeed;


                addspeed = 750f + (Speed * EnginePower); // 5f -> Speed = 360f // 1.5f = 100f


                float EnginePowerNerfer = 0.6f;
                float Spd = 10f;
                float AccBuffer = Acceleration;

                Turning = HoldTurningValue;

                if (IsDrifting == true || DriftRecorvering == true)
                {
                    addspeed = addspeed * (0.8f + (EnginePower / 20f));

                }



                if (Spd < Speed && Speed < MaxSpeed/* && IsSpinning == false*/)
                {
                    do
                    {
                        EnginePowerNerfer -= 0.025f; //Perfect value for current EnginePower levels
                        Spd += 10f;
                        //AddSpeed += 1f;
                        AccBuffer += 0.01f; //0.01f
                        Turning -= 0.05f;
                    }
                    while (Spd < Speed);
                }



                AccLerpTime = Speed * EnginePowerNerfer;

                CurrentAcceleration = Mathf.Lerp(0.1f, AccBuffer, AccLerpTime * Time.deltaTime);

                //CurrentAcc = AccBuffer;

                rb.AddRelativeForce(new Vector3(0, 0, CurrentAcceleration * addspeed * Time.deltaTime));
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

                rb.AddRelativeForce(new Vector3(0, 0, Brake * Time.deltaTime));



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
                    rb.AddRelativeForce(new Vector3(0, 0, ReverseSpeed * Time.deltaTime));
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
                LoseSpeed -= 0.8f; // How much car loses speed when not accelerating (per fixed frame)
                rb.AddRelativeForce(new Vector3(0, 0, CurrentAcceleration * LoseSpeed * Time.deltaTime));
            }
            else
            {
                LoseSpeed -= Brake*(Brake/10); //
                rb.AddRelativeForce(new Vector3(0, 0, CurrentAcceleration * LoseSpeed * Time.deltaTime));
                
            }

        }
        else
        {
            LoseSpeed += 0.8f; // REVERSE

            rb.AddRelativeForce(new Vector3(0, 0, LoseSpeed * Time.deltaTime));

        }
    }



    void Update() // UI STUFF
    {
        float SpeedDecimal = Mathf.Round(Speed * 1f) * 2; //ADDED double speed -> looks better ingame

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
            rb.AddRelativeForce(new Vector3(0, 0, 0));
            IsBraking = true;
        }

        CooldownWait = false;
    }

}
