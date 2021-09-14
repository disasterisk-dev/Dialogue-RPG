using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Card
{
    public string title;
    [TextArea]public string text;
    public Sprite sprite;
}
