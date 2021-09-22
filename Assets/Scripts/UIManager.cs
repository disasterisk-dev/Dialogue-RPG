using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public static UIManager Instance { get { return instance; } }

    public int version, release, patch;
    public TMP_Text versionText;
    //Screen object variables
    public GameObject[] screens;

    [Header("Warning Dialogue")]
    public GameObject warning;
    public TMP_Text warningText;

    [Header("Roll Dialogue")]
    public GameObject rollDialogue;
    public TMP_Text rollStat;
    public TMP_Text rollRoll;
    public TMP_Text rollSum;

    [Header("CautionDialogue")]
    public GameObject cautionDialogue;
    public TMP_Text cautionText;
    public Button cautionButton;

    [Header("Level Up Dialogue")]
    public GameObject levelUpDialogue;
    public TMP_Text levelStat;
    public TMP_Text levelNum;
    public Button increase;
    public Button decrease;
    public Button levelOkay;

    [Header("Invite Dialogue")]
    public GameObject InviteDialogue;

    [Header("Kick Dialogue")]
    public GameObject KickDialogue;
    public Button KickButton;

    Campaigns campaigns;

    private void Awake()
    {
        instance = this;
        //campaigns = GameObject.Find("Campaigns").GetComponent<Campaigns>();
        versionText.text = "Build " + Application.version;
        LoadScreen(0);
    }

    //Functions to change the login screen UI

    public void ClearScreen() //Turn off all screens
    {
        warning.SetActive(false);
        rollDialogue.SetActive(false);
        cautionDialogue.SetActive(false);
        levelUpDialogue.SetActive(false);
        InviteDialogue.SetActive(false);
        KickDialogue.SetActive(false);

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
        switch (functionCode)
        {
            case 0:
                cautionText.text = "Warning! \n\n This campaign will be permanently deleted and all players will lose access";
                cautionButton.onClick.AddListener(() => ActiveCampaign.Instance.Delete());
                break;
            case 1:
                cautionText.text = "Warning! \n\n This player will lose their character and all progress from this campaign";
                cautionButton.onClick.AddListener(() => ActiveCampaign.Instance.Kick(ActiveCampaign.Instance.kickField.text));
                break;
            case 2:
                cautionText.text = "Warning! \n\n You will not be able to rejoin unless invited and will lose this character permanently";
                cautionButton.onClick.AddListener(() => ActiveCampaign.Instance.Kick(PlayerData.Instance.user.username));
                break;
            default:
                cautionButton.onClick.AddListener(() => Okay());
                break;
        }

        cautionDialogue.SetActive(true);
    }

    public void Invite()
    {
        InviteDialogue.SetActive(true);
    }

    public void Kick()
    {
        KickDialogue.SetActive(true);
    }

    public void Roll(string stat, float roll, float statValue)
    {
        rollStat.text = stat;
        rollRoll.text = (roll + statValue).ToString();

        if (statValue < 0)
        {
            rollSum.text = "(" + roll.ToString() + statValue.ToString() + ")";
        }
        else
        {
            rollSum.text = "(" + roll.ToString() + "+" + statValue.ToString() + ")";
        }

        rollDialogue.SetActive(true);
    }

    public void LevelUp(string stat, float statValue, CharacterCard card)
    {
        float tempValue = 0;

        levelStat.text = stat;
        levelNum.text = statValue.ToString();

        increase.onClick.AddListener(() =>
        {
            if (statValue < 5)
            {
                tempValue = statValue + 1;
                levelNum.text = tempValue.ToString();
            }
        });

        decrease.onClick.AddListener(() =>
        {
            if (statValue > -5)
            {
                tempValue = statValue - 1;
                levelNum.text = tempValue.ToString();
            }
        });

        levelOkay.onClick.AddListener(() =>
        {
            if (tempValue != 0)
            {
                switch (stat)
                {
                    case "Word":
                        card.characterData.word = tempValue;
                        break;
                    case "Wit":
                        card.characterData.wit = tempValue;
                        break;
                    case "Will":
                        card.characterData.will = tempValue;
                        break;
                    case "Want":
                        card.characterData.want = tempValue;
                        break;
                }

                card.ConcludeLevelUp();
                Okay();
            }
        });

        levelUpDialogue.SetActive(true);
    }

    public void Okay()
    {
        warning.SetActive(false);
        rollDialogue.SetActive(false);
        cautionDialogue.SetActive(false);
        levelUpDialogue.SetActive(false);
        InviteDialogue.SetActive(false);
        KickDialogue.SetActive(false);
    }
}