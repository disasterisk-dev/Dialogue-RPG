using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Campaign
{
    public string name;
    public string id;
    public string genre;
    public string gameMaster;
    
    public List<string> players;

    public Campaign()
    {

    }

    public Campaign(string name, string id, string genre, string gameMaster)
    {
        this.name = name;
        this.id = id;
        this.genre = genre;
        this.gameMaster = gameMaster;
    }
}
