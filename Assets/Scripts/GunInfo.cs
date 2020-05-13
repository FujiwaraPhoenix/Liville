using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunInfo : MonoBehaviour
{
    public Text displayName, dNameDropShadow, gunStats, gunMods;
    public Image border;
    public Sprite[] pips = new Sprite[2];
    public Image[] ammoPips;
    public GameObject pipHolder;
    public Image basePip;

    //For base menu
    public int character;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (LoadoutUI.lUI.currentLoadoutMenu == 0)
        {
            updateStats(Controller.c.playerRoster[character].currEquip);
            updateMods(Controller.c.playerRoster[character].currEquip);
        }
    }

    public void updateStats (Item shownGun)
    {
        displayName.text = shownGun.itemName;
        dNameDropShadow.text = shownGun.itemName;
        string statString = "DMG: " + shownGun.minDmg + "-" + shownGun.maxDmg + "\nACC: " + shownGun.accuracy + "\nRNG: " + shownGun.range;
        gunStats.text = statString;
        initializePips(shownGun);
    }

    public void updateMods(Item shownGun)
    {
        string output = "";
        for (int i = 0; i < shownGun.mods.GetLength(0); i++)
        {
            output += "Mod " + (i + 1) + ": " + Controller.c.determineModName(shownGun.mods[i, 0], shownGun.mods[i, 1]) + "\n";
        }
        gunMods.text = output;
    }

    public void initializePips(Item selectedGun)
    {
        //Clear ammo pips
        if (ammoPips.Length != 0)
        {
            for (int i = 0; i < ammoPips.Length; i++)
            {
                Destroy(ammoPips[i].gameObject);
            }
        }
        ammoPips = new Image[selectedGun.clipSize];
        int isOdd = ammoPips.Length % 2;
        for (int i = 0; i < ammoPips.Length; i++)
        {
            Image temp = Instantiate(basePip, Vector3.zero, Quaternion.identity);
            ammoPips[i] = temp;
            temp.transform.SetParent(pipHolder.transform, false);
            //Make a new image for each, offset by as much as you need.
            if (isOdd == 0)
            {
                //Even; formula is (-10(pip array size/2) + 10n -5)
                temp.transform.localPosition = new Vector3((-10 * Mathf.Floor((ammoPips.Length -1 ) / 2)) + (10 * i) - 5, 0, 0);
            }
            else
            {
                temp.transform.localPosition = new Vector3((-10 * Mathf.Floor(ammoPips.Length / 2)) + (10 * i), 0, 0);
            }
        }
    }

    public void updateBorder(Item chosenGun)
    {
        border.sprite = LoadoutUI.lUI.gunEquippedBorders[chosenGun.rarity - 1];
    }

    public void updateBorderHL(Item chosenGun)
    {
        border.sprite = LoadoutUI.lUI.gunEquippedBordersHL[chosenGun.rarity - 1];
    }
}
