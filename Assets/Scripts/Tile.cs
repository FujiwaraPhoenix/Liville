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
                isPassable = true;
                overlayItem.gameObject.SetActive(false);
                sr.sprite = spriteList[0];
                break;

            case 1:
                isPassable = false;
                overlayItem.gameObject.SetActive(false);
                sr.sprite = spriteList[1];
                break;

            case 2:
                isPassable = false;
                overlayItem.gameObject.SetActive(true);
                break;
        }
    }
}
