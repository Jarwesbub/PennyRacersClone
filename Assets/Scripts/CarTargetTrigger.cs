using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarTargetTrigger : MonoBehaviour
{
    private GameObject Player, TargetController;
    public List<Vector3> targetPosList;

    public int nextTarget, targetCount;
    public float TargetDistance;
    public bool playerIsRespawing = false;

    void Awake()
    {
        Player = GameObject.FindWithTag("Player");
        TargetController = GameObject.FindWithTag("TargetController");
        nextTarget = 0;
        playerIsRespawing = false;
    }
    public void LoadAllTargets(Vector3 targetpos)//From TargetControl script
    {
        targetPosList.Add(targetpos);
        targetCount = targetPosList.Count - 1;

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
        if (!playerIsRespawing)
        {

            if (other.gameObject.layer != 7)// not GOAL OBJECT
            {

                if (other.tag == "slowtarget" || other.tag == "normaltarget" || other.tag == "fasttarget")
                {

                    nextTarget = TargetController.GetComponent<TargetControl>().PlayerTargetList(other.gameObject, false);
                    //Player.GetComponent<CarGroundControl>().nextTarget = nextTarget;

                }
            }
            else if (other.gameObject.layer == 7)// GOAL OBJECT
            {

                TargetController.GetComponent<TargetControl>().PlayerTargetList(other.gameObject, true);

                //TargetsCache.Clear();
                nextTarget = 0;
                //Player.GetComponent<CarGroundControl>().nextTarget = nextTarget;
            }

        }
        else
            playerIsRespawing = false;
    }
}
