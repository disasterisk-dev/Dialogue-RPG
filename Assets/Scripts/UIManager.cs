using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public static UIManager Instance { get { return instance; } }

    //Screen object variables
    public GameObject[] screens;
    public GameObject warning;
    public TMP_Text warningText;
    public GameObject rollDialogue;
    public GameObject cautionDialogue;
    public TMP_Text cautionText;
    public Button cautionButton;

    public TMP_Text rollStat, rollRoll, rollSum;
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
        rollDialogue.SetActive(false);
        cautionDialogue.SetActive(false);

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

    public void Caution(int functionCode)
    {
        switch(functionCode)
        {
            case 0:
                cautionText.text = "Warning! \n\n This campaign will be permanently deleted and all players will lose access";
                cautionButton.onClick.AddListener(() => ActiveCampaign.Instance.Delete());
                break;
            default:
                cautionButton.onClick.AddListener(() => Okay());
                break;
        }

        cautionDialogue.SetActive(true);
    }

    public void Roll(string stat, float roll, float statValue)
    {
        rollStat.text = stat;
        rollRoll.text = (roll + statValue).ToString();

        if(statValue < 0)
        {
            rollSum.text = "(" + roll.ToString() + statValue.ToString() + ")";
        }
        else
        {
            rollSum.text = "(" + roll.ToString() + "+" + statValue.ToString() + ")";
        }
        
        rollDialogue.SetActive(true);
    }

    public void Okay()
    {
        warning.SetActive(false);
        rollDialogue.SetActive(false);
        cautionDialogue.SetActive(false);
    }
}