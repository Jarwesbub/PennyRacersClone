using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LapControl : MonoBehaviour
{
    public DataManager dataManager;
    public GameObject PlayerController, PlayerCar, AICars;
    public GameObject UIGameplay, UIHighscores;
    public TMP_Text LapsTxt/*, PosTxt*/;
    public int MaxLaps;
    public int Laps;
    GameObject AIController;
    public List<float> LapTimes;
    public List<string> Leaderboard;
    public TMP_Text leaderboards, laptimes;
    public float CurrentTime;
    public string PlayerName;
    public bool GameStart = false;
    private bool GameEnd = false;

    // Start is called before the first frame update
    void Awake()
    {
        if (PlayerController == null)
            PlayerController = GameObject.FindWithTag("PlayerController");
        if (PlayerCar == null)
            PlayerCar = GameObject.FindWithTag("Player");
        if (AICars == null)
            AICars = GameObject.FindWithTag("AICars");
        if (UIGameplay == null)
            UIGameplay = GameObject.FindWithTag("UIGameplay");
        if (UIHighscores == null)
            UIHighscores = GameObject.FindWithTag("UIHighscores");

        UIHighscores.SetActive(false);

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
        else if (GameEnd)
        {
            

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
        //Leaderboards.Add(number);
        int minutes = 0;
        
        LapTimes.Add(Mathf.Round(CurrentTime * 100f) / 100f);
        for (float i = CurrentTime; i > 60f; i-=60f)
        {
            minutes++;
        }

        string front = "0"; // When time is "01" (digits)

        if (minutes >= 10) //When time is "10" (digits)
        {
            front = "";
        }

        string pos = (1+Leaderboard.Count).ToString();
        string space = "  ";
        if(1+Leaderboard.Count >= 10)
        {
            space = " ";
        }

        if (name == "player")
        {
            //Names.Add(PlayerName);
            name = PlayerName.ToString();
            Leaderboard.Add(pos + space + name);
            PlayerController.GetComponent<CarController>().Autopilot = true;
            PlayerCar.GetComponent<CarGroundControl>().Autopilot = true;
            StartCoroutine(WaitHighscores());
        }
        else
        {
            //Names.Add(name);
            Leaderboard.Add(pos + space + name);
            GameObject ai = AICars.gameObject.transform.GetChild(number).gameObject;
            ai.GetComponent<AIGroundControl>().RaceIsOver = true;
        }

        string result = "";
        foreach (var listMember in Leaderboard)
        {
            result += listMember.ToString() + "\n";
        }
        leaderboards.text = result;

        string times = "";
        foreach (var listMember in LapTimes)
        {
            string mins = minutes.ToString();
            //times += listMember.ToString() + "\n";
            times += front + mins + "," + string.Format("{0:F2}", listMember).ToString()+"\n";
        }
        laptimes.text = times;
    }

    IEnumerator WaitHighscores()
    {
        yield return new WaitForSeconds(0.2f);

        UIGameplay.SetActive(false);
        UIHighscores.SetActive(true);

    }
}
