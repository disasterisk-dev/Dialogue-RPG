using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CampaignCard : MonoBehaviour
{

    [Header("Elements")]
    public TMP_Text title;
    public TMP_Text gameMaster;
    public TMP_Text players;
    public GameObject text;

    [Header("Data")]
    public Campaign campaignData = new Campaign();
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetData()
    {
        try
        {
            title.text = campaignData.title;
            gameMaster.text = campaignData.gamemaster + "'s";

            if (campaignData.playerNames != null)
            {
                players.text = "PLAYERS:\n\n";

                foreach (string name in campaignData.playerNames)
                {
                    players.text += name + "\n";
                }
            }
            else
            {
                players.text = "";
            }


        }
        catch
        {
            Debug.Log("bitch ass exception strikes again");
        }
    }
    public void Load()
    {
        PlayerData.Instance.activeCampaign = campaignData;

        UIManager.Instance.LoadScreen(5);
    }
}
