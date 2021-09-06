using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invites : MonoBehaviour
{

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

        if (PlayerData.Instance.inviteKeys.Count == 0)
        {
            Debug.Log("No Invites in inbox");
        }
        else
        {
            for (int i = 0; i < PlayerData.Instance.inviteKeys.Count; i++)
            {
                GameObject card = Instantiate(InviteCard, zones[i].transform);
                Invitation invitation = card.GetComponent<Invitation>();

                invitation.entryKey = PlayerData.Instance.inviteEntryKeys[i];
                invitation.key = PlayerData.Instance.inviteKeys[i];
                invitation.title = PlayerData.Instance.inviteTitles[i];
                invitation.gameMaster = PlayerData.Instance.inviteGms[i];

                invitation.titleText.text = invitation.title;
                invitation.gameMasterText.text = invitation.gameMaster + " has invited you to join";
            }

            Debug.Log("Invite cards loaded");
        }

    }
}
