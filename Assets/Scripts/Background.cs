using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    public SpriteRenderer mySpr;
    public Sprite[] backgroundList = new Sprite[4];
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void changeBG(int newBGID)
    {
        mySpr.sprite = backgroundList[newBGID];
    }
}
