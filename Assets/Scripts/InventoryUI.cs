using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public int itemCountI, itemCountC, currentHL, currentMax, offset;
    public Text invListing, convoyListing, itemData;
    public Image background;
    public Sprite baseBG, litBG;
    public Image[] convoyIcons, invIcons;
    public Sprite basePotion, litPotion;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //For use with the full convoy, assuming the text goes out of border.
    public void updateText()
    {
        //Fits 15, by the way.
        string output = "";
        for (int i = 0; i < 15; i++)
        {
            if (i < itemCountC)
            {
                convoyIcons[i].transform.gameObject.SetActive(true);
            }
            else
            {
                convoyIcons[i].transform.gameObject.SetActive(false);
            }
        }
        if (currentMax < 15)
        {
            for (int i = 0; i < currentMax + 1; i++)
            {
                output += InvManager.im.convoy[i].itemName + "\t\t\t\t\t\t\t\t+" + InvManager.im.convoy[i].healAmt + " HP\n";
            }
        }
        else
        {
            for (int i = 0; i < 15; i++)
            {
                output += InvManager.im.convoy[i + offset].itemName + "\t\t\t\t\t\t\t\t+" + InvManager.im.convoy[i + offset].healAmt + " HP\n";
            }
        }
        convoyListing.text = output;
    }

    public void updatePlayerInvText(int playerID)
    {
        for (int i = 0; i < 5; i++)
        {
            if (i < itemCountI)
            {
                invIcons[i].transform.gameObject.SetActive(true);
            }
            else
            {
                invIcons[i].transform.gameObject.SetActive(false);
            }
        }
        string output = "";
        if (itemCountI > 0)
        {
            for (int i = 0; i < itemCountI; i++)
            {
                output += Controller.c.playerUnits[playerID].inventory[i].itemName + "\t\t\t+" + Controller.c.playerUnits[playerID].inventory[i].healAmt + " HP\n";
            }
        }
        invListing.text = output;
    }

    public void updateCountTotals(int playerID)
    {
        itemCountI = Controller.c.playerUnits[playerID].inventory.Count;
        itemCountC = InvManager.im.convoy.Count;
    }

    public void updateData(Item chosenItem)
    {
        if (chosenItem == null)
        {
            itemData.text = "";
        }
        else if (chosenItem.itemID == 1)
        {
            itemData.text = "A simple potion, meant to recover basic wounds. Heals " + chosenItem.healAmt + " HP upon use. Can only be used once.";
        }
    }
}
