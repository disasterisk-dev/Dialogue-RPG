using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public static PlayerData instance;
    public static PlayerData Instance { get { return instance; } }

    [Header("Credentials")]
    public User user;

    [Header("Active Campaign")]
    public string campaignName;
    public string key;
    public string genre;
    public bool gm;
    public List<string> players;

    [Header("Campaigns")]

    public List<Campaign> campaigns;
    //public bool gameMaster;

    public List<Character> characters;

    [Header("Invites")]
    public List<string> inviteEntryKeys;
    public List<string> inviteKeys;
    public List<string> inviteTitles;
    public List<string> inviteGms;

    [Header("New Campaign Temp Data")]
    public string tempGameMaster;
    public string tempGenre;
    public string tempCampaignName;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
