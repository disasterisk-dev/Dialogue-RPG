using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    public static CardManager instance;
    public static CardManager Instance { get { return instance; } }

    public List<Card> hookCards;
    public List<Card> eventCards;
    public List<Card> npcCards;
    public List<Item> itemCards;
    public List<Card> discard;

    public GameObject[] cards;
    public Button[] shuffles;

    void Awake() 
    {
        instance = this;
        ShuffleAll();
    }

    public void ShuffleAll()
    {
        Shuffle<Card>(hookCards);
        Shuffle<Card>(eventCards);
        Shuffle<Card>(npcCards);
        Shuffle<Item>(itemCards);
    }

    public void Shuffle<T>(List<T> ts) {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i) {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }

}
