using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CampaignCard : MonoBehaviour
{
    FirebaseManager fb;

    [Header("Elements")]
    public TMP_Text title;
    public TMP_Text gameMaster;
    public TMP_Text players;

    [Header("Data")]
    public string campaignName;
    public string key;
    public string gmName;
    public string[] playerNames;
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
            players.text = playerNames[1];
        }
        catch
        {
            Debug.Log("bitch ass exception strikes again");
        }
    }

    public void Delete()
    {
        fb = GameObject.Find("FirebaseManager").GetComponent<FirebaseManager>();

        fb.DeleteCampaign(key);
    }
}
