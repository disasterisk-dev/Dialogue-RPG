using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Campaign
{
    public string title;
    public string key;
    public string genre;
    public string gamemaster;
    public string gmid;
    public List<string> playerIds;
    public List<string> playerNames;
    public List<Character> characters;

    public Campaign()
    {

    }

    public Campaign(string title, string key, string genre, string gameMaster)
    {
        this.title = title;
        this.key = key;
        this.genre = genre;
        this.gamemaster = gameMaster;
    }
    
}
