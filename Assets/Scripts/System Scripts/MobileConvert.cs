using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileConvert : MonoBehaviour
{
    public DataManager dataManager;
    public bool IsMobilePlatform;
    [SerializeField]
    private bool useUIJoystick;
    public GameObject MobileUIObj;
    private GameObject PlayerController;

    void Awake()
    {
        dataManager.Load();
        useUIJoystick = dataManager.data.UIJoystick;

        if (useUIJoystick || IsMobilePlatform)
        {
            PlayerController = GameObject.FindWithTag("PlayerController");
            PlayerController.GetComponent<CarController>().useUIjoystick = true;
            MobileUIObj.SetActive(true);
        }
        else
        {
            MobileUIObj.SetActive(false);
        }

    }




    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        //Destroy(gameObject);
    }

}
