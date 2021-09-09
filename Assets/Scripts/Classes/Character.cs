using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Character
{
    [Header("Basics")]
    public string name;
    public string id;

    [Header("Stats")]
    public float word;
    public float wit;
    public float will;
    public float want;

    [Header("Gear")]
    public string weapon;
    public string clothing;
    public string relic;

    [Header("Details")]
    public string background;
    public string features;
    public string notes;

    public Character()
    {

    }
    
    public Character(string name, string id, string campaign, float word, float wit, float will, float want, string weapon, string clothing, string relic, string background, string features)
    {

    }
    
}
