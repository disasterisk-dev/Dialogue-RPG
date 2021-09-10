using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CharacterCard : MonoBehaviour
{
    public Character characterData;
    public string player;

    [Header("UI Elements")]
    public TMP_Text charName;
    public TMP_Text playerName;
    public TMP_Text word;
    public TMP_Text wit;
    public TMP_Text will;
    public TMP_Text want;

    public Image weapon;
    public Image clothing;
    public Image relic;

    // Start is called before the first frame update
    public void SetData()
    {
        charName.text = characterData.name;
        playerName.text = player;
        word.text = characterData.word.ToString();
        wit.text = characterData.wit.ToString();
        will.text = characterData.will.ToString();
        want.text = characterData.want.ToString();
    }
}
