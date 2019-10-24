using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{

    //Item stats
    //What kind of item is it?
    //0: gun, 1: healing item
    public int itemID = 0;

    //Is this a weapon?
    public int dmg, clipSize, currentClip, accuracy, range;
    public int[] mods = new int[3];
    public bool isMelee = false;

    //Is this a healing item?
    public int healAmt;

    //What is this thing CALLED?
    public string itemName;
}
