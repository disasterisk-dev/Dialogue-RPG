using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadCampaign : MonoBehaviour
{
    PlayerData playerData;
    UIManager uIManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Load()
    {
        playerData = GameObject.Find("PlayerDataManager").GetComponent<PlayerData>();

        playerData.campaignName = this.GetComponent<CampaignCard>().campaignName;
        playerData.key = this.GetComponent<CampaignCard>().key;
        playerData.genre = this.GetComponent<CampaignCard>().genre;
        playerData.gm = this.GetComponent<CampaignCard>().gmId == playerData.UID ? true : false;

        uIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        uIManager.LoadScreen(5);
    }
}
