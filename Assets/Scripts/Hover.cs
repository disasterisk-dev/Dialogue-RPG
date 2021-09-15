using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DentedPixel;

public class Hover : MonoBehaviour
{
    public float moveDistance;
    public float time;
    // Start is called before the first frame update

    private void Awake() 
    {
        gameObject.GetComponent<Button>().onClick.AddListener(() => Reset());
    }

    private void OnMouseEnter()
    {
        if(gameObject.GetComponent<Button>().interactable)
            LeanTween.moveLocalY(this.gameObject, moveDistance, time);
    }

    private void OnMouseExit()
    {
        if(gameObject.GetComponent<Button>().interactable)
            LeanTween.moveLocalY(this.gameObject, 0f, time);
    }

    private void Reset() 
    {
        transform.LeanSetLocalPosY(0f);
    }
}
