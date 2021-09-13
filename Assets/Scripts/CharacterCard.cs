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
    public Button[] buttons;

    // Start is called before the first frame update
    public void SetData()
    {
        charName.text = characterData.name;
        playerName.text = player;
        word.text = characterData.word.ToString();
        wit.text = characterData.wit.ToString();
        will.text = characterData.will.ToString();
        want.text = characterData.want.ToString();

        if(characterData.id != PlayerData.Instance.user.localId)
        {
            foreach(Button b in buttons)
            {
                b.interactable = false;
            }
        }
    }

    public void Roll(string stat)
    {
        float roll  = Random.Range(1,6);

        switch (stat)
        {
            case "word":
                UIManager.Instance.Roll("Word", roll, characterData.word);
                break;
            case "wit":
                UIManager.Instance.Roll("Wit", roll, characterData.wit);
                break;
            case "will":
                UIManager.Instance.Roll("Will", roll, characterData.will);
                break;
            case "want":
                UIManager.Instance.Roll("Want", roll, characterData.want);
                break;
        }
    }
}
