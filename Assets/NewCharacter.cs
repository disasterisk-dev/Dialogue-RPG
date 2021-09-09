using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NewCharacter : MonoBehaviour
{
    public TMP_InputField nameInput;
    public NewCharStats statsRef;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void New()
    {
        PlayerData.Instance.tempChar.id = PlayerData.Instance.user.localId;
        UIManager.Instance.LoadScreen(7);
    }

    public void AddName()
    {
        PlayerData.Instance.tempChar.name = nameInput.text;
        UIManager.Instance.LoadScreen(8);
    }

    

    public void AddStats()
    {
        PlayerData.Instance.tempChar.word = statsRef.stats[0];
        PlayerData.Instance.tempChar.wit = statsRef.stats[1];
        PlayerData.Instance.tempChar.will = statsRef.stats[2];
        PlayerData.Instance.tempChar.want = statsRef.stats[3];

        UIManager.Instance.LoadScreen(9);
    }
}
