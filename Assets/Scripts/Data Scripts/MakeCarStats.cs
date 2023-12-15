using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MakeCarStats : MonoBehaviour
{
    public GameObject UIJoystickButton, UIPedalsButton;
    private float Turning, Grip;

    public InputField playerName;
    public TMP_Text t_engineclass, t_acc, t_aiengineclass, t_maxlaps, t_turning, t_grip;
    public DataManager dataManager;

    private int engineClass, botEngineClass;
    private int maxLaps;

    void Awake()
    {
        if(dataManager == null)
            dataManager = GameObject.FindWithTag("DataManager").GetComponent<DataManager>();

    }


    void Start()
    {
        dataManager.Load();
        UIJoystickButton.GetComponent<Toggle>().isOn = dataManager.data.uiJoystick;
        UIPedalsButton.GetComponent<Toggle>().isOn = dataManager.data.uiButtonPedals;
        playerName.text = dataManager.data.name;
        ButtonSetMaxLaps(false);
        botEngineClass = dataManager.data.botEngineClass;
        FirstFrameCheck();
    }

    private void FirstFrameCheck()
    {
        
        int engineClass = dataManager.data.engineClass;
        if (engineClass == 0)
        {
            engineClass = 1;
            dataManager.data.engineClass = engineClass;
            dataManager.data.botEngineClass = engineClass;
            dataManager.data.accLvl = engineClass;
            dataManager.Save();
        }
        
        //If somehow Acceleration = 0 this fixes it to 1
        int Acc = dataManager.data.accLvl;
        if (Acc == 0)
        {
            Acc = 1;
            dataManager.data.accLvl = Acc;
            dataManager.Save();
        }
        GetCarEngineStats();
        GetCarAccStats();
        GetOtherStats();

        t_engineclass.text = ("Engine Power\n") + dataManager.data.engineClass.ToString();
        t_acc.text = ("Acceleration\n") + dataManager.data.accLvl.ToString();
        t_aiengineclass.text = ("AI's Engine Power\n") + dataManager.data.botEngineClass.ToString();
        t_aiengineclass.text = ("AI's Engine Power\n") + dataManager.data.botEngineClass.ToString();
        t_turning.text = ("Turning\n") + dataManager.data.turning.ToString();
        t_grip.text = ("Grip\n") + dataManager.data.grip.ToString();
    }

    private void GetCarEngineStats()
    {
        int engineClass = dataManager.data.engineClass;
        float enginePower = 2.48f;

        switch (engineClass)
        {
            case 1:
                enginePower = 2.48f;//2.48f//1: 90km/h, 3: 100km/h
                break;

            case 2:
                enginePower = 2.9f;//2.90f//1: 100km/h, 3: 110km/h
                break;

            case 3:
                enginePower = 3.4f;//3.27f//1: 120km/h, 3: 130km/h
                break;

            case 4:
                enginePower = 4.2f;//4.2f//1: 140km/h, 3: 150km/h
                break;

            case 5:
                enginePower = 5.2f;//5.2f//1: 160km/h, 3: 160km/h
                break;
            case 6:
                enginePower = 6.66f;//5.2f//1: 180km/h
                break;
        }

        dataManager.data.enginePower = enginePower;
        dataManager.Save();

    }

    private void GetCarAccStats()
    {
        int accLevel = dataManager.data.accLvl;
        float acc = 0.850f;

        if (accLevel == 1 || accLevel == 0)
        {
            acc = 0.850f; //0.845f
        }

        else if (accLevel == 2)
        {
            acc = 0.865f; //0.855f
        }

        else if (accLevel == 3)
        {
            acc = 0.88f; //0.87f
        }
        dataManager.data.acc = acc;
        dataManager.Save();

    }

    private void GetOtherStats()
    {
        float turning = dataManager.data.turning;
        float grip = dataManager.data.grip;

        if (turning == 0f)
            turning = 1f;

        if (grip == 0f)
            grip = 1f;

        Turning = turning;
        Grip = grip;

    }
    /// <summary>
    ///             CLICK BUTTONS->
    /// </summary>

    public void ClickForEngineClass()
    {
        int engineClass = dataManager.data.engineClass;
        int maxValue = 6;
        if (engineClass < maxValue)
            engineClass += 1;
        else
            engineClass = 1;

        dataManager.data.engineClass = engineClass;
        GetCarEngineStats();
        t_engineclass.text = ("Engine Power\n") + dataManager.data.engineClass.ToString();

        ClickForAIEnginePower(true);

    }
    public void ClickForTurning(bool isPositive)
    {
        float add = 0.1f;
        if (isPositive)
        {
            Turning += add;

        }
        else
        {
            Turning -= add;

        }
        dataManager.data.turning = Turning;
        t_turning.text = ("Turning\n") + dataManager.data.turning.ToString();
    }
    public void ClickForGrip(bool isPositive)
    {
        float add = 0.1f;
        if (isPositive)
        {
            Grip += add;
        }
        else
        {
            Grip -= add;
        }
        dataManager.data.grip = Grip;
        t_grip.text = ("Grip\n") + dataManager.data.grip.ToString();
    }


    public void ClickForAccelaration()
    {
        int accLVL = dataManager.data.accLvl;

        if (accLVL < 3)
            accLVL += 1;
        else
            accLVL = 1;

        dataManager.data.accLvl = accLVL;
        GetCarAccStats();
        t_acc.text = ("Acceleration\n") + dataManager.data.accLvl.ToString();

    }

    public void ClickForAIEnginePower(bool SameAsPlayer)
    {
        int aiengineclass = dataManager.data.botEngineClass;

        if (SameAsPlayer == false)
        {
            if (aiengineclass < 5)
                aiengineclass += 1;
            else
                aiengineclass = 0;
        }
        else
            aiengineclass = dataManager.data.engineClass;//Same class as the player

        
        dataManager.data.botEngineClass = aiengineclass;
        t_aiengineclass.text = ("AI's Engine Power\n") + dataManager.data.botEngineClass.ToString();

        
    }

    public void ButtonGoMap1()
    {
        dataManager.data.name = playerName.text;
        dataManager.Save();
        SceneManager.LoadScene("Map1");

    }
    public void ButtonGoTestmap()
    {
        dataManager.data.name = playerName.text;
        dataManager.Save();
        SceneManager.LoadScene("Map2");
    }
    
    public void ToggleButtonUseUIJoystick()
    {
        bool joystick = UIJoystickButton.GetComponent<Toggle>().isOn;

        if (joystick)
        {
            dataManager.data.uiJoystick = true;
            dataManager.Save();
        }
        else
        {
            dataManager.data.uiJoystick = false;
            dataManager.Save();
        }
    }

    public void ToggleButtonUseUIPedals()
    {
        bool usePedals = UIPedalsButton.GetComponent<Toggle>().isOn;

        if (usePedals)
        {
            dataManager.data.uiButtonPedals = true;
            dataManager.Save();
        }
        else
        {
            dataManager.data.uiButtonPedals = false;
            dataManager.Save();
        }
    }

    public void ButtonSetMaxLaps(bool IsButton)
    {
        int maxLaps = 6;

        dataManager.Load();
        this.maxLaps = dataManager.data.maxLaps;
        if (this.maxLaps == 0)
            this.maxLaps = 1;


        if (IsButton)
        {
            if (this.maxLaps < maxLaps)
            {
                this.maxLaps++;
            }
            else
            {
                this.maxLaps = 1;
            }
        }
        t_maxlaps.text = ("Max Laps = ") + this.maxLaps.ToString();
        dataManager.data.maxLaps = this.maxLaps;
        dataManager.Save();
    }
}
