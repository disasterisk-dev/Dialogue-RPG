using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGear : MonoBehaviour
{
    public List<Item> items;
    public ItemCard card;
    public string type;
    int current;

    private void OnEnable()
    {
        current = 0;
        card.itemData = items[current];
        card.SetData();

        switch (type)
        {
            case "weapon":
                PlayerData.Instance.tempChar.weapon = items[current];
                break;
            case "clothing":
                PlayerData.Instance.tempChar.clothing = items[current];
                break;
            case "relic":
                PlayerData.Instance.tempChar.relic = items[current];
                break;
        }
    }

    public void Cycle()
    {
        if (current < items.Count-1)
        {
            current++;
        }
        else if (current == items.Count-1)
        {
            current = 0;
        }

        card.itemData = items[current];
        card.SetData();

        switch (type)
        {
            case "weapon":
                PlayerData.Instance.tempChar.weapon = items[current];
                break;
            case "clothing":
                PlayerData.Instance.tempChar.clothing = items[current];
                break;
            case "relic":
                PlayerData.Instance.tempChar.relic = items[current];
                break;
        }
    }
}
