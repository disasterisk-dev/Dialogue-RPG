using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campaigns : MonoBehaviour
{
    public PlayerData playerData;

    [Header("UICards")]
    public GameObject [] zones;

    [Header("Prefabs")]
    public GameObject NewCampaign;
    public GameObject CampaignCard;
    // Start is called before the first frame update
    void Start()
    {
        playerData = GameObject.Find("PlayerDataManager").GetComponent<PlayerData>();
    }

    // Update is called once per frame
    void Awake()
    {
        playerData = GameObject.Find("PlayerDataManager").GetComponent<PlayerData>();
    }

    public void Refresh()
    {
        if (playerData.campaigns.Count == 0)
        {
            Instantiate(NewCampaign, zones[0].transform);
        }
        else
        {
            for(int i = 0; i < playerData.campaigns.Count; i++)
            {
                GameObject card = Instantiate(CampaignCard, zones[i].transform);
                CampaignCard campaign = card.GetComponent<CampaignCard>();

                //campaign.title.text = 
            }

            if(playerData.campaigns.Count < 4)
            {
                Instantiate(NewCampaign, zones[playerData.campaigns.Count].transform);
            }
        }
    }
}
