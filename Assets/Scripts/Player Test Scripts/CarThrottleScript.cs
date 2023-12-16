using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarThrottleScript
{
    GameObject Car;
    float verticalInput, maxSpeed, reverseSpeed, addSpeed, reverseMaxSpeed;
    float enginePower, acc, brake, carMass;
    float speed, steps;
    bool isAcc, isReverse, isBraking, clutchWait;
    Vector3 oldPosition;

    Rigidbody rb;

    public CarThrottleScript(GameObject Car, Rigidbody rb, int enginePower, float maxSpeed, float reverseMaxSpeed, float acc, float brake) // Constructor
    {
        this.Car = Car;
        this.rb = rb;
        this.enginePower = enginePower;
        this.maxSpeed = maxSpeed;
        this.reverseMaxSpeed= reverseMaxSpeed;
        this.acc = acc;
        this.brake = brake;
        carMass = rb.mass;
        oldPosition = Car.transform.position;
    }

    public void UpdateSpeed()
    {
        steps += Time.deltaTime;
        Debug.Log(steps);


        verticalInput = Input.GetAxis("Vertical");
        
        rb.AddRelativeForce(Vector3.forward * 2000f * verticalInput); //CarMass = rb.mass*10f
        speed = Vector3.Distance(Car.transform.position, oldPosition) *100f; // Original = * 100f

        oldPosition = Car.transform.position;
        Debug.Log("Speed: " + speed.ToString("F2"));


    }

    public float GetSpeedValue()
    {
        return speed;
    }


    public void CarGoForwardInputs(float driftVal, float rbDriftBuff) //Player's forward moving control based on car's stats
    {
        if (verticalInput !> 0.01f) {
            return ; }

        isAcc = true;
        isReverse = false;
        bool isGrounded = true;

        if (speed < maxSpeed && isGrounded)
        {
            float addspeed = 1f;


            addspeed = 750f + (speed * enginePower); // 5f -> Speed = 360f // 1.5f = 100f
            float EnginePowerNerfer = 0.6f;
            float Spd = 10f;
            float AccBuffer = acc;


            if (Spd < speed && speed < maxSpeed)
            {
                do
                {
                    EnginePowerNerfer -= 0.025f; //Perfect value for current EnginePower levels
                    Spd += 10f;
                    //AddSpeed += 1f;
                    AccBuffer += 0.01f; //0.01f
                    //turning -= 0.05f;
                }
                while (Spd < speed);
            }

            float accLerpTime = speed * EnginePowerNerfer;

            float calculatedAcc = Mathf.Lerp(0.1f, AccBuffer, accLerpTime * Time.deltaTime);

            if (isBraking)
                calculatedAcc *= 0.9f;

            rb.AddRelativeForce(new Vector3(rbDriftBuff, -1f, driftVal + carMass * speed * Time.deltaTime)); //CarMass = rb.mass*10f
        }



    }

    protected void CarGoBackwardsBrakingInputs(float rbDriftBuff) //Player's reverse moving input
    {
        if (verticalInput < -0.2f) {
            isBraking = false;
            return ; }
            if (verticalInput < -0.2f)
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

            if (speed < reverseMaxSpeed && isReverse)
            {
                if (clutchWait)
                {
                    //StartCoroutine(ClutchWaitTime());
                    clutchWait = false;
                }
                else
                {
                    reverseSpeed = -(150f + (enginePower * 10f));
                    rb.AddRelativeForce(new Vector3(rbDriftBuff, -1f, carMass * reverseSpeed * Time.deltaTime));//CarMass = rb.mass*10f
                }
            }



        }
        else
        {
            isBraking = false;
        }
    }

    IEnumerator ClutchWaitTime() //Little "wait time gap" between backwards- and forwards driving
    {
        yield return new WaitForSeconds(0.4f);
        clutchWait = false;

    }
}
