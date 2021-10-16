using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarGroundControl : MonoBehaviour
{
    public GameObject Ground;
    public GameObject PlayerController;
    public GameObject MainCamera;
    private GameObject TargetController;
    public List<Vector3> targetPosList;
    public int nextTarget, targetCount;
    public float TargetDistance;
    public bool CarIsGrounded, Autopilot;
    public int RoadType;

    //Possible BUGS in future:
    private int targetTriggerBug = 2;//Avoids multiple trigger actions when collision with targets
    [SerializeField]
    private int groundTriggerCount; //Works when ROAD and TERRAIN layered object are close to each other
    //
    void Awake()
    {
        RoadType = 0;
        Autopilot = false;
        nextTarget = 0;
        TargetController = GameObject.FindWithTag("TargetController");
        MainCamera = GameObject.FindWithTag("MainCamera");
        targetCount = targetPosList.Count;
    }
    public void LoadAllTargets(Vector3 targetpos)//From TargetControl script
    {
        targetPosList.Add(targetpos);


    }
    void Start()
    {
        CarIsGrounded = false; //test
    }
    void Update()
    {
        
        if (nextTarget < targetCount)
        {
            Vector3 targetpos = targetPosList[nextTarget];
            TargetDistance = Vector3.Distance(transform.position, targetpos);
        
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ground")
        {
            RoadType = 0;
            CarIsGrounded = true;
            MainCamera.GetComponent<CameraController>().ChangeCameraSettings(1); //Car is grounded -> script
        }
        else if(other.gameObject.layer == 8 || other.gameObject.layer == 9) //ROAD or TERRAIN
        {
            groundTriggerCount++;
            CheckGroundType(other);
        }
    }
    void OnTriggerStay(Collider other)
    {
        if(other.tag == "Asphalt" || other.tag == "Grass" || other.tag == "ground")
        {
            CarIsGrounded = true;
        }
        if(RoadType == 0)
        {
            CheckGroundType(other);
        }
    }
    void CheckGroundType(Collider other)
    {
        
        {
            
            if (other.tag == "Asphalt")
            {
                RoadType = 1;
                CarIsGrounded = true;
                MainCamera.GetComponent<CameraController>().ChangeCameraSettings(1); //Car is grounded -> script
            }
            if (other.tag == "Grass")
            {
                RoadType = 2;
                CarIsGrounded = true;
                MainCamera.GetComponent<CameraController>().ChangeCameraSettings(1); //Car is grounded -> script
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        
        if (other.tag == "ground")
        {
            RoadType = 0;
            groundTriggerCount--;
            if (groundTriggerCount <= 0)
                CarIsGrounded = false;
           MainCamera.GetComponent<CameraController>().ChangeCameraSettings(2); //Car is not grounded -> script
        }
        else if (other.tag == "Asphalt")
        {
            RoadType = 0;
            groundTriggerCount--;
            if (groundTriggerCount <= 0)
                CarIsGrounded = false;
            MainCamera.GetComponent<CameraController>().ChangeCameraSettings(2); //Car is not grounded -> script
        }
        else if (other.tag == "Grass")
        {
            RoadType = 0;
            groundTriggerCount--;
            if (groundTriggerCount <= 0)
                CarIsGrounded = false;
            MainCamera.GetComponent<CameraController>().ChangeCameraSettings(2); //Car is not grounded -> script
        }

        if (targetTriggerBug==2)
        {
            targetTriggerBug = 0;
                if (other.gameObject.layer != 7)// not GOAL OBJECT
                {

                    if (other.tag == "slowtarget" || other.tag == "normaltarget" || other.tag == "fasttarget")
                    {

                        nextTarget = TargetController.GetComponent<TargetControl>().PlayerTargetList(other.gameObject, false);
                    

                    }
                    /*
                    else if (other.tag == "autopilotroute" && Autopilot)
                    {
                        //nextTarget++;
                        nextTarget = TargetController.GetComponent<TargetControl>().PlayerTargetList(other.gameObject, false);
                    }*/
                }
                else if (other.gameObject.layer == 7)// GOAL OBJECT
                {
                    
                    TargetController.GetComponent<TargetControl>().PlayerTargetList(other.gameObject, true);
                    //TargetsCache.Clear();
                    nextTarget = 0;
                }
            
        }
        else
            targetTriggerBug++;
    }


}
