using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invites : MonoBehaviour
{
    public PlayerData playerData;

    [Header("UICards")]
    public GameObject[] zones;

    [Header("Prefabs")]
    public GameObject InviteCard;
    // Start is called before the first frame update
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
    void Update()
    {
        
    }
    public void Refresh()
    {

        playerData = GameObject.Find("PlayerDataManager").GetComponent<PlayerData>();

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

        if (playerData.inviteKeys.Count == 0)
        {
            Debug.Log("No Invites in inbox");
        }
        else
        {
            for (int i = 0; i < playerData.inviteKeys.Count; i++)
            {
                GameObject card = Instantiate(InviteCard, zones[i].transform);
                Invitation invitation = card.GetComponent<Invitation>();

                invitation.entryKey = playerData.inviteEntryKeys[i];
                invitation.key = playerData.inviteKeys[i];
                invitation.title = playerData.inviteTitles[i];
                invitation.gameMaster = playerData.inviteGms[i];

                invitation.titleText.text = invitation.title;
                invitation.gameMasterText.text = invitation.gameMaster + " has invited you to join";
            }

            Debug.Log("Invite cards loaded");
        }

    }
}
