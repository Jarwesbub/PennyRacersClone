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
    public bool carIsGrounded, autopilot, isRespawning = false;
    public int roadType;

    [SerializeField]
    private int groundTriggerCount; //Works when ROAD and TERRAIN layered object are close to each other

    void Awake()
    {
        carIsGrounded = false;
        roadType = 0;
        autopilot = false;
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
        PlayerController.GetComponent<CarController>().SetResetPlayer(true);

        yield return new WaitForSeconds(sec);

        nextTarget = PlayerGround.GetComponent<CarTargetTrigger>().nextTarget;
        int spawnPos = nextTarget;
        int lookAt = nextTarget;

        RespawnPosition();

        PlayerController.GetComponent<CarController>().SetResetPlayer(false);
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ground")
        {
            roadType = 0;
            carIsGrounded = true;
            MainCamera.GetComponent<CameraController>().ChangeCameraSettings(1); //Car is grounded -> script
        }
        else if (other.gameObject.layer == 8 || other.gameObject.layer == 9) //ROAD or TERRAIN
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
        if (other.tag == "Asphalt" || other.tag == "Grass" || other.tag == "ground")
        {
            carIsGrounded = true;
        }
        if (roadType == 0)
        {
            CheckGroundType(other);
        }
    }

    void CheckGroundType(Collider other)
    {

        if (other.tag == "Asphalt")
        {
            roadType = 1;
            carIsGrounded = true;
            MainCamera.GetComponent<CameraController>().ChangeCameraSettings(1); //Car is grounded -> script
        }
        if (other.tag == "Grass")
        {
            roadType = 2;
            carIsGrounded = true;
            MainCamera.GetComponent<CameraController>().ChangeCameraSettings(1); //Car is grounded -> script
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "ground")
        {
            roadType = 0;
            groundTriggerCount--;
            if (groundTriggerCount <= 0)
                carIsGrounded = false;
            MainCamera.GetComponent<CameraController>().ChangeCameraSettings(2); //Car is not grounded -> script
        }
        else if (other.tag == "Asphalt")
        {
            roadType = 0;
            groundTriggerCount--;
            if (groundTriggerCount <= 0)
                carIsGrounded = false;
            MainCamera.GetComponent<CameraController>().ChangeCameraSettings(2); //Car is not grounded -> script
        }
        else if (other.tag == "Grass")
        {
            roadType = 0;
            groundTriggerCount--;
            if (groundTriggerCount <= 0)
                carIsGrounded = false;
            MainCamera.GetComponent<CameraController>().ChangeCameraSettings(2); //Car is not grounded -> script
        }

    }

}
