using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ActiveCampaign : MonoBehaviour
{
    PlayerData playerData;
    FirebaseManager firebaseManager;
    UIManager uIManager;

    [Header("Campaign Data")]
    public string campaignName;
    public string key;
    public string genre;
    public bool gm;
    public List<string> playerKeys;
    public List<bool> characterSetup;

    public Character [] characters;

    [Header("UI Elements")]
    public TMP_Text title;
    public GameObject [] zones;


    [Header("Settings UI Elements")]
    public GameObject gmSettings;
    public GameObject playerSettings;
    public GameObject invite;
    public TMP_InputField inviteField;
    public TMP_Text inviteInfo;

    void OnEnable()
    {
        playerData = GameObject.Find("PlayerDataManager").GetComponent<PlayerData>();
        firebaseManager = GameObject.Find("FirebaseManager").GetComponent<FirebaseManager>();
        campaignName = playerData.campaignName;
        key = playerData.key;
        genre = playerData.genre;
        gm = playerData.gm;

        title.text = campaignName;
        gmSettings.SetActive(false);
        playerSettings.SetActive(false);
        invite.SetActive(false);
        inviteInfo.gameObject.SetActive(false);
        //firebaseManager.LoadCharacters(key);
    }

    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Settings()
    {
        if (gm)
        {
            if (!gmSettings.activeInHierarchy)
            {
                gmSettings.SetActive(true);
            }
            else if (gmSettings.activeInHierarchy)
            {
                gmSettings.SetActive(false);
            }
        }
        else
        {
            if (!playerSettings.activeInHierarchy)
            {
                playerSettings.SetActive(true);
            }
            else if (playerSettings.activeInHierarchy)
            {
                playerSettings.SetActive(false);
            }
        }
    }

    public void Delete()
    {
        firebaseManager = GameObject.Find("FirebaseManager").GetComponent<FirebaseManager>();
        uIManager = GameObject.Find("UIManager").GetComponent<UIManager>();

        firebaseManager.DeleteCampaign(key);
        uIManager.LoadScreen(2);
    }

    public void ShowInvite()
    {
        invite.SetActive(true);
        inviteInfo.gameObject.SetActive(false);
    }

    public void Invite()
    {
        if (inviteField.text != "")
        {
            Debug.Log(inviteField.text);

            firebaseManager = GameObject.Find("FirebaseManager").GetComponent<FirebaseManager>();
            firebaseManager.Invite(inviteField.text);

            inviteField.text = "";
            invite.SetActive(false);
        }
        else
        {
            Debug.Log("No email entered");
        }
    }

    public void InviteInfo(int output)
    {
        switch (output)
        {
            case 0:
                inviteInfo.text = "Invite sent!";
                break;
            case 1:
                inviteInfo.text = "Player not found";
                break;
            case 2:
                inviteInfo.text = "Player's inbox is full";
                break;
            case 3:
                inviteInfo.text = "Invalid email entered";
                break;
            default:
                inviteInfo.text = "Something went wrong";
                break;
        }
        inviteInfo.gameObject.SetActive(true);
    }

}
