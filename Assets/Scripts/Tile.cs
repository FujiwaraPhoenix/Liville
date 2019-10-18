using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{

    //Field = no penalty
    //Forest = -1mvt
    //Mountain = impassable
    public enum tileOptions
    {
        Field,
        Forest,
        Mountain
    }

    public tileOptions tileType;
    public bool isPassable, isHazardous;
    public int mvtPenalty;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
