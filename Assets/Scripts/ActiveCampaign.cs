using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Proyecto26;
using FullSerializer;

public class ActiveCampaign : MonoBehaviour
{
    public static ActiveCampaign instance;
    public static ActiveCampaign Instance { get { return instance; } }
    public static fsSerializer serializer = new fsSerializer();
    bool inGame;

    [Header("Campaign Data")]
    public Campaign activeCampaign = new Campaign();
    public bool isGM;
    public bool hasChar;

    [Header("UI Elements")]
    public TMP_Text title;
    public GameObject[] zones;


    [Header("Settings UI Elements")]
    public GameObject gmDrawer;
    public GameObject discard;
    public GameObject cardBlock;
    public GameObject gmSettings;
    public GameObject playerSettings;
    public GameObject invite;
    public TMP_InputField inviteField;
    public GameObject settingsButton;
    public GameObject[] cardButtons;

    [Header("Prefabs")]
    public GameObject newCharacter;
    public GameObject characterCard;
    public GameObject genericCard;
    public GameObject itemCard;

    [Header("Item Giving")]
    public bool giving;
    public Item giveItem = new Item();

    public bool blocked;

    void Awake()
    {
        instance = this;
    }

    void OnEnable()
    {
        activeCampaign = PlayerData.Instance.activeCampaign;

        isGM = activeCampaign.gmid == PlayerData.Instance.user.localId ? true : false;

        gmDrawer.SetActive(isGM);
        settingsButton.SetActive(true);

        title.text = activeCampaign.title;
        gmSettings.SetActive(false);
        playerSettings.SetActive(false);
        invite.SetActive(false);
        blocked = false;

        LoadCharacters();
        StartCoroutine(DataRefresh());
    }

    public void Settings()
    {
        if (isGM)
        {
            if (!gmSettings.activeInHierarchy)
            {
                settingsButton.SetActive(false);
                gmSettings.SetActive(true);
            }
            else if (gmSettings.activeInHierarchy)
            {
                settingsButton.SetActive(true);
                gmSettings.SetActive(false);
            }
        }
        else
        {
            if (!playerSettings.activeInHierarchy)
            {
                settingsButton.SetActive(false);
                playerSettings.SetActive(true);
            }
            else if (playerSettings.activeInHierarchy)
            {
                settingsButton.SetActive(true);
                playerSettings.SetActive(false);
            }
        }
    }

    public void Block()
    {
        if(!blocked)
        {
            cardBlock.SetActive(true);
            settingsButton.GetComponent<Button>().interactable = false;
            foreach(GameObject card in cardButtons)
            {
                card.GetComponent<Button>().interactable = false;
            }
            blocked = true;
        }
        else
        {
            cardBlock.SetActive(false);
            settingsButton.GetComponent<Button>().interactable = true;
            foreach(GameObject card in cardButtons)
            {
                card.GetComponent<Button>().interactable = true;
            }
            blocked = false;
        }
    }

    public void Delete()
    {
        RestClient.Delete(AccountManager.Instance.uri + "/campaigns/" + activeCampaign.key + ".json?auth=" + AccountManager.Instance.idToken)
        .Then(response =>
        {
            //Deleting campaign key from each of the players who are part of it
            if(activeCampaign.playerIds != null)
            {
                foreach (string player in activeCampaign.playerIds)
            {
                RestClient.Get<User>(AccountManager.Instance.uri + "/users/" + player + ".json?auth=" + AccountManager.Instance.idToken)
                .Then(response2 =>
                {
                    response2.campaigns.Remove(activeCampaign.key);
                    RestClient.Put(AccountManager.Instance.uri + "/users/" + player + ".json?auth=" + AccountManager.Instance.idToken, response2);
                })
                .Catch(error2 =>
                {
                    Debug.Log("Error deleting campaign from players: " + error2);
                });
            }
            }

            PlayerData.Instance.user.campaigns.Remove(activeCampaign.key);
            RestClient.Put(AccountManager.Instance.uri + "/users/" + AccountManager.Instance.localId + ".json?auth=" + AccountManager.Instance.idToken, PlayerData.Instance.user)
                .Then(response3 =>
                {
                    Debug.Log("Deleted");
                    UIManager.Instance.LoadScreen(2);
                })
                .Catch(error =>
                {
                    Debug.Log(error);
                });
            
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

                foreach (var user in users.Values)
                {
                    if (user.email == inviteField.text)
                    {
                        playerFound = true;


                        User tempUser = user;

                        if (tempUser.localId != PlayerData.Instance.user.localId)
                        {
                            if (tempUser.invites == null)
                                tempUser.invites = new List<string>();

                            if (!tempUser.invites.Contains(activeCampaign.key))
                            {
                                Invite newInvite = new Invite()
                                {
                                    key = activeCampaign.key,
                                    title = activeCampaign.title,
                                    gm = PlayerData.Instance.user.username
                                };

                                if (tempUser.invites == null)
                                    tempUser.invites = new List<string>();

                                tempUser.invites.Add(activeCampaign.key);

                                RestClient.Put(AccountManager.Instance.uri + "/users/" + tempUser.localId + ".json?auth=" + AccountManager.Instance.idToken, tempUser)
                                .Then(r =>
                                {
                                    UIManager.Instance.Warning(tempUser.username + " has been invited to join " + activeCampaign.title + "!");
                                    invite.SetActive(false);
                                    inviteField.text = "";
                                })
                                .Catch(error =>
                                 {
                                     UIManager.Instance.Warning("Something went wrong, invitation not sent");
                                     invite.SetActive(false);
                                     inviteField.text = "";
                                 });
                            }
                            else
                            {
                                UIManager.Instance.Warning(tempUser + " has already been invited to join this campaign");
                                invite.SetActive(false);
                                inviteField.text = "";
                            }
                        }
                        else
                        {
                            UIManager.Instance.Warning("Cannot send invitations to yourself");
                            invite.SetActive(false);
                            inviteField.text = "";
                        }
                    }
                }

                if (!playerFound)
                    UIManager.Instance.Warning("No player found with that email address");
            })
            .Catch(error =>
            {
                Debug.Log("Couldn't get player list: " + error);
            });


        }
        else
        {
            Debug.Log("No email entered");
        }
    }

    void LoadCharacters()
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

        if (activeCampaign.characters == null)
            activeCampaign.characters = new List<Character>();

        hasChar = false;

        foreach (Character c in activeCampaign.characters)
        {
            if (c.id == PlayerData.Instance.user.localId)
                hasChar = true;
            PlayerData.Instance.activeCharacter = c;
        }

        if (activeCampaign.characters.Count == 0 && !isGM && !hasChar)
        {
            Instantiate(newCharacter, zones[0].transform);
        }
        else
        {
            for (int i = 0; i < activeCampaign.characters.Count; i++)
            {
                GameObject card = Instantiate(characterCard, zones[i].transform);
                CharacterCard character = card.GetComponent<CharacterCard>();

                character.characterData = activeCampaign.characters[i];
                character.player = activeCampaign.playerNames[activeCampaign.playerIds.IndexOf(activeCampaign.characters[i].id)];

                character.SetData();
            }

            if (!isGM && !hasChar)
            {
                Instantiate(newCharacter, zones[activeCampaign.characters.Count].transform);
            }
        }
    }

    public void DrawItem()
    {
        int get;
        Item tempCard = new Item();
        GameObject cardObj;
        ItemCard item;

        get = Random.Range(0, CardManager.Instance.itemCards.Count);
        tempCard = CardManager.Instance.itemCards[get];
        CardManager.Instance.itemCards.RemoveAt(get);

        Block();

        cardObj = Instantiate(itemCard, transform);
        item = cardObj.GetComponent<ItemCard>();

        item.itemData = tempCard;
        item.SetData();
    }

    public void DrawCard(string cardType)
    {
        int get;
        Card tempCard = new Card();
        GameObject cardObj;
        GenericCard genCard;

        switch (cardType)
        {
            case "hook":
                get = Random.Range(0, CardManager.Instance.hookCards.Count);
                tempCard = CardManager.Instance.hookCards[get];
                CardManager.Instance.hookCards.RemoveAt(get);
                break;

            case "event":
                get = Random.Range(0, CardManager.Instance.eventCards.Count);
                tempCard = CardManager.Instance.eventCards[get];
                break;

            case "npc":
                get = Random.Range(0, CardManager.Instance.npcCards.Count);
                tempCard = CardManager.Instance.npcCards[get];
                break;

        }

        Block();

        cardObj = Instantiate(genericCard, transform);
        genCard = cardObj.GetComponent<GenericCard>();

        genCard.cardData = tempCard;
        genCard.SetData();
    }

    IEnumerator DataRefresh()
    {
        if (!gameObject.activeInHierarchy)
        {
            yield break;
        }
        else
        {
            inGame = true;
        }

        while (inGame)
        {
            yield return new WaitForSeconds(10f);

            RestClient.Get<Campaign>(AccountManager.Instance.uri + "/campaigns/" + activeCampaign.key + ".json?auth=" + AccountManager.Instance.idToken)
            .Then(response =>
            {
                if (response == activeCampaign)
                {
                    Debug.Log("No changes to game data");
                }
                else
                {
                    Debug.Log("Updated game data");
                    activeCampaign = response;
                    LoadCharacters();
                }
            })
            .Catch(error =>
            {
                Debug.Log("Error updating game data: " + error);
                UIManager.Instance.Warning("Error updating game data: " + error);
            });
        }
    }

}
