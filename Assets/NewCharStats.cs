using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NewCharStats : MonoBehaviour
{

    public TMP_Text title;
    public int points = 5;
    public int[] stats;
    public TMP_Text[] values;
    public bool direction = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetDirection(bool d)
    {
        direction = d;
    }

    public void UpdateStats(int stat)
    {
        if (points >= 1)
        {
            if (direction) //adding towards positive stat
            {
                if(stats[stat] < 0)
                {
                    points += 1;
                }
                else
                {
                    points -= 1;
                }

                stats[stat] += 1;
                
            }
            if (!direction)
            {
                if(stats[stat] > 0)
                {
                    points += 1;
                }
                else
                {
                    points -= 1;
                }
                stats[stat] -= 1;
            }
        }
        else if(points == 0)
        {
            if(direction && stats[stat] < 0)
            {
                stats[stat]+=1;
                points+=1;
            }
            else if(!direction && stats[stat] > 0)
            {
                stats[stat]-=1;
                points+=1;
            }
        }

        title.text = "Choose their stats... you have " + points + " points";
        values[stat].text = stats[stat].ToString();
    }
}
