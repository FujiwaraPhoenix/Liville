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
    public int rarity;

    //Is this a healing item?
    public int healAmt;

    //What is this thing CALLED?
    public string itemName;
    //What kind of thing is it?
    public string itemType;

    //Does this thing have stat modifiers? If so, how big/small are they?
    public int tempAcc, tempDef, tempEva, tempLck, tempRes, tempMinDmg, tempMaxDmg, tempMvt;

    void Start()
    {

    }

    public bool useItem(Unit user)
    {
        //Uses the item. If used, true. Else, false.
        if (itemID == 1)
        {
            int tempHeal = healAmt;
            //Check fire.
            if (user.negStatus[1] > 0)
            {
                healAmt = healAmt / 2;
            }
            if (user.hp < user.maxhp)
            {
                if (user.hp + tempHeal < user.maxhp)
                {
                    Debug.Log("Healed " + tempHeal + " HP!");
                    user.hp += tempHeal;
                }
                else
                {
                    Debug.Log("Healed " + (user.maxhp - user.hp) + " HP!");
                    user.hp = user.maxhp;
                }
                BattleMenuUI.bmui.updatePlayerValues(user);
                BattleMenuUI.bmui.updatePlayerDisplay();
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
}
