using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//Full car controlling scheme stacked to one script
public class CarController : MonoBehaviour
{
    public GameObject Car;
    private GameObject GameController;
    public TMP_Text SpeedTxt; //Speed data for speed-o-meter

    Rigidbody rb;
    float oldEulerAngles;

    //MOBILE
    public FixedJoystick joystick;
    public bool useUIjoystick = false, UIbuttonPedals = false;

    public int UIbuttonVertical; 
    public float steps; //How many steps/fixed frames

    //Accessable values from other scripts: (can be changed anytime) all public floats
    public float speed, enginePower, acceleration, maxSpeed, brake, grip;
    public float turning = 0.8f, brakeTurn = 1.8f;
    //public float DynFriction, StatFriction; //These are Car's current stats for rb physics material //Dynamic- and Static Friction

    //[SerializeField] //FOR DEBUGGING
    private float speedLimiter, carMass, currentAcceleration, driftVal, driftAccBuff, rbDriftBuff; //CurAcc/DriftCalc = show FixedUpdate values; PosZ = where Car is facing before spawning

    private float addSpeed, reverseSpeed, reverseMaxSpeed, driftValToAxis; //AddSpeed must be 750f!
    Vector3 oldPosition; //Spawning
    private float loseSpeed, accLerpTime;
    private float carRbDrag, carRbDragOnAir = 0f; //Fixes gravity inaccuracy when car is on ground/air (Changes rigidbody's "Drag" value)

    //[SerializeField] //FOR DEBUGGING
    private bool isReverse = false;
    public bool gameStart = false, isDrifting = false, isAcc = false, isBraking = false, isGrounded, isHitting = false, isOnGrass = false;
    public bool resetPlayer = false, autopilot = false;
    private bool cooldownWait = false, clutchWait = false;

    public float horizontalInput, verticalInput, prevHorizontalInput; //Turning values from axis (between values of -1 to 1)
    private bool horizontalInputIsNegative;

    void Awake()
    {
        GameController = GameObject.FindWithTag("GameController");
        if (useUIjoystick)
        {
            joystick = GameObject.FindWithTag("Joystick").GetComponent<FixedJoystick>();
            //Turning += MobileTurningBuff;
        }
        rbDriftBuff = 0f;
        autopilot = false;
        speed = 0f;
        speedLimiter = 1f;
        addSpeed = 750f; //MUST BE 750f for this build
        reverseMaxSpeed = 50f;
        driftVal = 0.1f;/////Mobile bug??

        isBraking = false;
        isAcc = false;

        rb = Car.GetComponent<Rigidbody>(); 
        carRbDrag = rb.drag;
        carMass= Car.GetComponent<Rigidbody>().mass;
        carMass *= 10f;//This works very well with current rb.force build! NEVER CHANGE!
        oldEulerAngles = Car.transform.rotation.eulerAngles.y;

        //HoldTurningValue = Turning;
    }
    void Start()
    {
        if (!useUIjoystick) //Keyboard turning buff
            turning *= 1.5f;

    }
    void DriftController(bool IsNotKeyboard) //Control your drift values based on Speed- and turning values (in FixedUpdate) 
    {
        rbDriftBuff = 0f;
        float input = horizontalInput;
        float driftStartLimit = 0.0f;//Works better with keyboard (buttons)
        if (useUIjoystick)
            driftStartLimit = 0.6f; //Works better with joystick (axis)
        
        if ((input > driftStartLimit || input < -driftStartLimit)/* || !IsNotKeyboard*/) // or if keyboard is active
        {
            if (!isReverse && isGrounded && speed > 30f / 2f && !isHitting)
            {
                float maxDriftVal = 4f;//5f normal
                float turn = turning;

                if (horizontalInputIsNegative)
                    turn -= turning * 2; //-- makes value negative

                if (IsNotKeyboard) //UIJoystick
                {
                    
                    if (isAcc)
                    {
                        input *= 4f;//4f
                    }
                    float scale = 0.6f;
                    if (horizontalInputIsNegative && input < -scale || !horizontalInputIsNegative && input > scale)
                    {
                        driftVal += input * (speed / 10000f) * turn; //5000f
                        driftAccBuff = 1f;

                        if (isBraking)
                        {
                            driftVal *= 1.05f;
                            driftAccBuff = 1f;
                        }
                        if (isAcc)
                        {
                            driftAccBuff *= (0.01f / grip); //(0.005f / Grip)perfect so far!
                            driftVal += driftAccBuff;
                        }
                    }
                    else
                        isDrifting = false;

                }
                else if (!IsNotKeyboard)//KEYBOARD
                {
                    if (isAcc)
                    {
                        input *= 4f;
                    }
                    float scale = 0.0f;
                    if (horizontalInputIsNegative && input < -scale || !horizontalInputIsNegative && input > scale)
                    {
                        driftVal += input * (speed / 10000f) * turn; //5000f
                        driftAccBuff = 2f;

                        if (isBraking)
                        {
                            driftVal *= 1.05f;
                            driftAccBuff = 4f;
                        }
                        if (isAcc)
                        {
                            driftAccBuff *= (0.01f / grip); //(0.005f / Grip)perfect so far!
                            driftVal += driftAccBuff;
                        }
                    }
                }

                if (driftVal > maxDriftVal)
                {
                    driftVal = maxDriftVal;
                }
                else if (driftVal < -maxDriftVal)
                {
                    driftVal = -maxDriftVal;
                }

                float GripControl = grip;

                //Final check for IsDrifting!
                if (GripControl < driftVal || -GripControl > driftVal)
                {
                    //float rbBuff = (0.25f*Speed) *(-turn * 3f); //2f
                    float rbBuff = (40f+(speed * 0.1f)) * (-turn * 3f); //2f 
                    if (isBraking)
                        rbBuff *= 2f;

                    rbDriftBuff = rbBuff;
                    isDrifting = true;
                    driftValToAxis = driftVal;

                }
                else //Grip higher than DriftVal
                {
                    isDrifting = false;
                }
            }
            else //Not grounded || too slow speed
            {
                driftVal = 0f;
                driftAccBuff = 0f;
                isDrifting = false;
            }
        }
        else if (useUIjoystick)//Input is too low (when turning)
        {
            isDrifting = false;
            
        }


    }

    void FixedUpdate()
    {
        steps += Time.deltaTime;
        float cargravity = 100f;
        rb.AddRelativeForce(Vector3.down * Time.deltaTime * cargravity);

        if (gameStart && !resetPlayer)
        {
            if (!autopilot)
            {
                if (isHitting == false)
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

                if (isGrounded == true)
                {
                    rb.drag = carRbDrag;
                    carRbDrag = rb.drag; //DELETE THIS! ONLY FOR TESTING IN GAMEPLAY SCREEN
                }
                else
                {
                    rb.drag = carRbDragOnAir;
                }

                if (Car.GetComponent<CarGroundControl>().CarIsGrounded == true)
                {

                    isGrounded = true;

                    DriftController(useUIjoystick);
                    //DriftController(false);
                    CarAllTurningInputs(driftValToAxis);


                    CarGoForwardInputs();

                    CarGoBackwardsBrakingInputs();

                }
                else
                {
                    isGrounded = false;

                    if (speed < 30f && cooldownWait == false)
                    {
                        cooldownWait = true;
                        StartCoroutine(ReSpawnCooldownWait());
                    }
                }


                if (isAcc == false && isGrounded == true && cooldownWait == false)
                {
                    if (speed > 1f && loseSpeed > 0f && isReverse == false)
                    {
                        StopAcceleratingForward(true);
                    }
                    else if (speed > 1f && loseSpeed < 0f && isReverse == true)
                    {
                        StopAcceleratingForward(false);

                    }

                }
                //Calculate speed based on objects movement speed
                speed = Vector3.Distance(oldPosition, Car.transform.position) * 200f * speedLimiter; // Original = * 100f
                oldPosition = Car.transform.position;
            }
            else //Autopilot ON
            {
                //AutopilotON();

            }
        }
        else if (!gameStart)
        {
            gameStart = GameController.GetComponent<RaceControl>().GameStart;
        }
        else if (resetPlayer)
        {
            rb.velocity = Vector3.zero;
            speed = 0f;
        }
        
    }

    private void CarAllTurningInputs(float driftvalue) //Player's turning values combined with drift value
    {
        if (speed > 0)
        {
            if (speed > 0.1f && isGrounded)
            {

                float rotValue = turning;

                if (isBraking) //BACKWADS TURNING
                {
                    //float TurnValue = horizontalInput;
                    rotValue = (turning * brakeTurn)/* * TurnValue*/;
                }

                //CONTROLS LEFT TO RIGHT

                if (horizontalInput == 0f)
                {
                    driftVal = 0f;
                    driftValToAxis -= 0.1f;
                    
                    if (driftValToAxis < 0.5f || !isGrounded)
                        driftValToAxis = 0f;
                    
                    isDrifting = false;
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

                if (!isHitting && isGrounded)
                {
                    rotValue *= horizontalInput;
                    Vector3 m_EulerAngleVelocity = new Vector3(0, rotValue * 30f, 0);
                    Quaternion deltaRotation = Quaternion.Euler(m_EulerAngleVelocity * Time.fixedDeltaTime);
                    rb.MoveRotation(rb.rotation * deltaRotation);

                }

            }
        }
    }

    private void CarGoForwardInputs() //Player's forward moving control based on car's stats
    {
        //if (Input.GetKey("w") || Input.GetKey("up")) //GAS GAS GAS/////////////////////////////
        if(verticalInput > 0.01f) // GAS
        {
            isAcc = true;
            //LoseSpeed = AddSpeed;
            isReverse = false;

            clutchWait = true;

            if (speed < maxSpeed && isGrounded)
            {
                float addspeed = addSpeed;


                addspeed = 750f + (speed * enginePower); // 5f -> Speed = 360f // 1.5f = 100f
                

                float EnginePowerNerfer = 0.6f;
                float Spd = 10f;
                float AccBuffer = acceleration;
                float turn = turning;
                //Turning = HoldTurningValue;

                loseSpeed = addspeed;

                if (Spd < speed && speed < maxSpeed)
                {
                    do
                    {
                        EnginePowerNerfer -= 0.025f; //Perfect value for current EnginePower levels
                        Spd += 10f;
                        //AddSpeed += 1f;
                        AccBuffer += 0.01f; //0.01f
                        turn -= 0.05f;
                    }
                    while (Spd < speed);
                }

                accLerpTime = speed * EnginePowerNerfer;

                currentAcceleration = Mathf.Lerp(0.1f, AccBuffer, accLerpTime * Time.deltaTime);

                if (isBraking)
                    currentAcceleration *= 0.9f;

                rb.AddRelativeForce(new Vector3(rbDriftBuff, -1f, driftVal + carMass * currentAcceleration * addspeed * Time.deltaTime)); //CarMass = rb.mass*10f
            }

        }
        else
        {
            isAcc = false;
        }

    }

    private void CarGoBackwardsBrakingInputs() //Player's reverse moving input
    {
        //if (Input.GetKey("s") || Input.GetKey("down")/* && IsDrifting == false*/) //BREAKING
        if(verticalInput < -0.2f)
        {


            if (speed > 1f)
            {

                isBraking = true;

                rb.AddRelativeForce(new Vector3(rbDriftBuff, -1f, brake * Time.deltaTime));



                //Vec.x += -1 * Time.deltaTime * MoveSpeed;
            }
            else
            {
                isReverse = true;

            }

            if (speed < reverseMaxSpeed && isReverse == true)
            {
                if (clutchWait == true)
                {
                    StartCoroutine(ClutchWaitTime());
                }
                else
                {
                    reverseSpeed = -(150f + (enginePower * 10f));
                    rb.AddRelativeForce(new Vector3(rbDriftBuff, -1f, speedLimiter * carMass * reverseSpeed * Time.deltaTime));//CarMass = rb.mass*10f
                }
            }



        }
        else
        {
            isBraking = false;
        }
    }

    private void StopAcceleratingForward(bool NoReverse) //Player is not hitting gas pedal and no braking (On GROUND)
    {
        if (NoReverse)
        {
            if (!isBraking)
            {
                loseSpeed -= 1.8f; // How much car loses speed when not accelerating (per fixed frame)
                rb.AddRelativeForce(new Vector3(rbDriftBuff, -1f, speedLimiter * carMass * currentAcceleration * loseSpeed * Time.deltaTime));//CarMass = rb.mass*10f
            }
            else
            {
                loseSpeed -= brake*(brake/5); //
                rb.AddRelativeForce(new Vector3(rbDriftBuff, -1f, speedLimiter * carMass * currentAcceleration * loseSpeed * Time.deltaTime));//CarMass = rb.mass*10f

            }

        }
        else
        {
            loseSpeed += 0.8f; // REVERSE

            rb.AddRelativeForce(new Vector3(rbDriftBuff, -1f, loseSpeed * Time.deltaTime));

        }
    }
    /*
    private void AutopilotON() //When player finishes race -> no more player control -> "watch your car gameplay on the background"
    {
        Autopilot = true;
        AIController.GetComponent<AIController>().StartPlayerAutopilot();
    }*/

    void Update() // Draw Speed-o-meter values on string
    {
        float SpeedDecimal = Mathf.Round(speed * 1f);

        //SpeedTxt.text = "Speed: " + SpeedDecimal.ToString();
        SpeedTxt.text = SpeedDecimal.ToString();

    }

    // TIMED STUFF ->

    IEnumerator SpeedLimitTimer(float speedlimiter, float waitTime) //Little "wait time gap" between backwards- and forwards driving
    {
        speedLimiter = speedlimiter;
        yield return new WaitForSeconds(waitTime);
        speedLimiter = 1f;
        isHitting = false;
    }

    IEnumerator ClutchWaitTime() //Little "wait time gap" between backwards- and forwards driving
    {
        yield return new WaitForSeconds(0.4f);
        clutchWait = false;

    }

    IEnumerator ReSpawnCooldownWait() //When player cant move -> respawn in given time
    {
        
        yield return new WaitForSeconds(2f);

        if (isGrounded == false)
        {
            //rb.velocity = Vector3.zero;
            Car.GetComponent<CarGroundControl>().PlayerRespawn(true);
            isBraking = true;
        }

        cooldownWait = false;
    }

}
