using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject Player; //Controlled character
    public GameObject CameraPosition; //Where camera "sits"
    public GameObject CameraLookAtPosition; //Where camera looks
    public GameObject BackCamera;
    public float GroundTime;
    public float AirTime;
    private int CamSet;

    Vector3 CameraOffset, FixedCameraPosition;

    private int LookCommander = 0;

    private void Awake()
    {
        CameraOffset = CameraPosition.transform.position;
        gameObject.transform.position = CameraOffset;
        CamSet = 1;


    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            FixedCameraPosition = BackCamera.transform.position;
            LookCommander = 2;
            Follow(CamSet);
        }
        else if (LookCommander == 2)
        {
            FixedCameraPosition = CameraPosition.transform.position;
            LookCommander = 1;
            Follow(CamSet);
        }
        else if (LookCommander == 0)//reset
        {
            FixedCameraPosition = CameraPosition.transform.position;
            Follow(CamSet);

        }
    }


    public void ChangeCameraSettings(int value)
    {
        if (value == 1)
        {
            CamSet = value;
        }

        else if (value == 2)
        {
            CamSet = value;
        }
    }

    void Follow(int value)
    {
        //value 1 = Follow car normally
        //value 2 = Follow slower -> car is not grounded (flying)
        float lerptime = GroundTime;
        Vector3 camlookpos = CameraLookAtPosition.transform.position;

        if (value == 1)
        {
            lerptime = GroundTime;
            
        }

        else if (value == 2)
        {
            lerptime = AirTime;

        }

        if (LookCommander == 2)//WHEN CAMERA LOOKS BACK
        {
            gameObject.transform.position = FixedCameraPosition;
        }
        else if (LookCommander == 1)//WHEN CAMERA RESETS
        {
            gameObject.transform.position = FixedCameraPosition; ;
            LookCommander = 0;
        }
        else if (LookCommander == 0) //NORMAL
        {
            gameObject.transform.position = Vector3.Lerp(transform.position, FixedCameraPosition, lerptime * Time.deltaTime);
        }

            gameObject.transform.LookAt(camlookpos);

    }


}
