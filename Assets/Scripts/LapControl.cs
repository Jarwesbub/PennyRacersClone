using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LapControl : MonoBehaviour
{
    public DataManager dataManager;
    public Text LapsTxt, PosTxt;
    public int MaxLaps;
    public int Laps;
    GameObject AIController;
    public List<int> Leaderboards;
    public List<string> Names;
    public List<float> LapTimes;
    public float CurrentTime;
    public string PlayerName;
    public bool GameStart = false;

    // Start is called before the first frame update
    void Awake()
    {
        DrawLaps(1);
        dataManager.Load();
        PlayerName = dataManager.data.name;
    }
    void Update()
    {
        if(GameStart)
        {
            CurrentTime += Time.deltaTime;
        }


    }

    public void DrawLaps(int laps)
    {
        if(Laps < MaxLaps)
            Laps = laps;

        LapsTxt.text = "Laps: " + Laps.ToString() + " / " + MaxLaps.ToString();


    }

    public void FinishedPlayers(int number, string name) //-1 = player
    {
        Leaderboards.Add(number);
        LapTimes.Add(CurrentTime);
        if (name == "player")
        {
            Names.Add(PlayerName);
        }
        else
        {
            Names.Add(name);
        }
    }
}
