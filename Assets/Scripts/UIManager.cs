using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    //Screen object variables
    public GameObject [] screens;

    Campaigns campaigns;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }

        //campaigns = GameObject.Find("Campaigns").GetComponent<Campaigns>();

        LoadScreen(0);
    }

    //Functions to change the login screen UI

    public void ClearScreen() //Turn off all screens
    {
        foreach(GameObject obj in screens)
        {
            obj.SetActive(false);
        }
    }

    public void LoadScreen(int i)
    {
        ClearScreen();
        screens[i].SetActive(true);
    }
}