using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleMenuUI : MonoBehaviour
{
    public static BattleMenuUI bmui;

    public Image menuPointer;
    public GameObject menuItems;
    public GameObject enemyDisplay, playerDisplay;

    public Sprite pipOn, pipOff;
    public Sprite ammoPipOn, ammoPipOff;

    //Player Stats
    public Unit currentPlayer, currentEnemy;
    public bool foundPlayer = false;
    public Text pUnitName, pGunName;
    public Image pImg;
    public Text HPDisplay, statDisplay, gunStatDisplay, modDisplay;
    public int pHP, pMaxHP, pDef, pEva, pSpd, pLck;
    public int pClipSize, pCurrClip, pMinDmg, pMaxDmg, pAcc, pRng;
    public int pTempSpd, pTempDef, pTempEva, pTempLck, pTempRng, pTempRes, pTempMinDmg, pTempMaxDmg;
    public int[,] pMods = new int[3,2];
    public string pMod1Name, pMod2Name, pMod3Name;
    public bool pIsMelee = false;
    public Image[] pHPPips = new Image[40];
    public Image[] pAmmoPips = new Image[3];

    //Enemy Stats
    public bool foundEnemy = false;
    public Text eUnitName, eGunName;
    public Image eImg;
    public Text eHPDisplay, estatDisplay, egunStatDisplay, emodDisplay;
    public int eHP, eMaxHP, eDef, eEva, eSpd, eLck;
    public int eClipSize, eCurrClip, eMinDmg, eMaxDmg, eAcc, eRng;
    public int eTempSpd, eTempDef, eTempEva, eTempLck, eTempRng, eTempRes, eTempMinDmg, eTempMaxDmg;
    public int[,] eMods = new int[3, 2];
    public string eMod1Name, eMod2Name, eMod3Name;
    public bool eIsMelee = false;
    public Image[] eHPPips = new Image[40];
    public Image[] eAmmoPips = new Image[3];

    void Awake()
    {
        if (bmui == null)
        {
            DontDestroyOnLoad(gameObject);
            bmui = this;
        }
        else if (bmui != this)
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Controller.c.gameMode == 4)
        {
            updateMenuPosition();
            updatePlayerDisplay();
            updateEnemyDisplay();
        }
    }

    public void updateMenuPosition()
    {
        if (!Controller.c.mp.menuActive)
        {
            menuPointer.gameObject.SetActive(false);
        }
        else
        {
            menuPointer.gameObject.SetActive(true);
            switch (Controller.c.mp.currentMenuChoice)
            {
                case 0:
                    menuPointer.transform.localPosition = new Vector3(-475, -120, 0);
                    break;
                case 1:
                    menuPointer.transform.localPosition = new Vector3(-475, -150f, 0);
                    break;
                case 2:
                    menuPointer.transform.localPosition = new Vector3(-475, -180f, 0);
                    break;
                case 3:
                    menuPointer.transform.localPosition = new Vector3(-475, -210f, 0);
                    break;
            }
        }
    }

    public void updatePlayerDisplay()
    {
        if (!foundPlayer)
        {
            playerDisplay.SetActive(false);
        }
        else
        {
            playerDisplay.SetActive(true);
        }
        //Do you have a player unit selected? If so, display their data.
        if (Controller.c.mp.targetUnit != null)
        {
            updatePlayerValues(Controller.c.mp.targetUnit);
            foundPlayer = true;
        }
        //Check if you're hovering over a player unit:
        else {
            foreach (Unit u in Controller.c.playerUnits)
            {
                if ((u.position[0] == Controller.c.mp.currX) && u.position[1] == Controller.c.mp.currY)
                {
                    //If you are, update the data.
                    currentPlayer = u;
                    updatePlayerValues(currentPlayer);
                    foundPlayer = true;
                }
            }
        }
        //Generate display text.
        //HP is usually left unmodified, so leave this as is.
        HPDisplay.text = "HP: " + pHP + "/" + pMaxHP;
        //Stats are returned in the corresponding fxn below.
        statDisplay.text = genPlayerStatDisplay();
        gunStatDisplay.text = "DMG: " + pMinDmg + "-" + pMaxDmg + "\nACC: " + pAcc + "\nRNG: " + pRng;
        modDisplay.text = "Mod 1: " + pMod1Name + "\nMod 2: " + pMod2Name + "\nMod 3: " + pMod3Name;

        for (int i = 0; i < 40; i++)
        {
            if (i < pHP)
            {
                pHPPips[i].gameObject.SetActive(true);
                pHPPips[i].sprite = pipOn;
            }
            else if (i < pMaxHP)
            {
                pHPPips[i].gameObject.SetActive(true);
                pHPPips[i].sprite = pipOff;
            }
            else
            {
                pHPPips[i].gameObject.SetActive(false);
            }
        }
        if (!pIsMelee)
        {
            for (int i = 0; i < pAmmoPips.Length; i++)
            {
                if (i < pCurrClip)
                {
                    pAmmoPips[i].gameObject.SetActive(true);
                    pAmmoPips[i].sprite = ammoPipOn;
                }
                else
                {
                    pAmmoPips[i].gameObject.SetActive(true);
                    pAmmoPips[i].sprite = ammoPipOff;
                }
            }
        }
    }


    public void updateEnemyDisplay()
    {
        if (!foundEnemy)
        {
            enemyDisplay.SetActive(false);
        }
        else
        {
            enemyDisplay.SetActive(true);
        }

        //Check if you're hovering over an enemy unit:
        foreach (Unit u in Controller.c.enemyUnits)
        {
            if ((u.position[0] == Controller.c.mp.currX) && u.position[1] == Controller.c.mp.currY)
            {
                //If you are, update the data.
                currentEnemy = u;
                updateEnemyValues(currentEnemy);
                foundEnemy = true;
            }
        }
        //Generate display text.
        //HP is usually left unmodified, so leave this as is.
        eHPDisplay.text = "HP: " + eHP + "/" + eMaxHP;
        //Stats are returned in the corresponding fxn below.
        estatDisplay.text = genEnemyStatDisplay();
        egunStatDisplay.text = "DMG: " + eMinDmg + "-" + eMaxDmg + "\nACC: " + eAcc + "\nRNG: " + eRng;
        emodDisplay.text = "Mod 1: " + eMod1Name + "\nMod 2: " + eMod2Name + "\nMod 3: " + eMod3Name;
        for (int i = 0; i < 40; i++)
        {
            if (i < eHP)
            {
                eHPPips[i].gameObject.SetActive(true);
                eHPPips[i].sprite = pipOn;
            }
            else if (i < eMaxHP)
            {
                eHPPips[i].gameObject.SetActive(true);
                eHPPips[i].sprite = pipOff;
            }
            else
            {
                eHPPips[i].gameObject.SetActive(false);
            }
        }
        if (!eIsMelee)
        {
            for (int i = 0; i < eAmmoPips.Length; i++)
            {
                if (i <= eCurrClip)
                {
                    eAmmoPips[i].gameObject.SetActive(true);
                    eAmmoPips[i].sprite = ammoPipOn;
                }
                else
                {
                    eAmmoPips[i].gameObject.SetActive(true);
                    eAmmoPips[i].sprite = ammoPipOff;
                }
            }
        }
    }

    public void updatePlayerValues(Unit playerUnit)
    {
        //Name and face
        pUnitName.text = playerUnit.unitName;
        pImg.sprite = playerUnit.unitFace;

        //Player stats
        pHP = playerUnit.hp;
        pMaxHP = playerUnit.maxhp;
        pSpd = playerUnit.spd;
        pEva = playerUnit.eva;
        pDef = playerUnit.def;
        pLck = playerUnit.lck;

        //Gun stats
        pGunName.text = playerUnit.currEquip.itemName;
        pMinDmg = playerUnit.currEquip.minDmg;
        pMaxDmg = playerUnit.currEquip.maxDmg;
        pClipSize = playerUnit.currEquip.clipSize;
        pCurrClip = playerUnit.currEquip.currentClip;
        pAcc = playerUnit.currEquip.accuracy;
        pRng = playerUnit.currEquip.range;
        pIsMelee = playerUnit.currEquip.isMelee;

        //Gun mod values
        pTempSpd = playerUnit.currEquip.tempSpd;
        pTempDef = playerUnit.currEquip.tempDef;
        pTempEva = playerUnit.currEquip.tempEva;
        pTempLck = playerUnit.currEquip.tempLck;
        pTempRes = playerUnit.currEquip.tempRes;
        pTempMinDmg = playerUnit.currEquip.tempMinDmg;
        pTempMaxDmg = playerUnit.currEquip.tempMaxDmg;

        //Mods
        pMods = playerUnit.currEquip.mods;
        pMod1Name = determineModName(pMods[0,0], pMods[0,1]);
        pMod2Name = determineModName(pMods[1, 0], pMods[1, 1]);
        pMod3Name = determineModName(pMods[2, 0], pMods[2, 1]);
    }

    public void updateEnemyValues(Unit enemyUnit)
    {
        //Name and face
        eUnitName.text = enemyUnit.unitName;
        eImg.sprite = enemyUnit.unitFace;

        //Player stats
        eHP = enemyUnit.hp;
        eMaxHP = enemyUnit.maxhp;
        eSpd = enemyUnit.spd;
        eEva = enemyUnit.eva;
        eDef = enemyUnit.def;
        eLck = enemyUnit.lck;

        //Gun stats
        eGunName.text = enemyUnit.currEquip.itemName;
        eMinDmg = enemyUnit.currEquip.minDmg;
        eMaxDmg = enemyUnit.currEquip.maxDmg;
        eClipSize = enemyUnit.currEquip.clipSize;
        eCurrClip = enemyUnit.currEquip.currentClip;
        eAcc = enemyUnit.currEquip.accuracy;
        eRng = enemyUnit.currEquip.range;
        eIsMelee = enemyUnit.currEquip.isMelee;

        //Gun mod values
        eTempSpd = enemyUnit.currEquip.tempSpd;
        eTempDef = enemyUnit.currEquip.tempDef;
        eTempEva = enemyUnit.currEquip.tempEva;
        eTempLck = enemyUnit.currEquip.tempLck;
        eTempRes = enemyUnit.currEquip.tempRes;
        eTempMinDmg = enemyUnit.currEquip.tempMinDmg;
        eTempMaxDmg = enemyUnit.currEquip.tempMaxDmg;
        
        //Mods
        eMods = enemyUnit.currEquip.mods;
        eMod1Name = determineModName(eMods[0, 0], eMods[0, 1]);
        eMod2Name = determineModName(eMods[0, 0], eMods[0, 1]);
        eMod3Name = determineModName(eMods[0, 0], eMods[0, 1]);
    }

    //The following two functions generate stat displays.

    public string genPlayerStatDisplay()
    {
        //Stats can be modified. Let's build this from scratch.
        string stats = "DEF: " + pDef;
        if (pTempDef < 0)
        {
            stats += " (" + pTempDef + ")\t";
        }
        else if (pTempDef > 0)
        {
            stats += " (+" + pTempDef + ")\t";
        }
        else
        {
            stats += "\t\t\t";
        }
        stats += "EVA: " + pEva;
        if (pTempEva < 0)
        {
            stats += " (" + pTempEva + ")\n";
        }
        else if (pTempEva > 0)
        {
            stats += " (+" + pTempEva + ")\n";
        }
        else
        {
            stats += "\n";
        }
        stats += "SPD: " + pSpd;
        if (pTempSpd < 0)
        {
            stats += " (" + pTempSpd + ")\t";
        }
        else if (pTempSpd > 0)
        {
            stats += " (+" + pTempSpd + ")\t";
        }
        else
        {
            stats += "\t\t\t";
        }
        stats += "LCK: " + pLck;
        if (pTempLck < 0)
        {
            stats += " (" + pTempLck + ")\n";
        }
        else if (pTempLck > 0)
        {
            stats += " (+" + pTempLck + ")\n";
        }
        return stats;
    }

    public string genEnemyStatDisplay()
    {
        //Stats can be modified. Let's build this from scratch.
        string stats = "DEF: " + eDef;
        if (eTempDef < 0)
        {
            stats += " (" + eTempDef + ")\t";
        }
        else if (eTempDef > 0)
        {
            stats += " (+" + eTempDef + ")\t";
        }
        else
        {
            stats += "\t\t\t";
        }
        stats += "EVA: " + eEva;
        if (eTempEva < 0)
        {
            stats += " (" + eTempEva + ")\n";
        }
        else if (eTempEva > 0)
        {
            stats += " (+" + eTempEva + ")\n";
        }
        else
        {
            stats += "\n";
        }
        stats += "SPD: " + eSpd;
        if (eTempSpd < 0)
        {
            stats += " (" + eTempSpd + ")\t";
        }
        else if (eTempSpd > 0)
        {
            stats += " (+" + eTempSpd + ")\t";
        }
        else
        {
            stats += "\t\t\t";
        }
        stats += "LCK: " + eLck;
        if (eTempLck < 0)
        {
            stats += " (" + eTempLck + ")\n";
        }
        else if (eTempLck > 0)
        {
            stats += " (+" + eTempLck + ")\n";
        }
        return stats;
    }

    public string determineModName(int modTier, int modID)
    {
        switch (modTier)
        {
            //From here, T1
            case 1:
                switch (modID)
                {
                    case 1:
                        return ("Fleetfoot");
                    case 2:
                        return ("Ironclad");
                    case 3:
                        return ("Aware");
                    case 4:
                        return ("Lucky");
                    case 5:
                        return ("Resistant");
                    case 6:
                        return ("Determined");
                    case 7:
                        return ("Scavenger");
                    case 8:
                        return ("Ammo Capacity+");
                    default:
                        return ("--");
                }
            //From here, T2
            case 2:
                switch (modID)
                {
                    case 1:
                        return ("Brutal");
                    case 2:
                        return ("Scope");
                    case 3:
                        return ("Frontloaded");
                    case 4:
                        return ("Regeneration");
                    case 5:
                        return ("Stun");
                    case 6:
                        return ("Ammo Capacity++");
                    case 7:
                        return ("Ammo Capacity-");
                    default:
                        return ("--");
                }
            //From here, T3
            case 3:
                switch (modID)
                {
                    case 1:
                        return ("Recycle");
                    case 2:
                        return ("Kamikaze");
                    case 3:
                        return ("Backloaded");
                    case 4:
                        return ("Gentle");
                    case 5:
                        return ("Flatfoot");
                    case 6:
                        return ("Paperclad");
                    case 7:
                        return ("Unaware");
                    case 8:
                        return ("Unlucky");
                    case 9:
                        return ("Uncalibrated");
                    case 10:
                        return ("Ammo Capacity--");
                    case 11:
                        return ("Ethereal");
                    default:
                        return ("--");
                }
            //From here, T4
            case 4:
                switch (modID)
                {
                    default:
                        return ("--");
                }
            //Default case.
            default:
                return ("--");
        }
    }
}
