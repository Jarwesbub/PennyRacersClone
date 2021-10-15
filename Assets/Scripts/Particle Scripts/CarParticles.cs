using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarParticles : MonoBehaviour
{
    private GameObject PlayerController;
    public ParticleSystem left_drift, right_drift;
    public bool IsDrifting, IsGrounded;

    // Start is called before the first frame update
    void Awake()
    {
        IsDrifting = false;
        IsGrounded = false;
        PlayerController = GameObject.FindWithTag("PlayerController");
    }

    // Update is called once per frame
    void Update()
    {
        IsDrifting = PlayerController.GetComponent<CarController>().IsDrifting;
        IsGrounded = PlayerController.GetComponent<CarController>().IsGrounded;

        if (IsDrifting && IsGrounded)
        {
            float playerinput = PlayerController.GetComponent<CarController>().verticalInput;
            bool IsBraking = PlayerController.GetComponent<CarController>().IsBraking;
            float scale = 0.8f;
            if (playerinput >= scale || playerinput <= -scale || IsBraking)
            {
                left_drift.Play();
                right_drift.Play();
            }
        }
        else if (!IsGrounded)
        {
            left_drift.Stop();
            right_drift.Stop();
        }

    }
}
