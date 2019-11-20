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
    public int minDmg, maxDmg, clipSize, currentClip, accuracy, range;
    public int[] mods = new int[3];
    public bool isMelee = false;

    //Is this a healing item?
    public int healAmt;

    //What is this thing CALLED?
    public string itemName;

    //Temp; check if randomized.
    public bool randomized = false;

    void Start()
    {
        if (!randomized)
        {
            tempRandGun();
            randomized = true;
        }
    }

    public bool useItem(Unit user)
    {
        //Uses the item. If used, true. Else, false.
        if (itemID == 1)
        {
            if (user.hp < user.maxhp)
            {
                if (user.hp + healAmt < user.maxhp)
                {
                    Debug.Log("Healed " + healAmt + " HP!");
                    user.hp += healAmt;
                }
                else
                {
                    Debug.Log("Healed " + (user.maxhp - user.hp) + " HP!");
                    user.hp = user.maxhp;
                }
                return true;
            }
            else
            {
                //Well, you goofed. Return false.
                Debug.Log("HP full! No point in using this.");
                return false;
            }
        }
        return false;
    }

    public void tempRandGun()
    {
        minDmg = Random.Range(1, 5);
        maxDmg = Random.Range(minDmg, minDmg + 5);
        //Maintain clip size for now
        accuracy = Random.Range(75, 100);
        range = Random.Range(2, 4);
        //For starters, we want a gun mod. A SINGLE gun mod.
        mods[0] = Random.Range(1, 7);
    }
}
