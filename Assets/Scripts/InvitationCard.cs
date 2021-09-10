using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Proyecto26;
using FullSerializer;

public class InvitationCard : MonoBehaviour
{
    public TMP_Text gameMasterText;
    public TMP_Text titleText;

    public string gameMaster;
    public string title;
    public string key;

    public static fsSerializer serializer = new fsSerializer();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Accept()
    {
        Campaign joinCamp;

        if (PlayerData.Instance.user.campaigns.Count < 4)
        {
            RestClient.Get<Campaign>(AccountManager.Instance.uri + "/campaigns/" + key + "/.json?auth=" + AccountManager.Instance.idToken)
            .Then(response =>
            {
                joinCamp = response;

                if (joinCamp.playerIds == null)
                    joinCamp.playerIds = new List<string>();

                if (joinCamp.playerNames == null)
                    joinCamp.playerNames = new List<string>();

                joinCamp.playerIds.Add(PlayerData.Instance.user.localId);
                joinCamp.playerNames.Add(PlayerData.Instance.user.username);
                RestClient.Put(AccountManager.Instance.uri + "/campaigns/" + key + ".json?auth=" + AccountManager.Instance.idToken, joinCamp)
                .Then(response2 =>
                {
                    //Adding campaign key to user entry

                    if (PlayerData.Instance.user.campaigns == null)
                        PlayerData.Instance.user.campaigns = new List<string>();

                    PlayerData.Instance.user.campaigns.Add(key);
                    PlayerData.Instance.user.invites.Remove(key);

                    RestClient.Put(AccountManager.Instance.uri + "/users/" + PlayerData.Instance.user.localId + ".json?auth=" + AccountManager.Instance.idToken, PlayerData.Instance.user)
                    .Then(response3 =>
                    {
                        Destroy(gameObject);
                        GameObject.Find("Invites").GetComponent<Invites>().Refresh();
                    })
                   .Catch(error3 =>
                    {
                        Debug.Log("Couldn't update player data:" + error3);
                    });
                })
               .Catch(error2 =>
                {
                    Debug.Log("Couldn't send updated campaign data:" + error2);
                });


            }).Catch(error =>
                {
                    Debug.Log("Couldn't retrieve campaign:" + error);
                });
        }
        else
        {
            UIManager.Instance.Warning("Your campaign limit has been reached, to join this campaign you must leave one that you are currently participating in.");
        }


    }

    public void Decline()
    {
        PlayerData.Instance.user.invites.Remove(key);

        RestClient.Put(AccountManager.Instance.uri + "/users/" + PlayerData.Instance.user.localId + ".json?auth=" + AccountManager.Instance.idToken, PlayerData.Instance.user)
        .Then(response3 =>
        {
            Destroy(gameObject);
            GameObject.Find("Invites").GetComponent<Invites>().Refresh();
        })
        .Catch(error3 =>
        {
            Debug.Log("Couldn't update player data:" + error3);
        });
    }
}
