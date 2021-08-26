using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public static PlayerData instance;

    [Header("Credentials")]
    public string username;
    public string email;
    public string UID;

    [Header("Active Campaign")]
    public string campaignName;
    public string key;
    public string genre;
    public bool gm;

    [Header("Campaigns")]
    public List<string> campaignKeys;
    public List<string> campaignTitles;
    public List<string> campaignGmNames;
    public List<string> campaignGmIds;
    public List<string> campaignGenres;
    //public bool gameMaster;

    [Header("Invites")]
    public List<string> inviteEntryKeys;
    public List<string> inviteKeys;
    public List<string> inviteTitles;
    public List<string> inviteGms;

    [Header("New Campaign Temp Data")]
    public string tempGameMaster;
    public string tempGenre;
    public string tempCampaignName;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
