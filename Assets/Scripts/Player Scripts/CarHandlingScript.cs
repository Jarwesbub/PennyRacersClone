using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarHandlingScript
{
    float turning = 1.0f, brakeTurn = 1.8f, driftVal, driftValToAxis;
    float turnForce, turnTime;
    float horizontalInput, speed;

    Rigidbody rb;

    public CarHandlingScript(Rigidbody rb) // Constructor
    {
        this.rb = rb;
    }

    public void UpdateHorizontalInput(float speed)
    {
        horizontalInput = Input.GetAxis("Horizontal");
        this.speed = speed;

        SetTurnForce();

        float handlingMultiplier = speed * turnForce;

        Vector3 m_EulerAngleVelocity = new Vector3(0, horizontalInput * handlingMultiplier, 0);
        Quaternion deltaRotation = Quaternion.Euler(m_EulerAngleVelocity * Time.fixedDeltaTime);
        rb.MoveRotation(rb.rotation * deltaRotation);
    }

    private void SetTurnForce()
    {
        if (horizontalInput > 0.1 || horizontalInput < -0.1)
        {
            if (turnTime == 0f)
                turnTime = 2f;
            else if (turnTime < 1f)
                turnTime -= Time.deltaTime;
        }
        else
        {
            turnTime = 0f;
            turnForce = 0f;
        }

        float multiply = speed / 10f;
        if(multiply > 1f)
        {
            multiply = 1f;
        }

        turnForce = turnTime * multiply;
    }

    public void CarAllTurningInputs(float speed, bool isGrounded, bool isBraking, bool isHitting, float multiplier) //Player's turning values combined with drift value
    {
        if (speed < 0.1f || !isGrounded || isHitting) { return; }

        float rotValue = turning;

        if (isBraking) //BACKWADS TURNING
        {
            rotValue = (turning * brakeTurn);
        }


        if (horizontalInput > 0.01f || horizontalInput < -0.01f) // Turning left or right
        {
            rotValue *= multiplier;

        }

        Vector3 m_EulerAngleVelocity = new Vector3(0, horizontalInput * 3f, 0);
        Quaternion deltaRotation = Quaternion.Euler(m_EulerAngleVelocity * Time.fixedDeltaTime);
        rb.MoveRotation(rb.rotation * deltaRotation);

    }
}
