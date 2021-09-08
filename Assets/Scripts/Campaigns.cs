using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Proyecto26;
using FullSerializer;

public class Campaigns : MonoBehaviour
{
    public TMP_Text title;

    [Header("UICards")]
    public GameObject[] zones;

    [Header("Prefabs")]
    public GameObject NewCampaign;
    public GameObject CampaignCard;
    // Start is called before the first frame update

    public static fsSerializer serializer = new fsSerializer();

    void OnEnable()
    {
        title.text = PlayerData.Instance.user.username + "'s Campaigns";
        PlayerData.Instance.campaigns.Clear();

        RestClient.Get(AccountManager.Instance.uri + "/campaigns.json?auth=" + AccountManager.Instance.idToken)
            .Then(response =>
            {
                fsData campaignData = fsJsonParser.Parse(response.Text);
                Dictionary<string, Campaign> campaigns = null;
                serializer.TryDeserialize(campaignData, ref campaigns);

                foreach (string k in PlayerData.Instance.user.campaigns)
                {
                    if (campaigns.ContainsKey(k))
                    {
                        if (campaigns[k].playerIds != null)
                        {
                            foreach (string id in campaigns[k].playerIds)
                            {
                                RestClient.Get(AccountManager.Instance.uri + "/users/" + id + ".json?auth=" + AccountManager.Instance.idToken)
                                .Then(response2 =>
                                {
                                    fsData userData = fsJsonParser.Parse(response2.Text);
                                    User user = null;
                                    serializer.TryDeserialize(userData, ref user);

                                    if(campaigns[k].playerNames == null)
                                        campaigns[k].playerNames = new List<string>();

                                    campaigns[k].playerNames.Add(user.username);
                            })
                                .Catch(error =>
                                {
                                    Debug.Log(error);
                                });
                            }
                        }

                        PlayerData.Instance.campaigns.Add(campaigns[k]);
                    }
                }
                Debug.Log("Loading");
                Refresh();
            });
    }

    void Start()
    {

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


        foreach (GameObject obj in zones)
        {
            if (obj.transform.childCount > 0)
            {
                foreach (Transform child in obj.transform)
                {
                    Destroy(child.gameObject);
                }
            }
        }

        if (PlayerData.Instance.campaigns.Count == 0)
        {
            Instantiate(NewCampaign, zones[0].transform);
        }
        else
        {
            for (int i = 0; i < PlayerData.Instance.campaigns.Count; i++)
            {
                GameObject card = Instantiate(CampaignCard, zones[i].transform);
                CampaignCard campaign = card.GetComponent<CampaignCard>();

                campaign.key = PlayerData.Instance.campaigns[i].key;
                campaign.campaignName = PlayerData.Instance.campaigns[i].title;
                campaign.genre = PlayerData.Instance.campaigns[i].genre;
                campaign.gmName = PlayerData.Instance.campaigns[i].gamemaster;
                campaign.gmId = PlayerData.Instance.campaigns[i].gmid;
                campaign.playerNames = PlayerData.Instance.campaigns[i].playerNames;
                campaign.playerIDs = PlayerData.Instance.campaigns[i].playerIds;

                campaign.SetData();
            }

            Debug.Log("Campaign cards loaded");

            if (PlayerData.Instance.campaigns.Count < 4)
            {
                Instantiate(NewCampaign, zones[PlayerData.Instance.campaigns.Count].transform);
            }
        }

    }
}
