using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Rulebook : MonoBehaviour
{
    public static Rulebook instance;
    public static Rulebook Instance { get { return instance; } }
    public List<Rule> rules;

    [Header("UI Elements")]
    public GameObject content;
    public GameObject rulePrefab;
    public Scrollbar scrollbar;
    public TMP_Text title;
    public TMP_Text copy;

    private void OnEnable() 
    {
        scrollbar.value = 1;
        title.text = "";
        copy.text = "";
    }

    private void Awake() 
    {
        instance = this;
    }

    private void Start() 
    {
        foreach(Rule r in rules)
        {
            GameObject newRule = Instantiate(rulePrefab, content.transform);
            RuleTab ruleTab = newRule.GetComponent<RuleTab>();

            ruleTab.rule = r;
            ruleTab.SetData();
        }
    }
}
