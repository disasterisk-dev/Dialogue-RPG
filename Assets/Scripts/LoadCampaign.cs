using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadCampaign : MonoBehaviour
{
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

        //firebaseManager.LoadCharacters(this.GetComponent<CampaignCard>().key);
 
        PlayerData.Instance.campaignName = this.GetComponent<CampaignCard>().campaignName;
        PlayerData.Instance.key = this.GetComponent<CampaignCard>().key;
        PlayerData.Instance.genre = this.GetComponent<CampaignCard>().genre;
        PlayerData.Instance.gm = this.GetComponent<CampaignCard>().gmId == PlayerData.Instance.user.localId ? true : false;
        PlayerData.Instance.players = this.GetComponent<CampaignCard>().playerIDs;

        UIManager.Instance.LoadScreen(5);
    }
}
