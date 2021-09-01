using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Background : MonoBehaviour
{
    public float speed;
    public List<Sprite> images;
    public Image background;
    // Start is called before the first frame update
    void Start()
    {
        background.sprite = images[Random.Range(0,images.Count)];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
