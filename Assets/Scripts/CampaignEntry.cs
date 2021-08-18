using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampaignEntry
{
    public string name;
    public string genre;
    public string gameMaster;
    
    public string playerOne, playerTwo, playerThree, playerFour;

    public CampaignEntry()
    {

    }

    public CampaignEntry(string name, string genre, string gameMaster)
    {
        this.name = name;
        this.genre = genre;
        this.gameMaster = gameMaster;
    }

    public Dictionary<string, string> ToDictionary() {
        Dictionary<string, string> result = new Dictionary<string, string>();
        result["name"] = name;
        result["genre"] = genre;
        result["gameMaster"] = gameMaster;

        return result;
    }
}
