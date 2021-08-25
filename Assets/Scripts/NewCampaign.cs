using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class NewCampaign : MonoBehaviour
{
    public TMP_InputField nameInput;
    FirebaseManager fm;
    UIManager uIManager;
    PlayerData playerData;

    void Awake() 
    {
        fm = GameObject.Find("FirebaseManager").GetComponent<FirebaseManager>();
        uIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        playerData = GameObject.Find("PlayerDataManager").GetComponent<PlayerData>();
    }

    public void New()
    {
        uIManager.LoadScreen(3); //load genre selection screen
        //fm.CreateCampaign();
        //fm.Load();

    }

    public void Genre(string genre)
    {
        playerData.tempGenre = genre;
        uIManager.LoadScreen(4);
    }

    public void Create()
    {
        fm.CreateCampaign(nameInput.text, playerData.tempGenre);
        nameInput.text = "";
        fm.Load();
        uIManager.LoadScreen(2); //loads campaign screen
    }
}
