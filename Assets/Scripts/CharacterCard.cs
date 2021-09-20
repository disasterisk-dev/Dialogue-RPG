using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Proyecto26;

public class CharacterCard : MonoBehaviour
{
    public Character characterData;
    public string player;

    public float wordT, witT, willT, wantT;

    [Header("UI Elements")]
    public TMP_Text charName;
    public TMP_Text playerName;
    public TMP_Text level;
    public TMP_Text word;
    public TMP_Text wit;
    public TMP_Text will;
    public TMP_Text want;
    public Image weapon;
    public Image clothing;
    public Image relic;
    public Button[] buttons;
    public Image[] points;

    private void OnEnable()
    {
    }

    // Start is called before the first frame update
    public void SetData()
    {
        wordT = characterData.word + ModSum(Item.statType.word);
        witT = characterData.wit + ModSum(Item.statType.wit);
        willT = characterData.will + ModSum(Item.statType.will);
        wantT = characterData.want + ModSum(Item.statType.want);

        charName.text = characterData.name;
        playerName.text = player;
        level.text = characterData.level.ToString();
        word.text = wordT.ToString();
        wit.text = witT.ToString();
        will.text = willT.ToString();
        want.text = wantT.ToString();

        weapon.sprite = characterData.weapon.sprite;
        clothing.sprite = characterData.clothing.sprite;
        relic.sprite = characterData.relic.sprite;

        SetPoints();

        if (characterData.id == PlayerData.Instance.user.localId || ActiveCampaign.Instance.activeCampaign.gmid == PlayerData.Instance.user.localId)
        {
            foreach (Button b in buttons)
            {
                b.interactable = true;
            }
        }
        else
        {
            foreach (Button b in buttons)
            {
                b.interactable = false;
            }
        }


    }

    public void SetPoints()
    {
        for (int i = 0; i < characterData.points; i++)
        {
            points[i].color = new Color32(248, 48, 48, 255);
        }

        if (characterData.points == 0)
        {
            foreach (Image i in points)
            {
                i.color = new Color32(255, 255, 255, 255);
            }
        }

        if(characterData.points == 5)
            ActiveCampaign.Instance.playerLevelling = true;
    }

    public float ModSum(Item.statType _type)
    {
        float sum = 0;

        if (characterData.weapon.stat == _type)
            sum += characterData.weapon.bonus;

        if (characterData.clothing.stat == _type)
            sum += characterData.clothing.bonus;

        if (characterData.relic.stat == _type)
            sum += characterData.relic.bonus;

        return sum;
    }

    public void Roll(string stat)
    {
        if (!ActiveCampaign.Instance.playerLevelling)
        {
            float roll = Random.Range(1, 6);

            switch (stat)
            {
                case "word":
                    UIManager.Instance.Roll("Word", roll, wordT);
                    break;
                case "wit":
                    UIManager.Instance.Roll("Wit", roll, witT);
                    break;
                case "will":
                    UIManager.Instance.Roll("Will", roll, willT);
                    break;
                case "want":
                    UIManager.Instance.Roll("Want", roll, wantT);
                    break;
            }
        }
        else
        {
            switch (stat)
            {
                case "word":
                    UIManager.Instance.LevelUp("Word", characterData.word, this);
                    break;
                case "wit":
                    UIManager.Instance.LevelUp("Wit", characterData.word, this);
                    break;
                case "will":
                    UIManager.Instance.LevelUp("Will", characterData.word, this);
                    break;
                case "want":
                    UIManager.Instance.LevelUp("Want", characterData.word, this);
                    break;
            }
        }
    }

    public void AddPoint()
    {
        if (ActiveCampaign.Instance.pointGive)
        {
            if (characterData.points < 5)
            {
                characterData.points++;
                SetPoints();


                RestClient.Put(AccountManager.Instance.uri + "/campaigns/" + ActiveCampaign.Instance.activeCampaign.key + ".json?auth=" + AccountManager.Instance.idToken, ActiveCampaign.Instance.activeCampaign)
                .Then(response =>
                {
                    Debug.Log("Updated player's points");
                    ActiveCampaign.Instance.pointGive = false;
                });
            }
            else
            {
                UIManager.Instance.Warning(characterData.name + " must level up before more points can be awarded");
            }
        }
    }

    public void ConcludeLevelUp()
    {
        characterData.level++;
        characterData.points = 0;

        RestClient.Put(AccountManager.Instance.uri + "/campaigns/" + ActiveCampaign.Instance.activeCampaign.key + ".json?auth=" + AccountManager.Instance.idToken, ActiveCampaign.Instance.activeCampaign)
            .Then(response =>
            {
                Debug.Log("Updated player level");
                SetData();
                ActiveCampaign.Instance.playerLevelling = false;
                ActiveCampaign.Instance.pointGive = false;
            });
    }

    public void NewItem(string _type)
    {
        Item item = ActiveCampaign.Instance.giveItem;

        if (ActiveCampaign.Instance.giving &&
            PlayerData.Instance.user.localId == ActiveCampaign.Instance.activeCampaign.gmid &&
            item.type.ToString() == _type)
        {
            switch (_type)
            {
                case "weapon":
                    characterData.weapon = item;
                    break;
                case "clothing":
                    characterData.clothing = item;
                    break;
                case "relic":
                    characterData.relic = item;
                    break;
            }

            SetData();

            RestClient.Put(AccountManager.Instance.uri + "/campaigns/" + ActiveCampaign.Instance.activeCampaign.key + ".json?auth=" + AccountManager.Instance.idToken, ActiveCampaign.Instance.activeCampaign)
            .Then(response =>
            {
                Debug.Log("Updated player's items");
                ActiveCampaign.Instance.giving = false;
            });

        }
        else if (!ActiveCampaign.Instance.giving)
        {
            switch (_type)
            {
                case "weapon":
                    item = characterData.weapon;
                    break;
                case "clothing":
                    item = characterData.clothing;
                    break;
                case "relic":
                    item = characterData.relic;
                    break;
            }

            GameObject cardObj;
            ItemCard itemCard;

            ActiveCampaign.Instance.Block();

            cardObj = Instantiate(ActiveCampaign.Instance.itemCard, ActiveCampaign.Instance.transform);
            itemCard = cardObj.GetComponent<ItemCard>();

            itemCard.itemData = item;
            itemCard.SetData();
        }
    }
}
