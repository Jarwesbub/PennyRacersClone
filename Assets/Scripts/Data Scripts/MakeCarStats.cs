using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MakeCarStats : MonoBehaviour
{
    public GameObject UIJoystickButton;

    public InputField playerName;
    public int EngineClass,AIEngineClass;

    public TMP_Text t_engineclass;
    public TMP_Text t_acc;
    public TMP_Text t_aiengineclass;

    public DataManager dataManager;
    

    void Start()
    {
        dataManager.Load();
        UIJoystickButton.GetComponent<Toggle>().isOn = dataManager.data.UIJoystick;
        playerName.text = dataManager.data.name;
        //t_engineclass.text = ("Engine Power =") + dataManager.data.EngineClass.ToString();
        AIEngineClass = dataManager.data.AIEngineClass;
        FirstFrameCheck();
    }

    private void FirstFrameCheck()
    {
        
        int engineClass = dataManager.data.EngineClass;
        if (engineClass == 0)
        {
            engineClass = 1;
            dataManager.data.EngineClass = engineClass;
            dataManager.data.AIEngineClass = engineClass;
            dataManager.data.AccLvl = engineClass;
            dataManager.Save();
        }
        
        //If somehow Acceleration = 0 this fixes it to 1
        int Acc = dataManager.data.AccLvl;
        if (Acc == 0)
        {
            Acc = 1;
            dataManager.data.AccLvl = Acc;
            dataManager.Save();
        }
        
        
        t_engineclass.text = ("Engine Power\n") + dataManager.data.EngineClass.ToString();
        t_acc.text = ("Acceleration\n") + dataManager.data.AccLvl.ToString();
        t_aiengineclass.text = ("AI's Engine Power\n") + dataManager.data.AIEngineClass.ToString();
    }

    public void ClickForEngineClass()
    {
        int engineClass = dataManager.data.EngineClass;
        int maxValue = 6;
        if (engineClass < maxValue)
            engineClass += 1;
        else
            engineClass = 1;

        dataManager.data.EngineClass = engineClass;
        t_engineclass.text = ("Engine Power\n") + dataManager.data.EngineClass.ToString();

        ClickForAIEnginePower(true);

    }

    public void ClickForAccelaration()
    {
        int Acc = dataManager.data.AccLvl;

        if (Acc < 3)
            Acc += 1;
        else
            Acc = 1;

        dataManager.data.AccLvl = Acc;
        t_acc.text = ("Acceleration\n") + dataManager.data.AccLvl.ToString();

    }

    public void ClickForAIEnginePower(bool SameAsPlayer)
    {
        int aiengineclass = dataManager.data.AIEngineClass;

        if (SameAsPlayer == false)
        {
            if (aiengineclass < 5)
                aiengineclass += 1;
            else
                aiengineclass = 0;
        }
        else
            aiengineclass = dataManager.data.EngineClass;//Same class as the player

        
        dataManager.data.AIEngineClass = aiengineclass;
        t_aiengineclass.text = ("AI's Engine Power\n") + dataManager.data.AIEngineClass.ToString();

        
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
        Debug.Log(joystick);

        if (joystick)
        {
            dataManager.data.UIJoystick = true;
            dataManager.Save();
        }
        else
        {
            dataManager.data.UIJoystick = false;
            dataManager.Save();
        }
    }

}
