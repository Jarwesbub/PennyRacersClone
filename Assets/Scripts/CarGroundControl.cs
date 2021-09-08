using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarGroundControl : MonoBehaviour
{
    public GameObject Ground;
    public GameObject PlayerController;
    public GameObject MainCamera;
    public bool CarIsGrounded;
    private int FrictionValue = -1;

    // Start is called before the first frame update
    void Start()
    {
        CarIsGrounded = false; //test
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "ground")
        {
            PlayerController.GetComponent<CarController>().GetFrictionValues(FrictionValue);
        }


    }

    void OnTriggerEnter(Collider other)
    {

        if (other.tag == "ground")
        {
            CarIsGrounded = true;
            MainCamera.GetComponent<CameraController>().ChangeCameraSettings(1); //Car is grounded -> script
        }

    }

    void OnTriggerExit(Collider other)
    {

        if (other.tag == "ground")
        {
            CarIsGrounded = false;

            MainCamera.GetComponent<CameraController>().ChangeCameraSettings(2); //Car is not grounded -> script
        }

    }
}
