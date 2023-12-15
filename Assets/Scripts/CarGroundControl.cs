using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarGroundControl : MonoBehaviour
{
    private GameObject PlayerController, PlayerGround;
    private GameObject MainCamera;
    private GameObject TargetController;
    public List<Vector3> targetPosList;
    private int nextTarget, targetCount;
    public bool CarIsGrounded, Autopilot, IsRespawning = false;
    public int RoadType;

    //Possible BUGS in future:
    //private int targetTriggerBug = 2;//Avoids multiple trigger actions when collision with targets
    [SerializeField]
    private int groundTriggerCount; //Works when ROAD and TERRAIN layered object are close to each other
    //
    void Awake()
    {
        CarIsGrounded = false;
        RoadType = 0;
        Autopilot = false;
        nextTarget = 0;
        PlayerController = GameObject.FindWithTag("PlayerController");
        PlayerGround = GameObject.FindWithTag("PlayerGround");
        TargetController = GameObject.FindWithTag("TargetController");
        MainCamera = GameObject.FindWithTag("MainCamera");
    }
    public void LoadAllTargets(Vector3 targetpos)//From TargetControl script
    {
        targetPosList.Add(targetpos);
        targetCount = targetPosList.Count - 1;

    }
    /*
    void Update()
    {
        
        if (nextTarget < targetCount)
        {
            Vector3 targetpos = targetPosList[nextTarget];
            TargetDistance = Vector3.Distance(transform.position, targetpos);
        
        }
    }*/
    public void PlayerRespawn(bool fromOtherScript) //Accessed from CarController script when TRUE
    {
        PlayerGround.GetComponent<CarTargetTrigger>().playerIsRespawing = true;
        if (!fromOtherScript)
        {
            float sec = 2f;
            StartCoroutine(RespawnCooldown(sec));
        }
        else
        {
            RespawnPosition();
        }
        
    }
    private void RespawnPosition()
    {
        nextTarget = PlayerGround.GetComponent<CarTargetTrigger>().nextTarget;
        int spawnPos = nextTarget;
        int lookAt = nextTarget;

        if (lookAt < targetCount)
        {
            lookAt++;

        }
        transform.rotation = Quaternion.Euler(0f, transform.rotation.y, 0f);
        transform.position = targetPosList[spawnPos];
        transform.LookAt(targetPosList[lookAt]);
    }

    IEnumerator RespawnCooldown(float sec)
    {
        PlayerController.GetComponent<CarController>().resetPlayer = true;

        yield return new WaitForSeconds(sec);

        nextTarget = PlayerGround.GetComponent<CarTargetTrigger>().nextTarget;
        int spawnPos = nextTarget;
        int lookAt = nextTarget;

        RespawnPosition();

        PlayerController.GetComponent<CarController>().resetPlayer = false;
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
        else if (other.tag == "ForceRespawn")
        {
            PlayerRespawn(false);
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
        /*
        if (targetTriggerBug==2)
        {
            targetTriggerBug = 0;
                if (other.gameObject.layer != 7)// not GOAL OBJECT
                {

                    if (other.tag == "slowtarget" || other.tag == "normaltarget" || other.tag == "fasttarget")
                    {

                        nextTarget = TargetController.GetComponent<TargetControl>().PlayerTargetList(other.gameObject, false);
                    

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
            targetTriggerBug++;
        */
    }


}
