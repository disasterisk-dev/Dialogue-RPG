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

    [Header("Data")]
    public string campaignName;
    public string gmName;
    public string [] playerNames;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
