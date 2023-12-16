using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelRotate : MonoBehaviour
{
    public GameObject frontWheels, backWheels;
    Vector3 oldPosition;
    float speed;


    void Awake()
    {
        oldPosition = transform.position;
    }


    void Update()
    {
        speed = Vector3.Distance(oldPosition, transform.position) * 20000f; // Original = * 100f

        frontWheels.transform.Rotate(Vector3.forward * Time.deltaTime * -speed, Space.Self);
        backWheels.transform.Rotate(Vector3.forward * Time.deltaTime * -speed, Space.Self);
        oldPosition = transform.position;
    }
}
