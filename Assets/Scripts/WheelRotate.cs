using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelRotate : MonoBehaviour
{
    public GameObject frontWheels, backWheels;
    float Speed;
    Vector3 oldPosition;
    // Start is called before the first frame update
    void Awake()
    {
        oldPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Speed = Vector3.Distance(oldPosition, transform.position) * 20000f; // Original = * 100f

        frontWheels.transform.Rotate(Vector3.forward * Time.deltaTime * -Speed, Space.Self);
        backWheels.transform.Rotate(Vector3.forward * Time.deltaTime * -Speed, Space.Self);
        oldPosition = transform.position;
    }
}
