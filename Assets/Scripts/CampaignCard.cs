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
    public string campaignName;
    public string key;
    public string gmName;
    public string gmId;
    public string genre;
    public List<string> playerNames;
    public List<string> playerIDs;
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
            title.text = campaignName;
            gameMaster.text = gmName + "'s";

            if (playerNames != null)
            {
                players.text = "PLAYERS:\n\n";

                foreach (string name in playerNames)
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
}
