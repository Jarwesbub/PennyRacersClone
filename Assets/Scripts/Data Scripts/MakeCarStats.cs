using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MakeCarStats : MonoBehaviour
{
    public InputField playerName;
    public int EngineClass;

    public Text t_engineclass;
    public Text t_acc;
    public Text t_aiengineclass;

    public DataManager dataManager;


    void Start()
    {
        dataManager.Load();
        playerName.text = dataManager.data.name;
        //t_engineclass.text = ("Engine Power =") + dataManager.data.EngineClass.ToString();

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

        int Acc = dataManager.data.AccLvl;
        if (Acc == 0)
        {
            Acc = 1;
            dataManager.data.AccLvl = Acc;
            dataManager.Save();
        }

        
        t_engineclass.text = ("Engine Power = ") + dataManager.data.EngineClass.ToString();
        t_acc.text = ("Acceleration = ") + dataManager.data.AccLvl.ToString();
        t_aiengineclass.text = ("AI's Engine Power = ") + dataManager.data.AIEngineClass.ToString();
    }

    public void ClickForEngineClass()
    {
        int engineClass = dataManager.data.EngineClass;

        if (engineClass < 6)
            engineClass += 1;
        else
            engineClass = 1;

        dataManager.data.EngineClass = engineClass;
        t_engineclass.text = ("Engine Power = ") + dataManager.data.EngineClass.ToString();

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
        t_acc.text = ("Acceleration = ") + dataManager.data.AccLvl.ToString();

    }

    public void ClickForAIEnginePower(bool SameAsPlayer)
    {
        int aiengineclass = dataManager.data.AIEngineClass;

        if (SameAsPlayer == false)
        {
            if (aiengineclass < 6)
                aiengineclass += 1;
            else
                aiengineclass = 1;
        }
        else
            aiengineclass = dataManager.data.EngineClass;//Same class as the player

        dataManager.data.AIEngineClass = aiengineclass;
        t_aiengineclass.text = ("AI's Engine Power = ") + dataManager.data.AIEngineClass.ToString();


    }

    public void ClickSaveAndContinue()
    {
        dataManager.data.name = playerName.text;
        dataManager.Save();
        //SceneManager.LoadScene("TestScene");
        SceneManager.LoadScene("Map1");

    }


}
