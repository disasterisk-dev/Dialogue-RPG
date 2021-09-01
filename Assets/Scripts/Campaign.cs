using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Campaign
{
    public string name;
    public string key;
    public string genre;
    public string gameMaster;
    public string gmID;
    public List<string> playerIDs;
    public List<string> playerNames;

    public Campaign()
    {

    }

    public Campaign(string name, string key, string genre, string gameMaster)
    {
        this.name = name;
        this.key = key;
        this.genre = genre;
        this.gameMaster = gameMaster;
    }
    
}
