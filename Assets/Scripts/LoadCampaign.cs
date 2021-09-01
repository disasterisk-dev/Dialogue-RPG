using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadCampaign : MonoBehaviour
{
    PlayerData playerData;
    FirebaseManager firebaseManager;
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
        firebaseManager = GameObject.Find("FirebaseManager").GetComponent<FirebaseManager>();

        //firebaseManager.LoadCharacters(this.GetComponent<CampaignCard>().key);

        playerData.campaignName = this.GetComponent<CampaignCard>().campaignName;
        playerData.key = this.GetComponent<CampaignCard>().key;
        playerData.genre = this.GetComponent<CampaignCard>().genre;
        playerData.gm = this.GetComponent<CampaignCard>().gmId == playerData.UID ? true : false;
        playerData.players = this.GetComponent<CampaignCard>().playerIDs;

        uIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        uIManager.LoadScreen(5);
    }
}
