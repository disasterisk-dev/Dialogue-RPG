using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager instance;
    public static CardManager Instance { get { return instance; } }

    public List<Card> hookCards;
    public List<Card> eventCards;
    public List<Card> npcCards;
    public List<Item> itemCards;
    public List<Card> discard;

    void Awake() 
    {
        instance = this;
    }
}
