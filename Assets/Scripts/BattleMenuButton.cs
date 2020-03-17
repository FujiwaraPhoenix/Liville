using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleMenuButton : MonoBehaviour
{
    public Sprite longDefault, longHL, iconDef, iconHL;
    public Image myImg, iconImg;
    public int menuVal;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateSprite()
    {
        if (Controller.c.mp.currentMenuChoice == menuVal)
        {
            myImg.sprite = longHL;
            iconImg.sprite = iconHL;
        }
        else
        {
            myImg.sprite = longDefault;
            iconImg.sprite = iconDef;
        }
    }
}
