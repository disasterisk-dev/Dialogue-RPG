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
        if (!discarded)
        {
            ActiveCampaign.Instance.cardBlock.SetActive(false);

            CardManager.Instance.discard.Add(cardData);

            if (ActiveCampaign.Instance.discard.transform.childCount > 0)
            {
                foreach (Transform child in ActiveCampaign.Instance.discard.transform)
                {
                    Destroy(child.gameObject);
                }
            }

            gameObject.transform.SetParent(ActiveCampaign.Instance.discard.transform);
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localScale = Vector3.one;

            discarded = true;
        }
        else
        {
            ActiveCampaign.Instance.cardBlock.SetActive(true);

            CardManager.Instance.discard.Remove(cardData);

            gameObject.transform.SetParent(GameObject.Find("Gameplay").transform);
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localScale = new Vector3(2f, 2f, 1f);

            discarded = false;
        }
    }
}
