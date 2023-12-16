using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject Car;
    Rigidbody rb;
    [SerializeField] int enginePower = 6;
    [SerializeField] float speed, maxSpeed = 1000f, reverseSpeed, reverseMaxSpeed = 200f;
    [SerializeField] float acc, brake, carMass;

    CarThrottleScript ThrottleControl;
    CarHandlingScript HandlingControl;


    private void Awake()
    {
        rb = Car.GetComponent<Rigidbody>();
        ThrottleControl = new CarThrottleScript(Car, rb, enginePower, maxSpeed, reverseMaxSpeed, acc, brake);
        HandlingControl = new CarHandlingScript(rb);
    }

    private void Update()
    {
        speed = ThrottleControl.GetSpeedValue();
    }

    private void FixedUpdate()
    {
        HandlingControl.UpdateHorizontalInput(speed);
        ThrottleControl.UpdateSpeed();
        
    }

}
