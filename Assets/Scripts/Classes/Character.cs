using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Character
{
    [Header("Basics")]
    public string name;
    public string id;

    [Header("Details")]
    [TextArea] public string background;
    [TextArea] public string features;

    [Header("Stats")]
    public float word;
    public float wit;
    public float will;
    public float want;

    [Header("Gear")]
    public Item weapon;
    public Item clothing;
    public Item relic;

    [Header("Levelling")]
    public float level = 1;
    public float points = 0;

    public Character()
    {

    }
    
}
