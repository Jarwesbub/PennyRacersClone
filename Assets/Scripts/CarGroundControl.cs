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
    private int targetTriggerBug = 2;

    void Awake()
    {
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

            /*
            if (Autopilot && CarIsGrounded) //Turning control is here; Gas control is at CarController
            {
                Vector3 relativePos = targetpos - transform.position;
                Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
                float turnspeed = (TargetDistance/10f) +1f;
                float turnlerp = Mathf.Lerp(turnspeed, 0.1f, 10f * Time.deltaTime);
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * turnlerp);

            }*/
        
        }
    }


    void OnCollisionEnter(Collision other)
    {



    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ground")
        {
            CarIsGrounded = true;
            MainCamera.GetComponent<CameraController>().ChangeCameraSettings(1); //Car is grounded -> script
        }
    }
    void OnTriggerStay(Collider other)
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
