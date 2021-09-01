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

        if (playerData.campaigns.Count == 0)
        {
            Instantiate(NewCampaign, zones[0].transform);
        }
        else
        {
            for (int i = 0; i < playerData.campaigns.Count; i++)
            {
                GameObject card = Instantiate(CampaignCard, zones[i].transform);
                CampaignCard campaign = card.GetComponent<CampaignCard>();

                campaign.key = playerData.campaigns[i].key;
                campaign.campaignName = playerData.campaigns[i].name;
                campaign.genre = playerData.campaigns[i].genre;
                campaign.gmName = playerData.campaigns[i].gameMaster;
                campaign.gmId = playerData.campaigns[i].gmID;
                campaign.playerNames = playerData.campaigns[i].playerNames;
                campaign.playerIDs = playerData.campaigns[i].playerIDs;

                campaign.SetData();
            }

            Debug.Log("Campaign cards loaded");

            if (playerData.campaigns.Count < 4)
            {
                Instantiate(NewCampaign, zones[playerData.campaigns.Count].transform);
            }
        }

    }
}
