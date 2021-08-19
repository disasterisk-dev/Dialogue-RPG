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

    [Header("Campaigns")]
    public List<string> campaignKeys;
    public List<string> campaignTitles;
    public List<string> campaignGms;
    public List<string> campaignGenres;
    //public bool gameMaster;

    [Header("New Campaign Temp Data")]
    public string gameMaster;
    public string genre;
    public string campaignName;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
