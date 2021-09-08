using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoMenuScript : MonoBehaviour
{
    /*
    public GameObject QuickMenu;
    private bool QuickMenuOpen;
    public GameObject AICntrl;

    void Awake()
    {
        AICntrl = GameObject.Find("AIController");
        AICntrl.SetActive(false);

        Time.timeScale = 0;
        QuickMenu.SetActive(true);
        QuickMenuOpen = true;
    }
    */
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("backspace"))
        {
            SceneManager.LoadScene("1MenuScene");

            /*
            if (QuickMenuOpen == false)
            {
                AICntrl.SetActive(false);
                Time.timeScale = 0;
                QuickMenu.SetActive(true);
                QuickMenuOpen = true;
            }
            else
            {
                AICntrl.SetActive(true);
                Time.timeScale = 1;
                QuickMenu.SetActive(false);
                QuickMenuOpen = false;
            }
            */
        }
            

        }
        /*
    public void CloseQuickMenuTab()
    {
        AICntrl.SetActive(true);
        Time.timeScale = 1;
        QuickMenu.SetActive(false);
        QuickMenuOpen = false;

    }
        */

}
