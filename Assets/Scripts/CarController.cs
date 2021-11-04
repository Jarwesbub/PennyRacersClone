using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CarController : MonoBehaviour
{
    public GameObject Car;
    //public GameObject PlayerData;
    private GameObject GameController, AIController;
    public TMP_Text SpeedTxt;

    Rigidbody rb;
    Collider col;
    float oldEulerAngles;

    //MOBILE
    public FixedJoystick joystick;
    public bool useUIjoystick = false, UIbuttonPedals = false;
    //public float MobileTurningBuff;
    public int UIbuttonVertical; //1=gas, -1=brake; //This is vertical input on buttons "gas pedal and brake pedal"
    //slide power speed nerf
    public float TEST, TEST2; //DELETE WHEN NOT IN TEST USE!
    public float Steps, driftWaitSteps; //How many steps/fixed frames

    //Non changeable values (so far) horizDP and verNP are rigidbody force -values that gives better drifting experience
    //public float MinimumDriftValue;

    //Accessable values from other scripts: (can be changed anytime) all public floats
    public float Speed, EnginePower, Acceleration, MaxSpeed, Brake, Grip;
    public float Turning = 0.8f, BrakeTurn = 1.8f;
    public float DynFriction, StatFriction; //These are Car's current stats for rb physics material //Dynamic- and Static Friction

    [SerializeField] //FOR DEBUGGING
    private float SpeedLimiter, CarMass, CurrentAcceleration, DriftVal, DriftAccBuff, rbDriftBuff; //CurAcc/DriftCalc = show FixedUpdate values; PosZ = where Car is facing before spawning

    private float AddSpeed, ReverseSpeed, ReverseMaxSpeed, DriftValToAxis, CurrRotation; //AddSpeed must be 750f!
    Vector3 oldPosition; //Spawning
    private float LoseSpeed, AccLerpTime, HoldTurningValue, FrictionLevel;
    private float CarRbDrag, CarRbDragOnAir = 0f; //Fixes gravity inaccuracy when car is on ground/air (Changes rigidbody's "Drag" value)

    //[SerializeField] //FOR DEBUGGING
    private bool IsReverse = false;
    public bool GameStart = false, IsDrifting = false, IsAcc = false, IsBraking = false, IsGrounded, IsHitting = false, IsOnGrass = false;
    public bool ResetPlayer = false, Autopilot = false;
    private bool CooldownWait = false, ClutchWait = false;

    public float horizontalInput, verticalInput, prevHorizontalInput; //Turning values from axis (between values of -1 to 1)
    private bool horizontalInputIsNegative;

    void Awake()
    {
        GameController = GameObject.FindWithTag("GameController");
        AIController = GameObject.FindWithTag("aicontroller");
        if (useUIjoystick)
        {
            joystick = GameObject.FindWithTag("Joystick").GetComponent<FixedJoystick>();
            //Turning += MobileTurningBuff;
        }
        rbDriftBuff = 0f;
        Autopilot = false;
        Speed = 0f;
        SpeedLimiter = 1f;
        AddSpeed = 750f; //MUST BE 750f for this build
        ReverseMaxSpeed = 50f;
        DriftVal = 0.1f;/////Mobile bug??

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
    void Start()
    {
        if (!useUIjoystick) //Keyboard turning buff
            Turning *= 1.5f;

    }
    void DriftController(bool IsNotKeyboard)
    {
        rbDriftBuff = 0f;
        float input = horizontalInput;
        float driftStartLimit = 0.0f;//Works better with keyboard (buttons)
        if (useUIjoystick)
            driftStartLimit = 0.6f; //Works better with joystick (axis)
        
        if ((input > driftStartLimit || input < -driftStartLimit)/* || !IsNotKeyboard*/) // or if keyboard is active
        {
            if (!IsReverse && IsGrounded && Speed > 30f / 2f && !IsHitting)
            {
                float maxDriftVal = 4f;//5f normal
                float turn = Turning;

                if (horizontalInputIsNegative)
                    turn -= Turning * 2; //-- makes value negative

                if (IsNotKeyboard) //UIJoystick
                {
                    
                    if (IsAcc)
                    {
                        input *= 4f;//4f
                    }
                    float scale = 0.6f;
                    if (horizontalInputIsNegative && input < -scale || !horizontalInputIsNegative && input > scale)
                    {
                        DriftVal += input * (Speed / 10000f) * turn; //5000f
                        DriftAccBuff = 1f;

                        if (IsBraking)
                        {
                            DriftVal *= 1.05f;
                            DriftAccBuff = 1f;
                        }
                        if (IsAcc)
                        {
                            DriftAccBuff *= (0.01f / Grip); //(0.005f / Grip)perfect so far!
                            DriftVal += DriftAccBuff;
                        }
                    }
                    else
                        IsDrifting = false;

                }
                else if (!IsNotKeyboard)//KEYBOARD
                {
                    if (IsAcc)
                    {
                        input *= 4f;
                    }
                    float scale = 0.0f;
                    if (horizontalInputIsNegative && input < -scale || !horizontalInputIsNegative && input > scale)
                    {
                        DriftVal += input * (Speed / 10000f) * turn; //5000f
                        DriftAccBuff = 2f;

                        if (IsBraking)
                        {
                            DriftVal *= 1.05f;
                            DriftAccBuff = 4f;
                        }
                        if (IsAcc)
                        {
                            DriftAccBuff *= (0.01f / Grip); //(0.005f / Grip)perfect so far!
                            DriftVal += DriftAccBuff;
                        }
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

                //Final check for IsDrifting!
                if (GripControl < DriftVal || -GripControl > DriftVal)
                {
                    //float rbBuff = (0.25f*Speed) *(-turn * 3f); //2f
                    float rbBuff = (40f+(Speed * 0.1f)) * (-turn * 3f); //2f 
                    if (IsBraking)
                        rbBuff *= 2f;

                    rbDriftBuff = rbBuff;
                    IsDrifting = true;
                    DriftValToAxis = DriftVal;

                }
                else //Grip higher than DriftVal
                {
                    IsDrifting = false;
                }
            }
            else //Not grounded || too slow speed
            {
                DriftVal = 0f;
                DriftAccBuff = 0f;
                IsDrifting = false;
            }
        }
        else if (useUIjoystick)//Input is too low (when turning)
        {
            IsDrifting = false;
            
        }


    }


    void FixedUpdate()
    {
        Steps += Time.deltaTime;
        float cargravity = 100f;
        rb.AddRelativeForce(Vector3.down * Time.deltaTime * cargravity);

        if (GameStart && !ResetPlayer)
        {
            if (!Autopilot)
            {
                if (IsHitting == false)
                {
                    if (!useUIjoystick)
                    {
                        
                        horizontalInput = Input.GetAxis("Horizontal");
                        verticalInput = Input.GetAxis("Vertical");

                    }
                    else //UI JOYSTICK
                    {
                        horizontalInput = joystick.Horizontal; //TURNING

                        if (UIbuttonPedals) //USE GAS/BRAKE PEDALS
                            verticalInput = UIbuttonVertical;
                        else // JOYSTICK GAS/THROTTLE
                            verticalInput = joystick.Vertical;

                    }
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

                    DriftController(useUIjoystick);
                    //DriftController(false);
                    CarAllTurningInputs(DriftValToAxis);


                    CarGoForwardInputs();

                    CarGoBackwardsBrakingInputs();

                }
                else
                {
                    IsGrounded = false;

                    if (Speed < 30f && CooldownWait == false)
                    {
                        CooldownWait = true;
                        StartCoroutine(ReSpawnCooldownWait());
                    }
                }


                if (IsAcc == false && IsGrounded == true && CooldownWait == false)
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
                //Calculate speed based on objects movement speed
                Speed = Vector3.Distance(oldPosition, Car.transform.position) * 200f * SpeedLimiter; // Original = * 100f
                oldPosition = Car.transform.position;
            }
            else //Autopilot ON
            {
                //AutopilotON();

            }
        }
        else if (!GameStart)
        {
            GameStart = GameController.GetComponent<RaceControl>().GameStart;
        }
        else if (ResetPlayer)
        {
            rb.velocity = Vector3.zero;
            Speed = 0f;
        }
        
    }

    private void CarAllTurningInputs(float driftvalue)
    {
        if (Speed > 0)
        {
            if (Speed > 0.1f && IsGrounded)
            {

                float rotValue = Turning;


                if (IsBraking == true) //BACKWADS TURNING
                {
                    //float TurnValue = horizontalInput;
                    rotValue = (Turning * BrakeTurn)/* * TurnValue*/;
                }

                //CONTROLS LEFT TO RIGHT

                if (horizontalInput == 0f)
                {
                    DriftVal = 0f;
                    DriftValToAxis -= 0.1f;
                    
                    if (DriftValToAxis < 0.5f || !IsGrounded)
                        DriftValToAxis = 0f;
                    
                    IsDrifting = false;
                    rotValue = 0f;
                }

                if (horizontalInput > 0.01f) //TURN RIGHT
                {

                    rotValue *= driftvalue+1f;

                }
                if (horizontalInput < -0.01f) //TURN LEFT
                {
                    rotValue *= driftvalue+1f;

                }

                if (IsHitting == false && IsGrounded)
                {
                    rotValue *= horizontalInput;
                    Vector3 m_EulerAngleVelocity = new Vector3(0, rotValue * 30f, 0);
                    Quaternion deltaRotation = Quaternion.Euler(m_EulerAngleVelocity * Time.fixedDeltaTime);
                    rb.MoveRotation(rb.rotation * deltaRotation);

                }

            }
        }
    }

    private void CarGoForwardInputs()
    {
        //if (Input.GetKey("w") || Input.GetKey("up")) //GAS GAS GAS/////////////////////////////
        if(verticalInput > 0.01f) // GAS
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

                rb.AddRelativeForce(new Vector3(rbDriftBuff, -1f, DriftVal + CarMass * CurrentAcceleration * addspeed * Time.deltaTime)); //CarMass = rb.mass*10f
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
        if(verticalInput < -0.2f)
        {


            if (Speed > 1f)
            {

                IsBraking = true;

                rb.AddRelativeForce(new Vector3(rbDriftBuff, -1f, Brake * Time.deltaTime));



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
                    rb.AddRelativeForce(new Vector3(rbDriftBuff, -1f, SpeedLimiter * CarMass * ReverseSpeed * Time.deltaTime));//CarMass = rb.mass*10f
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
                rb.AddRelativeForce(new Vector3(rbDriftBuff, -1f, SpeedLimiter * CarMass * CurrentAcceleration * LoseSpeed * Time.deltaTime));//CarMass = rb.mass*10f
            }
            else
            {
                LoseSpeed -= Brake*(Brake/5); //
                rb.AddRelativeForce(new Vector3(rbDriftBuff, -1f, SpeedLimiter * CarMass * CurrentAcceleration * LoseSpeed * Time.deltaTime));//CarMass = rb.mass*10f

            }

        }
        else
        {
            LoseSpeed += 0.8f; // REVERSE

            rb.AddRelativeForce(new Vector3(rbDriftBuff, -1f, LoseSpeed * Time.deltaTime));

        }
    }
    /*
    private void AutopilotON() //When player finishes race -> no more player control -> "watch your car gameplay on the background"
    {
        Autopilot = true;
        AIController.GetComponent<AIController>().StartPlayerAutopilot();
    }*/

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
            //rb.velocity = Vector3.zero;
            Car.GetComponent<CarGroundControl>().PlayerRespawn(true);
            IsBraking = true;
        }

        CooldownWait = false;
    }

}
