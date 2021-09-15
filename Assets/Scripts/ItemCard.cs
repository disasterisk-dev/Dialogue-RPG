using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemCard : MonoBehaviour
{
    public Item itemData;
    public enum type {Weapon, Clothing, Relic, Misc};

    [Header("UI Elements")]
    public TMP_Text title;
    public TMP_Text copy;
    public TMP_Text mod;
    public Image image;

    public void SetData()
    {
        title.text = itemData.title;
        copy.text = itemData.text;
        image.sprite = itemData.sprite;
        
        if(itemData.bonus < 0)
        {
            mod.text = itemData.bonus.ToString() + " " + itemData.stat;
        }
        else
        {
            mod.text = "+" + itemData.bonus.ToString() + " " + itemData.stat;
        }

        Debug.Log(itemData.type);
    }
}
