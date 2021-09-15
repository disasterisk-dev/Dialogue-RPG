using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Item
{
    public string title;
    [TextArea]public string text;
    public Sprite sprite;
    public string stat;
    public float bonus;
    public float level;
    public enum cardType {weapon, clothing, relic, misc};
    public cardType type;
}
