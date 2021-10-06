using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject Player; //Controlled character
    public GameObject CameraPosition; //Where camera "sits"
    public GameObject CameraLookAtPosition; //Where camera looks
    public GameObject BackCamera;
    public GameObject AICars, PlayerCamera;


    public float GroundTime;
    public float AirTime;
    private int CamSet;
    Vector3 CameraOffset, FixedCameraPosition;

    private int LookCommander = 0;
    private bool holdbutton = false;
    private void Awake()
    {
        if(PlayerCamera == null)
            PlayerCamera = GameObject.FindWithTag("PlayerCamera");

        CameraOffset = CameraPosition.transform.position;
        gameObject.transform.position = CameraOffset;
        CamSet = 1;

        CameraFollow(-1);//player

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
    void Update()
    {
        if(Input.GetKey("enter"))
        {
            holdbutton = true;
        }
        else
        {
            holdbutton = false;
        }

        if (holdbutton == true)
        {
            if (Input.GetKey(KeyCode.Keypad9))//player
            {
                CameraFollow(-1);
            }
            else if (Input.GetKey(KeyCode.Keypad0))
            {
                CameraFollow(0);
            }
            else if (Input.GetKey(KeyCode.Keypad1))
            {
                CameraFollow(1);
            }
            else if (Input.GetKey(KeyCode.Keypad2))
            {
                CameraFollow(2);
            }
            else if (Input.GetKey(KeyCode.Keypad3))
            {
                CameraFollow(3);
            }
            else if (Input.GetKey(KeyCode.Keypad4))
            {
                CameraFollow(4);
            }
            else if (Input.GetKey(KeyCode.Keypad5))
            {
                CameraFollow(5);
            }
            else if (Input.GetKey(KeyCode.Keypad6))
            {
                CameraFollow(6);
            }
            else if (Input.GetKey(KeyCode.Keypad7))
            {
                CameraFollow(7);
            }
            else if (Input.GetKey(KeyCode.Keypad8))
            {
                CameraFollow(8);
            }
        }

    }
    private void CameraFollow(int number)
    {
        GameObject cameraposition;
        GameObject cameralookatposition;
        GameObject backcamera;
        if (number == -1) // PLAYER
        {
            cameraposition = PlayerCamera.transform.GetChild(0).gameObject;
            CameraPosition = cameraposition;
            cameralookatposition = PlayerCamera.transform.GetChild(1).gameObject;
            CameraLookAtPosition = cameralookatposition;
            backcamera = PlayerCamera.transform.GetChild(2).gameObject;
            BackCamera = backcamera;
        }
        else // AI
        {
            GameObject AInumb = AICars.transform.GetChild(number).gameObject;
            cameraposition = AInumb.transform.GetChild(0).gameObject;
            CameraPosition = cameraposition;
            cameralookatposition = AInumb.transform.GetChild(1).gameObject;
            CameraLookAtPosition = cameralookatposition;
            backcamera = AInumb.transform.GetChild(2).gameObject;
            BackCamera = backcamera;

        }
        CameraOffset = CameraPosition.transform.position;
        gameObject.transform.position = CameraOffset;
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
            gameObject.transform.position = FixedCameraPosition;
            LookCommander = 0;
        }
        else if (LookCommander == 0) //NORMAL
        {
            gameObject.transform.position = Vector3.Lerp(transform.position, FixedCameraPosition, lerptime * Time.deltaTime);
        }

            gameObject.transform.LookAt(camlookpos);

    }


}
