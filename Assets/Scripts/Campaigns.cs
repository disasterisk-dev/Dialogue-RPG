using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campaigns : MonoBehaviour
{
    public PlayerData playerData;

    [Header("UICards")]
    public GameObject[] zones;

    [Header("Prefabs")]
    public GameObject NewCampaign;
    public GameObject CampaignCard;
    // Start is called before the first frame update
    void Start()
    {
        playerData = GameObject.Find("PlayerDataManager").GetComponent<PlayerData>();

        // foreach (GameObject obj in zones)
        // {
        //     if (obj.transform.childCount > 0)
        //     {
        //         Destroy(obj.transform.GetChild(0).gameObject);
        //     }
        // }
    }

    // Update is called once per frame
    void Awake()
    {
        //Refresh();
    }

    public void Refresh()
    {

        playerData = GameObject.Find("PlayerDataManager").GetComponent<PlayerData>();

        foreach (GameObject obj in zones)
        {
            if (obj.transform.childCount > 0)
            {
                foreach(Transform child in obj.transform)
                {
                    Destroy(child.gameObject);
                }
            }
        }

        if (playerData.campaignKeys.Count == 0)
        {
            Instantiate(NewCampaign, zones[0].transform);
        }
        else
        {
            for (int i = 0; i < playerData.campaignKeys.Count; i++)
            {
                GameObject card = Instantiate(CampaignCard, zones[i].transform);
                CampaignCard campaign = card.GetComponent<CampaignCard>();

                campaign.key = playerData.campaignKeys[i];
                campaign.campaignName = playerData.campaignTitles[i];
                campaign.genre = playerData.campaignGenres[i];
                campaign.gmName = playerData.campaignGmNames[i];
                campaign.gmId = playerData.campaignGmIds[i];

                campaign.SetData();
            }

            Debug.Log("Campaign cards loaded");

            if (playerData.campaignKeys.Count < 4)
            {
                Instantiate(NewCampaign, zones[playerData.campaignKeys.Count].transform);
            }
        }

    }
}
