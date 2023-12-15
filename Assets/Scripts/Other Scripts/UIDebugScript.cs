using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDebugScript : MonoBehaviour
{
    public Text ui_turn;
    public GameObject ui_drift;
    private GameObject PlayerController;
    [SerializeField]
    private bool isDrifting;


    // Start is called before the first frame update
    void Awake()
    {
        PlayerController = GameObject.FindWithTag("PlayerController");
        ui_drift.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        isDrifting = PlayerController.GetComponent<CarController>().isDrifting;
        float turning = PlayerController.GetComponent<CarController>().horizontalInput;

        if (isDrifting)
            ui_drift.SetActive(true);
        else
            ui_drift.SetActive(false);

        ui_turn.text = "Turn: " + turning.ToString();



    }
}
