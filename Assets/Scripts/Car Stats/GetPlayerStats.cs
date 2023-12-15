using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GetPlayerStats : MonoBehaviour
{
    public DataManager dataManager;
    public GameObject PlayerController;
    public GameObject AIController;

    public TMP_Text t_playername;

    private int engineClass, aiEngineClass;
    private float enginePower, acc, turning, grip;

    void Awake()
    {
        GetAllStats();
        
        PlayerController.GetComponent<CarController>().enginePower = enginePower;
        PlayerController.GetComponent<CarController>().acceleration = acc;
        PlayerController.GetComponent<CarController>().turning = turning;
        PlayerController.GetComponent<CarController>().grip = grip;
        AIController.GetComponent<BotController>().botEngineClass = aiEngineClass;//AI 
        
    }
    void GetAllStats()
    {
        dataManager.Load();
        t_playername.text = ("Player: ") + dataManager.data.name;

        engineClass = dataManager.data.engineClass;
        enginePower = dataManager.data.enginePower;
        acc = dataManager.data.acc;
        turning = dataManager.data.turning;
        grip = dataManager.data.grip;
        aiEngineClass = dataManager.data.botEngineClass;
    }

}
