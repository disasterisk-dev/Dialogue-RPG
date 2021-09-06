using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Invitation : MonoBehaviour
{
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

        FirebaseManager.Instance.Accept(key, entryKey);

        Destroy(gameObject);
    }

    public void Decline()
    {
        FirebaseManager.Instance.Decline(entryKey);
        Destroy(gameObject);
    }
}
