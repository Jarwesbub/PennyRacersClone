using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



/// <summary>
/// This script calculate the current fps and show it to a text ui.
/// </summary>
public class FpsCalculator : MonoBehaviour
{
    public Text FPStext;

    public string formatedString = "{value} FPS";
    //public Text txtFps;

    public float updateRateSeconds = 4.0F;

    int frameCount = 0;
    float dt = 0.0F;
    float fps = 0.0F;

    void Update()
    {
        frameCount++;
        dt += Time.unscaledDeltaTime;
        if (dt > 1.0 / updateRateSeconds)
        {
            fps = frameCount / dt;
            frameCount = 0;
            dt -= 1.0F / updateRateSeconds;
        }
        //txtFps.text = formatedString.Replace("{value}", System.Math.Round(fps, 1).ToString("0.0"));

        FPStext.text = "FPS: " + System.Math.Round(fps, 1).ToString();

    }
    /*
    private void OnGUI()
    {
        //Fetch the PlayerPrefs settings and output them to the screen using Labels
        GUI.contentColor = Color.black;
        GUI.Label(new Rect(50, 50, 200, 30), formatedString.Replace("{value}", System.Math.Round(fps, 1).ToString("0.0")));
        
    }
    */
}