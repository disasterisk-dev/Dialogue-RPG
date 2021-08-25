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

    [Header("UI Elements")]
    public TMP_Text title;


    [Header("Settings UI Elements")]
    public GameObject settings;
    public GameObject invite;
    public TMP_InputField inviteField;

    void OnEnable()
    {
        playerData = GameObject.Find("PlayerDataManager").GetComponent<PlayerData>();
        campaignName = playerData.campaignName;
        key = playerData.key;
        genre = playerData.genre;
        gm = playerData.gm;

        title.text = campaignName;
        settings.SetActive(false);
        invite.SetActive(false);
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
        if (!settings.activeInHierarchy)
        {
            settings.SetActive(true);
        }
        else if (settings.activeInHierarchy)
        {
            settings.SetActive(false);
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

}
