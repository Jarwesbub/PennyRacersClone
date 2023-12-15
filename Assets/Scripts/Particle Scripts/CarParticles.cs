using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarParticles : MonoBehaviour
{
    private GameObject Player,PlayerController;
    public GameObject LeftWheel, RightWheel;
    private ParticleSystem psLeftDrift, psRightDrift;
    public int roadType;
    public bool isDrifting, isGrounded;
    public bool uiJoystickActive = false;
    public Color grey, brown;
    private int currentColor;
    public float test;


    void Awake()
    {
        isDrifting = false;
        isGrounded = false;
        PlayerController = GameObject.FindWithTag("PlayerController");
        Player = GameObject.FindWithTag("Player");
        psLeftDrift = LeftWheel.GetComponent<ParticleSystem>();
        psRightDrift = RightWheel.GetComponent<ParticleSystem>();

        uiJoystickActive = PlayerController.GetComponent<CarController>().useUIjoystick;
    }


    void Update()
    {
        isDrifting = PlayerController.GetComponent<CarController>().isDrifting;
        roadType = Player.GetComponent<CarGroundControl>().RoadType;

        InputChecks();


    }
    private void InputChecks()
    {
        float scale = 0.8f; //How easily effects are played when turning
        scale = 0f; //TESTING

        if (isDrifting && roadType == 1)//Asphalt
        {
            float playerinput = PlayerController.GetComponent<CarController>().verticalInput;
            bool IsBraking = PlayerController.GetComponent<CarController>().isBraking;
            if (playerinput >= scale || playerinput <= -scale || IsBraking)
            {
                AsphaltDriftPlay(true);
            }
        }
        else if (isDrifting && roadType == 2)//Grass
        {
            float playerinput = PlayerController.GetComponent<CarController>().verticalInput;
            bool IsBraking = PlayerController.GetComponent<CarController>().isBraking;
            if (playerinput >= scale || playerinput <= -scale || IsBraking)
            {
                DirtDriftPlay(true);
            }
        }
        else if (roadType == 0)
        {
            AsphaltDriftPlay(false);
            DirtDriftPlay(false);
        }


    }

    private void AsphaltDriftPlay(bool play)
    {
        if(play)
        {
            if(currentColor != 1)
            {
                LeftWheel.GetComponent<Renderer>().material.color = grey;
                RightWheel.GetComponent<Renderer>().material.color = grey;
                currentColor = 1;
            }

            psLeftDrift.Play();
            psRightDrift.Play();
        }
        else
        {
            psLeftDrift.Stop();
            psRightDrift.Stop();
        }

    }
    private void DirtDriftPlay(bool play)
    {
        if (play)
        {
            if (currentColor != 2)
            {
                LeftWheel.GetComponent<Renderer>().material.color = brown;
                RightWheel.GetComponent<Renderer>().material.color = brown;
                currentColor = 2;
            }

            psLeftDrift.Play();
            psRightDrift.Play();
        }
        else
        {
            psLeftDrift.Stop();
            psRightDrift.Stop();
        }

    }

}
