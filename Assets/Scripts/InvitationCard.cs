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

        Destroy(gameObject);
    }

    public void Decline()
    {
        Destroy(gameObject);
    }
}
