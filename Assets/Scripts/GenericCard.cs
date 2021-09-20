using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GenericCard : MonoBehaviour
{
    public Card cardData;

    [Header("UI Elements")]
    public TMP_Text title;
    public TMP_Text copy;
    public Image image;

    public bool discarded = false;

    public void SetData()
    {
        title.text = cardData.title;
        copy.text = cardData.text;
        image.sprite = cardData.sprite;
    }

    public void Discard()
    {
        ActiveCampaign.Instance.Block();
        CardManager.Instance.discard.Remove(cardData);
        Destroy(gameObject);
    }
}
