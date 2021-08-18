using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewCampaign : MonoBehaviour
{
    FirebaseManager fm;
    UIManager uIManager;

    void Awake() 
    {
        fm = GameObject.Find("FirebaseManager").GetComponent<FirebaseManager>();
        uIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
    }

    public void New()
    {
        fm.CreateCampaign();
        fm.Load();

    }
}
