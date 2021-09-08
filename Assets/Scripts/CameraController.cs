using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject Player; //Controlled character
    public GameObject CameraPosition; //Where camera "sits"
    public GameObject CameraLookAtPosition; //Where camera looks
    public float GroundTime;
    public float AirTime;
    private int CamSet;

    Vector3 CameraOffset;

    private void Awake()
    {
        CameraOffset = CameraPosition.transform.position;
        gameObject.transform.position = CameraOffset;
        CamSet = 1;
    }

    private void FixedUpdate()
    {
        Follow(CamSet);

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
            Debug.Log("Slow Camera");
        }


        gameObject.transform.position = Vector3.Lerp(transform.position, CameraPosition.transform.position, lerptime * Time.deltaTime);
        gameObject.transform.LookAt(camlookpos);

    }


}
