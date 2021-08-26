using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Invitation : MonoBehaviour
{
    FirebaseManager firebaseManager;
    PlayerData playerData;
    public TMP_Text gameMasterText;
    public TMP_Text titleText;

    public string gameMaster;
    public string title;
    public string key;
    public string entryKey;

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
        firebaseManager = GameObject.Find("FirebaseManager").GetComponent<FirebaseManager>();

        firebaseManager.Accept(key, entryKey);

        Destroy(gameObject);
    }

    public void Decline()
    {
        firebaseManager = GameObject.Find("FirebaseManager").GetComponent<FirebaseManager>();
        playerData = GameObject.Find("PlayerDataManager").GetComponent<PlayerData>();

        firebaseManager.Decline(entryKey);
        Destroy(gameObject);
    }
}
