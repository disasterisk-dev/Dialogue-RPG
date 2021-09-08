using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public static UIManager Instance { get { return instance; } }

    //Screen object variables
    public GameObject[] screens;
    public GameObject warning;
    public TMP_Text warningText;
    Campaigns campaigns;

    private void Awake()
    {
        instance = this;

        //campaigns = GameObject.Find("Campaigns").GetComponent<Campaigns>();

        LoadScreen(0);
    }

    //Functions to change the login screen UI

    public void ClearScreen() //Turn off all screens
    {
        warning.SetActive(false);
        foreach (GameObject obj in screens)
        {
            obj.SetActive(false);
        }
    }

    public void LoadScreen(int i)
    {
        ClearScreen();
        screens[i].SetActive(true);
    }

    public void Warning(string message)
    {
        warningText.text = message;
        warning.SetActive(true);
    }

    public void Okay()
    {
        warning.SetActive(false);
    }
}