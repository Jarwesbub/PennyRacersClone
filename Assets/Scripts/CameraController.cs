using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraController : MonoBehaviour
{
    public GameObject Player; //Controlled character
    public GameObject CameraPosition; //Where camera "sits"
    public GameObject CameraLookAtPosition; //Where camera looks
    public GameObject BackCamera;
    public GameObject BotCars;

    // Visible for testing
    [SerializeField] private float GroundTime, AirTime;
    private int ObjectID;
    [SerializeField]//DEBUG
    private bool holdbutton = false;
    public bool useUIjoystick = false;
    [SerializeField]bool isLookingBack = false;
    private int backCameraButton = -1; // -1=not in use // This variable is used when pressing UI camera button (on mobile)

    private void Awake()
    {
        gameObject.transform.position = CameraPosition.transform.position;
        ObjectID = 1;

        GroundTime = 20;
        AirTime = 10;

        SetCameraToFollowByID(-1); // player

    }
    public void ButtonLookBack()
    {
        useUIjoystick = true;

        if (backCameraButton<=0)
            backCameraButton = 1;
        else
            backCameraButton = 0;


        Follow(ObjectID);

    }

    private void FixedUpdate()
    {
        Follow(ObjectID);

    }

    
    void Update() 
    {
        if (Input.GetKey(KeyCode.LeftShift) || backCameraButton == 1) { isLookingBack = true; }
        else { isLookingBack = false; }

        if (Input.GetKey("enter"))
        {
            holdbutton = true;
        }
        else
        {
            holdbutton = false;
        }

        if (holdbutton == true)
        {
            if (Input.GetKey(KeyCode.Alpha9))//player
            {
                SetCameraToFollowByID(-1);
            }
            else if (Input.GetKey(KeyCode.Alpha0))
            {
                SetCameraToFollowByID(0);
            }
            else if (Input.GetKey(KeyCode.Alpha1))
            {
                SetCameraToFollowByID(1);
            }
            else if (Input.GetKey(KeyCode.Alpha2))
            {
                SetCameraToFollowByID(2);
            }
            else if (Input.GetKey(KeyCode.Alpha3))
            {
                SetCameraToFollowByID(3);
            }
            else if (Input.GetKey(KeyCode.Alpha4))
            {
                SetCameraToFollowByID(4);
            }
            else if (Input.GetKey(KeyCode.Alpha5))
            {
                SetCameraToFollowByID(5);
            }
            else if (Input.GetKey(KeyCode.Alpha6))
            {
                SetCameraToFollowByID(6);
            }
            else if (Input.GetKey(KeyCode.Keypad7))
            {
                SetCameraToFollowByID(7);
            }
            else if (Input.GetKey(KeyCode.Keypad8))
            {
                SetCameraToFollowByID(8);
            }
        }

    }
    

    private void SetCameraToFollowByID(int id)
    {
        if (id == -1) // PLAYER
        {
            CameraPosition = Player.transform.GetChild(0).gameObject;
            CameraLookAtPosition = Player.transform.GetChild(1).gameObject;
            BackCamera = Player.transform.GetChild(2).gameObject;
        }
        else // AI
        {
            GameObject AInumb = BotCars.transform.GetChild(id).gameObject;
            CameraPosition = AInumb.transform.GetChild(0).gameObject;
            CameraLookAtPosition = AInumb.transform.GetChild(1).gameObject;
            BackCamera = AInumb.transform.GetChild(2).gameObject;

        }

        gameObject.transform.position = CameraPosition.transform.position;
    }

    public void ChangeCameraSettings(int value)
    {
        ObjectID = value;

    }

    void Follow(int value)
    {
        //value 1 = Follow car normally
        //value 2 = Follow slower -> car is not grounded (flying)
        float lerptime = GroundTime;

        if (value == 1)
        {
            lerptime = GroundTime;
            
        }

        else if (value == 2)
        {
            lerptime = AirTime;

        }

        if (isLookingBack)
        {
            gameObject.transform.position = BackCamera.transform.position;
        }
        else
        {
            gameObject.transform.position = Vector3.Lerp(transform.position, CameraPosition.transform.position, lerptime * Time.deltaTime);
        }

        gameObject.transform.LookAt(CameraLookAtPosition.transform.position);

    }


}
