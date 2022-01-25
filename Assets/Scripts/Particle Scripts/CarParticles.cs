using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarParticles : MonoBehaviour
{
    private GameObject Player,PlayerController;
    public GameObject L_wheel, R_wheel;
    private ParticleSystem left_drift, right_drift;
    public int RoadType;
    public bool IsDrifting, IsGrounded;
    public bool UIjoystickActive = false;
    public Color grey, brown;
    private int currentColor;
    public float TEST;

    // Start is called before the first frame update
    void Awake()
    {
        IsDrifting = false;
        IsGrounded = false;
        PlayerController = GameObject.FindWithTag("PlayerController");
        Player = GameObject.FindWithTag("Player");
        left_drift = L_wheel.GetComponent<ParticleSystem>();
        right_drift = R_wheel.GetComponent<ParticleSystem>();

        UIjoystickActive = PlayerController.GetComponent<CarController>().useUIjoystick;
    }

    // Update is called once per frame
    void Update()
    {
        IsDrifting = PlayerController.GetComponent<CarController>().IsDrifting;
        //IsGrounded = PlayerController.GetComponent<CarController>().IsGrounded;
        RoadType = Player.GetComponent<CarGroundControl>().RoadType;

        InputChecks();


    }
    private void InputChecks()
    {
        float scale = 0.8f; //How easily effects are played when turning
        scale = 0f; //TESTING

        if (IsDrifting && RoadType == 1)//Asphalt
        {
            float playerinput = PlayerController.GetComponent<CarController>().verticalInput;
            bool IsBraking = PlayerController.GetComponent<CarController>().IsBraking;
            if (playerinput >= scale || playerinput <= -scale || IsBraking)
            {
                AsphaltDriftPlay(true);
            }
        }
        else if (IsDrifting && RoadType == 2)//Grass
        {
            float playerinput = PlayerController.GetComponent<CarController>().verticalInput;
            bool IsBraking = PlayerController.GetComponent<CarController>().IsBraking;
            if (playerinput >= scale || playerinput <= -scale || IsBraking)
            {
                DirtDriftPlay(true);
            }
        }
        else if (RoadType == 0)
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
                L_wheel.GetComponent<Renderer>().material.color = grey;
                R_wheel.GetComponent<Renderer>().material.color = grey;
                currentColor = 1;
            }

            left_drift.Play();
            right_drift.Play();
        }
        else
        {
            left_drift.Stop();
            right_drift.Stop();
        }

    }
    private void DirtDriftPlay(bool play)
    {
        if (play)
        {
            if (currentColor != 2)
            {
                L_wheel.GetComponent<Renderer>().material.color = brown;
                R_wheel.GetComponent<Renderer>().material.color = brown;
                currentColor = 2;
            }

            left_drift.Play();
            right_drift.Play();
        }
        else
        {
            left_drift.Stop();
            right_drift.Stop();
        }

    }

}
