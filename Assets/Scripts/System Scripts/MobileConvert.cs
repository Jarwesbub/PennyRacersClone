using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileConvert : MonoBehaviour
{
    public DataManager dataManager;
    public bool IsMobilePlatform;
    [SerializeField]
    private bool useUIJoystick, UIbuttonPedalsActive;
    public GameObject MobileUIObj, MobileUIpedals;
    private GameObject PlayerController;

    void Awake()
    {
        dataManager.Load();
        useUIJoystick = dataManager.data.UIJoystick;
        UIbuttonPedalsActive = dataManager.data.UIbuttonPedals;

        if (useUIJoystick || IsMobilePlatform)
        {
            PlayerController = GameObject.FindWithTag("PlayerController");
            PlayerController.GetComponent<CarController>().useUIjoystick = true;
            MobileUIObj.SetActive(true);

            if (UIbuttonPedalsActive)
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




    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        //Destroy(gameObject);
    }

}
