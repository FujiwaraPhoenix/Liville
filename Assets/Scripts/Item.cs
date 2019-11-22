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
    public int[,] mods = new int[3,2];
    public bool isMelee = false;

    //Is this a healing item?
    public int healAmt;

    //What is this thing CALLED?
    public string itemName;

    //Does this thing have stat modifiers? If so, how big/small are they?
    public int tempSpd, tempDef, tempEva, tempLck, tempRes, tempMinDmg, tempMaxDmg;

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
        minDmg = Random.Range(2, 5);
        maxDmg = Random.Range(minDmg, minDmg + 5);
        //Maintain clip size for now
        accuracy = Random.Range(75, 100);
        range = Random.Range(2, 4);
        //Mods are set up in a 2d array, 3x2. First number is the mod rarity, second is the mod ID of that given rarity.
        mods[0,0] = Random.Range(1, 4);
        switch (mods[0, 0])
        {
            case 1:
                mods[0, 1] = Random.Range(1, 9);
                switch (mods[0, 1])
                {
                    case 1:
                        //Fleetfoot: Spd+
                        tempSpd = 5;
                        break;
                    case 2:
                        //Ironclad: Def+
                        tempDef = 5;
                        break;
                    case 3:
                        //Aware: Eva+
                        tempEva = 5;
                        break;
                    case 4:
                        //Lucky: Lck+
                        tempLck = 5;
                        break;
                    case 5:
                        //Resistant: Res+
                        tempRes = 10;
                        break;
                    case 8:
                        //Ammo Capacity+: 1.5x Cap
                        clipSize = (int) (clipSize * 1.5f);
                        currentClip = clipSize;
                        break;
                }
                break;
            case 2:
                mods[0, 1] = Random.Range(1, 8);
                switch (mods[0, 1])
                {
                    case 1:
                        //Brutal: Dmg+
                        tempMinDmg = 5;
                        tempMaxDmg = 5;
                        break;
                    case 2:
                        //Scope: Range+
                        range += 1;
                        break;
                    case 6:
                        //Ammo Capacity++: 2x Cap
                        clipSize = (int)(clipSize * 2f);
                        currentClip = clipSize;
                        break;
                    case 7:
                        //Ammo Capacity-: .75x Cap
                        clipSize = (int)(clipSize * .75f);
                        currentClip = clipSize;
                        break;
                }
                break;
            case 3:
                mods[0, 1] = Random.Range(1, 12);
                switch (mods[0, 1])
                {
                    case 4:
                        //Gentle: Dmg-
                        tempMinDmg = -2;
                        tempMaxDmg = -2;
                        break;
                    case 5:
                        //Flatfoot: Spd-
                        tempSpd = -2;
                        break;
                    case 6:
                        //Paperclad: Def-
                        tempDef = -2;
                        break;
                    case 7:
                        //Unaware: Eva-
                        tempEva = -2;
                        break;
                    case 8:
                        //Unlucky: Lck-
                        tempLck = -2;
                        break;
                    case 9:
                        //Uncalibrated: Rng-
                        range += -1;
                        break;
                    case 10:
                        //Ammo Capacity--: .5x Cap
                        clipSize = (int)(clipSize * .5f);
                        currentClip = clipSize;
                        break;
                }
                break;
        }
        if (Random.Range(0,100) < 5)
        {
            mods[1, 0] = Random.Range(1, 4);
            switch (mods[1, 0])
            {
                case 1:
                    mods[1, 1] = Random.Range(1, 9);
                    switch (mods[1, 1])
                    {
                        case 1:
                            //Fleetfoot: Spd+
                            tempSpd = 5;
                            break;
                        case 2:
                            //Ironclad: Def+
                            tempDef = 5;
                            break;
                        case 3:
                            //Aware: Eva+
                            tempEva = 5;
                            break;
                        case 4:
                            //Lucky: Lck+
                            tempLck = 5;
                            break;
                        case 5:
                            //Resistant: Res+
                            tempRes = 10;
                            break;
                        case 8:
                            //Ammo Capacity+: 1.5x Cap
                            clipSize = (int)(clipSize * 1.5f);
                            currentClip = clipSize;
                            break;
                    }
                    break;
                case 2:
                    mods[1, 1] = Random.Range(1, 8);
                    switch (mods[1, 1])
                    {
                        case 1:
                            //Brutal: Dmg+
                            tempMinDmg = 5;
                            tempMaxDmg = 5;
                            break;
                        case 2:
                            //Scope: Range+
                            range += 1;
                            break;
                        case 6:
                            //Ammo Capacity++: 2x Cap
                            clipSize = (int)(clipSize * 2f);
                            currentClip = clipSize;
                            break;
                        case 7:
                            //Ammo Capacity-: .75x Cap
                            clipSize = (int)(clipSize * .75f);
                            currentClip = clipSize;
                            break;
                    }
                    break;
                case 3:
                    mods[1, 1] = Random.Range(1, 12);
                    switch (mods[1, 1])
                    {
                        case 4:
                            //Gentle: Dmg-
                            tempMinDmg = -2;
                            tempMaxDmg = -2;
                            break;
                        case 5:
                            //Flatfoot: Spd-
                            tempSpd = -2;
                            break;
                        case 6:
                            //Paperclad: Def-
                            tempDef = -2;
                            break;
                        case 7:
                            //Unaware: Eva-
                            tempEva = -2;
                            break;
                        case 8:
                            //Unlucky: Lck-
                            tempLck = -2;
                            break;
                        case 9:
                            //Uncalibrated: Rng-
                            range += -1;
                            break;
                        case 10:
                            //Ammo Capacity--: .5x Cap
                            clipSize = (int)(clipSize * .5f);
                            currentClip = clipSize;
                            break;
                    }
                    break;
            }
        }
    }
}
