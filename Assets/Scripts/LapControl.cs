using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LapControl : MonoBehaviour
{
    public DataManager dataManager;
    private GameObject PlayerController, AICars;
    public GameObject UIGameplay, UIHighscores;
    public TMP_Text LapsTxt;
    public int maxLaps;
    public int laps;
    GameObject AIController;
    public List<string> lapTimes;
    public List<string> leaderboard;
    [SerializeField]
    public int leaderboardMaxLength;
    public TMP_Text playtime,leaderboards, laptimes;
    public float currentTime;
    public string playerName;
    public bool gameStart = false;
    private int skipFrames;

    // Start is called before the first frame update
    void Awake()
    {
        if (dataManager == null)
            dataManager = GameObject.FindWithTag("DataManager").GetComponent<DataManager>();

        PlayerController = GameObject.FindWithTag("PlayerController");
        AICars = GameObject.FindWithTag("AICars");
        UIGameplay = GameObject.FindWithTag("UIGameplay");
        AIController = GameObject.FindWithTag("aicontroller");

        if(UIHighscores==null)
            UIHighscores = GameObject.FindWithTag("UIHighscores");

        UIHighscores.SetActive(false);
        int player = 1;
        leaderboardMaxLength = AICars.transform.childCount + player;

        dataManager.Load();
        playerName = dataManager.data.name;
        maxLaps = dataManager.data.maxLaps;
        DrawLaps(1);
    }


    void Update()
    {
        if (gameStart)
        {
            currentTime += Time.deltaTime;
            skipFrames++;

            if (skipFrames > 10)
            {
                skipFrames = 0;
                ShowTime();
            }
        }
    }

    public void DrawLaps(int laps)
    {
        if(this.laps < maxLaps)
            this.laps = laps;

        LapsTxt.text = "Laps: " + this.laps.ToString() + " / " + maxLaps.ToString();

    }

    private void ShowTime()
    {
        float curtime = (Mathf.Round(currentTime * 100f) / 100f);
        string calculatedTime = ConvertTime(curtime); 
        playtime.text = calculatedTime;

    }

    private string ConvertTime(float curtime) 
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
        if (lapTimes.Count < leaderboardMaxLength && (name != "NotAItoday")) // Not a player who is using AIgroundControl-script
        {
            float curtime = Mathf.Round(currentTime * 100f) / 100f;
            string calculatedTime = ConvertTime(curtime);
            lapTimes.Add(calculatedTime);

            //Calculate car positions in leaderboard
            string pos = (1 + leaderboard.Count).ToString();
            string space = "  ";
            if (1 + leaderboard.Count >= 10)
            {
                space = " ";
            }

            if (name == "player")
            {
                name = playerName.ToString();
                leaderboard.Add(pos + space + name);
                //PLAYER AUTOPILOT ON!
                AIController.GetComponent<BotController>().StartPlayerAutopilot();
                PlayerController.SetActive(false); //Multiple leaderboards if true
                StartCoroutine(WaitHighscores());
            }
            else
            {
                leaderboard.Add(pos + space + name);
                GameObject ai = AICars.gameObject.transform.GetChild(number).gameObject;
                ai.GetComponent<BotGroundControl>().raceIsOver = true;
            }
            //Add leaderboard results in loop command
            string result = "";
            foreach (var listMember in leaderboard)
            {
                result += listMember.ToString() + "\n";
            }
            leaderboards.text = result;

            //Add all laptimes to single string "alltimes" in loop command
            string allTimes = "";
            foreach (var listMember in lapTimes)
            {
                allTimes += string.Format("{0:F2}", listMember).ToString() + "\n";
            }
            laptimes.text = allTimes;
        }
    }


    IEnumerator WaitHighscores()
    {
        yield return new WaitForSeconds(0.2f);
        //Hide player UI -> show highscores UI
        UIGameplay.SetActive(false);
        UIHighscores.SetActive(true);

    }
}
