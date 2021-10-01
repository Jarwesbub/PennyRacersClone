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
    public List<GameObject> TargetsCache;
    public int nextTarget;
    public float TargetDistance;
    public bool CarIsGrounded;
    private bool targetTriggerBug = false;
    void Awake()
    {
        nextTarget = 0;
        TargetController = GameObject.FindWithTag("TargetController");
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
        if (targetPosList[nextTarget] != null)
        {
            Vector3 targetpos = targetPosList[nextTarget];
            TargetDistance = Vector3.Distance(transform.position, targetpos);
        }


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
            PlayerController.GetComponent<CarController>().GetFrictionValues(1);
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
        if (!targetTriggerBug)
        {
            targetTriggerBug = true;
            if (other.gameObject.layer != 7)// not GOAL OBJECT
            {

                if (other.tag == "slowtarget" || other.tag == "normaltarget" || other.tag == "fasttarget")
                {
                    //if (!TargetsCache.Contains(other.gameObject))
                    {
                        //TargetsCache.Add(other.gameObject);
                        nextTarget++;
                        nextTarget = TargetController.GetComponent<TargetControl>().PlayerTargetList(other.gameObject, false);
                    }

                        //nextTarget++;
                    
                }
            }
            else if (other.gameObject.layer == 7)// GOAL OBJECT
            {
                TargetController.GetComponent<TargetControl>().PlayerTargetList(other.gameObject, true);
                //TargetsCache.Clear();
                nextTarget = 0;
            }
        }
        else
            targetTriggerBug = false;
    }


}
