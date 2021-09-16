using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RuleTab : MonoBehaviour
{
    public Rule rule;

    public TMP_Text title;

    public void SetData()
    {
        title.text = rule.title;
    }

    public void ShowRule()
    {
        Rulebook.Instance.title.text = rule.title;
        Rulebook.Instance.copy.text = rule.copy;
    }
}
