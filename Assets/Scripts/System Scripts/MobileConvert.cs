using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileConvert : MonoBehaviour
{
    public DataManager dataManager;
    public GameObject MobileUIObj, MobileUIpedals;
    GameObject PlayerController;

    public bool isMobilePlatform;
    [SerializeField]
    bool useUIJoystick, uiButtonPedalsActive;


    void Awake()
    {
        dataManager.Load();
        useUIJoystick = dataManager.data.uiJoystick;
        uiButtonPedalsActive = dataManager.data.uiButtonPedals;

        if (useUIJoystick || isMobilePlatform)
        {
            PlayerController = GameObject.FindWithTag("PlayerController");
            PlayerController.GetComponent<CarController>().useUIjoystick = true;
            MobileUIObj.SetActive(true);

            if (uiButtonPedalsActive)
            {
                PlayerController.GetComponent<CarController>().UIbuttonPedals = true;
                MobileUIpedals.SetActive(true);
            }
            else
            {
                PlayerController.GetComponent<CarController>().UIbuttonPedals = false;
                MobileUIpedals.SetActive(false);
            }
        }
        else
        {
            MobileUIObj.SetActive(false);
            MobileUIpedals.SetActive(false);
        }

    }


    void Start()
    {
        gameObject.SetActive(false);
    }

}
