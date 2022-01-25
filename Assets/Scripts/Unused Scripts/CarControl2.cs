using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarControl2 : MonoBehaviour
{
    public float Timer, TimerWait, Speed;

  //Acceleration example, Level 2

  //this is our target velocity while decelerating
  float initialVelocity = 0.0f;

  //this is our target velocity while accelerating
  float finalVelocity = 500.0f;

  //this is our current velocity
  float currentVelocity = 0.0f;
  
  //this is the velocity we add each second while accelerating
  float accelerationRate = 10.0f;
  
  //this is the velocity we subtract each second while decelerating
  float decelerationRate = 50.0f;
  
    void Start()
    {
        TimerWait = 1f;
    }


  //movement happens here
  void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            //add to the current velocity according while accelerating
            currentVelocity = currentVelocity + (accelerationRate * Time.deltaTime);
            transform.Translate(0, 0, currentVelocity);
        }
        else
        {
            //subtract from the current velocity while decelerating
            currentVelocity = currentVelocity - (decelerationRate * Time.deltaTime);
            if (currentVelocity > 0)
            {
                transform.Translate(0, 0, currentVelocity);
            }
            else
            {
                transform.Translate(0, 0, 0);
            }
        }

        //ensure the velocity never goes out of the initial/final boundaries
        currentVelocity = Mathf.Clamp(currentVelocity, initialVelocity, finalVelocity);

        //propel the object forward

        Timer += Time.deltaTime;

        if (Timer > TimerWait)
        {
            Speed = gameObject.transform.position.y * Timer;

        }
        else
        {


        }



    }
}
