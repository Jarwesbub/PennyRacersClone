using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarGroundControl : MonoBehaviour
{
    public GameObject Ground;
    public GameObject PlayerController;
    public GameObject MainCamera;
    public bool CarIsGrounded;
    //private int FrictionValue = -1;

    // Start is called before the first frame update
    void Start()
    {
        CarIsGrounded = false; //test
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "ground")
        {
            PlayerController.GetComponent<CarController>().GetFrictionValues(-1);
        }
        else if (other.gameObject.tag == "ai")
        {
            PlayerController.GetComponent<CarController>().IsBraking = true;
        }

    }

    void OnTriggerEnter(Collider other)
    {

        if (other.tag == "ground")
        {
            CarIsGrounded = true;
            MainCamera.GetComponent<CameraController>().ChangeCameraSettings(1); //Car is grounded -> script
            PlayerController.GetComponent<CarController>().GetFrictionValues(3);
        }
        else if (other.tag == "ai")
        {
            PlayerController.GetComponent<CarController>().GetFrictionValues(2);//Enemy
        }

    }

    void OnTriggerExit(Collider other)
    {

        if (other.tag == "ground")
        {
            CarIsGrounded = false;

            MainCamera.GetComponent<CameraController>().ChangeCameraSettings(2); //Car is not grounded -> script
        }
        else if (other.tag == "grass")
        {
            PlayerController.GetComponent<CarController>().GetFrictionValues(1);//grass
        }

    }
}
