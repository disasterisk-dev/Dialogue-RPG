using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Proyecto26;
using FullSerializer;
public class NewCampaign : MonoBehaviour
{
    public TMP_InputField nameInput;

    public static fsSerializer serializer = new fsSerializer();

    void Awake()
    {
    }

    public void New()
    {
        UIManager.Instance.LoadScreen(3); //load genre selection screen
        //fm.CreateCampaign();
        //fm.Load();

    }

    public void Genre(string genre)
    {
        PlayerData.Instance.tempGenre = genre;
        UIManager.Instance.LoadScreen(4);
    }

    public void Create()
    {
        if (nameInput.text != "")
        {
            Campaign campaign = new Campaign()
            {
                title = nameInput.text,
                genre = PlayerData.Instance.tempGenre,
                gamemaster = PlayerData.Instance.user.username,
                gmid = PlayerData.Instance.user.localId
            };

            RestClient.Post(AccountManager.Instance.uri + "/campaigns/.json?auth=" + AccountManager.Instance.idToken, campaign)
            .Then(response =>
            {
                Debug.Log(response.Text);
                fsData data = fsJsonParser.Parse(response.Text);
                Dictionary<string, string> output = null;
                serializer.TryDeserialize(data, ref output);

                foreach (var s in output.Values)
                {
                    campaign.key = s;
                    RestClient.Put(AccountManager.Instance.uri + "/campaigns/" + s + ".json?auth=" + AccountManager.Instance.idToken, campaign)
                    .Then(response2 =>
                    {
                        PlayerData.Instance.user.campaigns.Add(s);
                        RestClient.Put(AccountManager.Instance.uri + "/users/" + AccountManager.Instance.localId + ".json?auth=" + AccountManager.Instance.idToken, PlayerData.Instance.user)
                        .Catch(error =>
                        {
                            Debug.Log(error);
                        });

                        UIManager.Instance.LoadScreen(2);
                    });



                }
            })
            .Catch(error =>
            {
                Debug.Log(error);
            });

            nameInput.text = "";
            //fm.Load();
            UIManager.Instance.LoadScreen(2); //loads campaign screen
        }
    }
}
