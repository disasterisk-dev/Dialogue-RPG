using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proyecto26;
using FullSerializer;

public class Invites : MonoBehaviour
{
    public static fsSerializer serializer = new fsSerializer();

    [Header("UICards")]
    public GameObject[] zones;

    [Header("Prefabs")]
    public GameObject InviteCard;
    // Start is called before the first frame update
    void OnEnable()
    {
        PlayerData.Instance.invites.Clear();

        RestClient.Get(AccountManager.Instance.uri + "/campaigns.json?auth=" + AccountManager.Instance.idToken)
            .Then(response =>
            {
                fsData campaignData = fsJsonParser.Parse(response.Text);
                Dictionary<string, Campaign> campaigns = null;
                serializer.TryDeserialize(campaignData, ref campaigns);

                foreach (string k in PlayerData.Instance.user.invites)
                {
                    if (campaigns.ContainsKey(k))
                    {
                        PlayerData.Instance.invites.Add(new Invite()
                        {
                            title = campaigns[k].title,
                            key = k,
                            gm = campaigns[k].gamemaster,
                        });
                    }
                }

                Refresh();
            });
    }
    public void Refresh()
    {

        foreach (GameObject obj in zones)
        {
            if (obj.transform.childCount > 0)
            {
                foreach(GameObject child in obj.transform)
                {
                    Destroy(child);
                }
            }
        }

        if (PlayerData.Instance.invites.Count == 0)
        {
            Debug.Log("No Invites in inbox");
        }
        else
        {
            for (int i = 0; i < PlayerData.Instance.invites.Count; i++)
            {
                GameObject card = Instantiate(InviteCard, zones[i].transform);
                InvitationCard invitation = card.GetComponent<InvitationCard>();

                invitation.key = PlayerData.Instance.invites[i].key;
                invitation.title = PlayerData.Instance.invites[i].title;
                invitation.gameMaster = PlayerData.Instance.invites[i].gm;

                invitation.titleText.text = invitation.title;
                invitation.gameMasterText.text = invitation.gameMaster + " has invited you to join";
            }

            Debug.Log("Invite cards loaded");
        }

    }
}
