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
    public List<string> LapTimes;
    public List<string> Leaderboard;
    public TMP_Text playtime,leaderboards, laptimes;
    public float CurrentTime;
    public string PlayerName;
    public bool GameStart = false;
    private int SkipFrames;

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

        //CurrentTime = 200f; //TESTING DELETE!!!!!!!!!!!!!!!!!
    }
    void Update()
    {
        if(GameStart)
        {
            CurrentTime += Time.deltaTime;
            SkipFrames++;

            if (SkipFrames > 50)
            {
                SkipFrames = 0;
                ShowTime();
            }

        }


    }

    public void DrawLaps(int laps)
    {
        if(Laps < MaxLaps)
            Laps = laps;

        LapsTxt.text = "Laps: " + Laps.ToString() + " / " + MaxLaps.ToString();


    }
    private void ShowTime()
    {
        float curtime = (Mathf.Round(CurrentTime * 100f) / 100f);

        string calculatedTime = ConvertTime(curtime); //Can cause crashing!
        playtime.text = calculatedTime;

    }

    private string ConvertTime(float curtime) //Can cause crashing in update!
    {
        int minutes = 0;
        string result;

        while (curtime > 60f)
        {
            minutes++;
            curtime -= 60f;
        }

        string front = "";
        string second = "";

        if (curtime < 10)
        {
            second = "0";
        }
        if (minutes < 10)
        {
            front = "0";
        }

        result = (front + minutes + "," + second + string.Format("{00:F2}", curtime).ToString());
        return result;
    }

    public void FinishedPlayers(int number, string name) //-1 = player
    {
        //Calculate times
        //int minutes = 0;
        float curtime = Mathf.Round(CurrentTime * 100f) / 100f;

        string calculatedTime = ConvertTime(curtime);
        LapTimes.Add(calculatedTime);

        //Calculate car positions in leaderboard

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
        //Add leaderboard results in loop command
        string result = "";
        foreach (var listMember in Leaderboard)
        {
            result += listMember.ToString() + "\n";
        }
        leaderboards.text = result;

        //Add all laptimes to single string "alltimes" in loop command
        string allTimes = "";
        foreach (var listMember in LapTimes)
        {
            //string mins = minutes.ToString();
            //times += listMember.ToString() + "\n";
            allTimes += string.Format("{0:F2}", listMember).ToString()+"\n";
            
        }
        laptimes.text = allTimes;
    }

    IEnumerator WaitHighscores()
    {
        yield return new WaitForSeconds(0.2f);
        //Hide player UI -> show highscores UI
        UIGameplay.SetActive(false);
        UIHighscores.SetActive(true);

    }
}
