using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{

    //Field = 0 = no penalty
    //Tree = 1 = Impassable, can shoot through.
    //Wall = 2 = Impassable, cannot shoot through.
    public int tileType;
    public bool isPassable, isHazardous;
    public int mvtPenalty;

    public GameObject overlayItem, indicatorOL;  

    public SpriteRenderer sr, olsr, iolsr;
    public Sprite[] spriteList;
    public Sprite[] olSpriteList;
    public Sprite[] indicOLSpriteList;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (tileType)
        {
            case 0:
            case 1:
            case 3:
            case 19:
                isPassable = true;
                overlayItem.gameObject.SetActive(false);
                sr.sprite = spriteList[tileType];
                break;

            case 4:
            case 5:
            case 6:
            case 7:
            case 8:
            case 9:
            case 10:
            case 11:
            case 12:
            case 13:
            case 14:
            case 15:
            case 16:
            case 17:
            case 18:
                isPassable = false;
                overlayItem.gameObject.SetActive(false);
                sr.sprite = spriteList[tileType];
                break;

            case 2:
                isPassable = false;
                overlayItem.gameObject.SetActive(true);
                break;
        }
    }
}
