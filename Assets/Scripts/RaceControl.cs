using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RaceControl : MonoBehaviour
{
    public GameObject LapController;
    public bool gameStart;
    public TMP_Text CountText;

    // Start is called before the first frame update
    void Awake()
    {
        CountText.text = "";
        if (LapController == null)
            LapController = GameObject.FindWithTag("LapController");

        gameStart = false;
    }
    void Start()
    {
        StartCoroutine(Countdown());
    }

    IEnumerator Countdown()
    {
        int countNumber = 3;
        float firstWait = 2f;
        yield return new WaitForSeconds(firstWait); // 3->
        CountText.text = ""+countNumber.ToString();
        countNumber--;
        yield return new WaitForSeconds(1f);// 2->
        CountText.text = "" + countNumber.ToString();
        countNumber--;
        yield return new WaitForSeconds(1f);// 1->
        CountText.text = "" + countNumber.ToString();
        countNumber--;
        yield return new WaitForSeconds(1f);// 0 ->
        CountText.text = "GO!";
        gameStart = true;
        LapController.GetComponent<LapControl>().gameStart = true;
        yield return new WaitForSeconds(1f);
        CountText.text = "";
        
    }




}
