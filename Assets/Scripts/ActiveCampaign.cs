using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Proyecto26;
using FullSerializer;

public class ActiveCampaign : MonoBehaviour
{

    public static fsSerializer serializer = new fsSerializer();

    [Header("Campaign Data")]
    public string campaignName;
    public string key;
    public string genre;
    public bool gm;
    public List<string> playerKeys;
    public List<bool> characterSetup;

    public Character[] characters;

    [Header("UI Elements")]
    public TMP_Text title;
    public GameObject[] zones;


    [Header("Settings UI Elements")]
    public GameObject gmSettings;
    public GameObject playerSettings;
    public GameObject invite;
    public TMP_InputField inviteField;

    void OnEnable()
    {
        campaignName = PlayerData.Instance.campaignName;
        key = PlayerData.Instance.key;
        genre = PlayerData.Instance.genre;
        gm = PlayerData.Instance.gm;

        title.text = campaignName;
        gmSettings.SetActive(false);
        playerSettings.SetActive(false);
        invite.SetActive(false);
        //firebaseManager.LoadCharacters(key);
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Settings()
    {
        if (gm)
        {
            if (!gmSettings.activeInHierarchy)
            {
                gmSettings.SetActive(true);
            }
            else if (gmSettings.activeInHierarchy)
            {
                gmSettings.SetActive(false);
            }
        }
        else
        {
            if (!playerSettings.activeInHierarchy)
            {
                playerSettings.SetActive(true);
            }
            else if (playerSettings.activeInHierarchy)
            {
                playerSettings.SetActive(false);
            }
        }
    }

    public void Delete()
    {
        RestClient.Delete(AccountManager.Instance.uri + "/campaigns/" + key + ".json?auth=" + AccountManager.Instance.idToken)
        .Then(response =>
        {
            PlayerData.Instance.user.campaigns.Remove(key);
            RestClient.Put(AccountManager.Instance.uri + "/users/" + AccountManager.Instance.localId + ".json?auth=" + AccountManager.Instance.idToken, PlayerData.Instance.user)
                    .Catch(error =>
                    {
                        Debug.Log(error);
                    });
            Debug.Log("Deleted");
            UIManager.Instance.LoadScreen(2);
        })
        .Catch(error =>
        {
            Debug.Log("Couldn't Delete campaign: " + error);
        });
    }

    public void ShowInvite()
    {
        invite.SetActive(true);
    }

    public void Invite()
    {

        if (inviteField.text != "")
        {
            RestClient.Get(AccountManager.Instance.uri + "/users.json?auth=" + AccountManager.Instance.idToken)
            .Then(response =>
            {
                fsData userData = fsJsonParser.Parse(response.Text);
                Dictionary<string, User> users = null;
                serializer.TryDeserialize(userData, ref users);

                bool playerFound = false;

                foreach (var user in users)
                {
                    if (user.Value.email == inviteField.text)
                    {
                        playerFound = true;
                        Debug.Log(user.Value.email);

                        User tempUser = user.Value;

                        if (tempUser.localId != PlayerData.Instance.user.localId)
                        {
                            if (!tempUser.invites.Contains(key))
                            {
                                Invite newInvite = new Invite()
                                {
                                    key = this.key,
                                    title = this.name,
                                    gm = PlayerData.Instance.user.username
                                };

                                if (tempUser.invites == null)
                                    tempUser.invites = new List<string>();

                                tempUser.invites.Add(key);

                                RestClient.Put(AccountManager.Instance.uri + "/users/" + tempUser.localId + ".json?auth=" + AccountManager.Instance.idToken, tempUser)
                                .Then(r =>
                                {
                                    UIManager.Instance.Warning(tempUser.username + " has been invited to join " + campaignName + "!");
                                })
                                .Catch(error =>
                                 {
                                     UIManager.Instance.Warning("Something went wrong, invitation not sent");
                                 });
                            }
                            else
                            {
                                UIManager.Instance.Warning(tempUser + " has already been invited to join this campaign");
                            }
                        }
                        else
                        {
                            UIManager.Instance.Warning("Cannot send invitations to yourself");
                        }
                    }
                }
                if (!playerFound)
                    UIManager.Instance.Warning("No player found with that email address");
            });


        }
        else
        {
            Debug.Log("No email entered");
        }
        invite.SetActive(false);
        inviteField.text = "";
    }

}
