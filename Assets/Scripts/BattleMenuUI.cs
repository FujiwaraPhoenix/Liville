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
    public int pClipSize, pCurrClip, pMinDmg, pMaxDmg, pAcc, pRng, pMod1, pMod2, pMod3;
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
    public int eClipSize, eCurrClip, eMinDmg, eMaxDmg, eAcc, eRng, eMod1, eMod2, eMod3;
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
        if (Controller.c.gameMode == 0)
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
        HPDisplay.text = "HP: " + pHP + "/" + pMaxHP;
        statDisplay.text = "DEF: " + pDef + "\t\t\tEVA: " + pEva + "\nSPD: " + pSpd + "\t\t\tLCK: " + pLck;
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
        eHPDisplay.text = "HP: " + eHP + "/" + eMaxHP;
        estatDisplay.text = "DEF: " + eDef + "\t\t\tEVA: " + eEva + "\nSPD: " + eSpd + "\t\t\tLCK: " + eLck;
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
        //Add mods later.
        pMod1 = playerUnit.currEquip.mods[0];
        pMod2 = playerUnit.currEquip.mods[1];
        pMod3 = playerUnit.currEquip.mods[2];
        pMod1Name = determineModName(pMod1);
        pMod2Name = determineModName(pMod2);
        pMod3Name = determineModName(pMod3);
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
        //Add mods later.
        eMod1 = enemyUnit.currEquip.mods[0];
        eMod2 = enemyUnit.currEquip.mods[1];
        eMod3 = enemyUnit.currEquip.mods[2];
        eMod1Name = determineModName(eMod1);
        eMod2Name = determineModName(eMod2);
        eMod3Name = determineModName(eMod3);
    }

    public string determineModName(int modID)
    {
        switch (modID)
        {
            case 1:
                return ("Kamikaze");
            case 2:
                return ("Stun");
            case 3:
                return ("Recycle");
            case 4:
                return ("Regeneration");
            case 5:
                return ("Frontloaded");
            case 6:
                return ("Backloaded");
            default:
                return ("--");
        }
    }

}
