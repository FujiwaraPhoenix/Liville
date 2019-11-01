using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleMenuUI : MonoBehaviour
{
    public Image menuPointer;
    public GameObject menuItems;
    public GameObject enemyDisplay;

    public Sprite pipOn, pipOff;

    //Player Stats
    public Text pUnitName, pGunName;
    public Image pImg;
    public Text HPDisplay, statDisplay, gunStatDisplay, modDisplay;
    public int pHP, pMaxHP, pDef, pEva, pSpd, pLck;
    public int pClipSize, pCurrClip, pDmg, pAcc, pRng, pMod1, pMod2, pMod3;
    public Image[] pHPPips = new Image[40];

    //Enemy Stats
    public bool foundEnemy = false;
    public Text eUnitName, eGunName;
    public Image eImg;
    public Text eHPDisplay, estatDisplay, egunStatDisplay, emodDisplay;
    public int eHP, eMaxHP, eDef, eEva, eSpd, eLck;
    public int eClipSize, eCurrClip, eDmg, eAcc, eRng, eMod1, eMod2, eMod3;
    public Image[] eHPPips = new Image[40];

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        updateMenuPosition();
        updatePlayerDisplay();
        updateEnemyDisplay();
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
        //Do you have a player unit selected? If so, display their data.
        if (Controller.c.mp.targetUnit != null)
        {
            updatePlayerValues(Controller.c.mp.targetUnit);
        }
        //Check if you're hovering over a player unit:
        else {
            foreach (Unit u in Controller.c.playerUnits)
            {
                if ((u.position[0] == Controller.c.mp.currX) && u.position[1] == Controller.c.mp.currY)
                {
                    //If you are, update the data.
                    updatePlayerValues(u);
                }
            }
        }
        HPDisplay.text = "HP: " + pHP + "/" + pMaxHP;
        statDisplay.text = "DEF: " + pDef + "\t\t\tEVA: " + pEva + "\nSPD: " + pSpd + "\t\t\tLCK: " + pLck;
        gunStatDisplay.text = "DMG: " + pDmg + "\nACC: " + pAcc + "\nRNG: " + pRng;

        for (int i = 0; i < 40; i++)
        {
            if (i <= pHP)
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
                updateEnemyValues(u);
                foundEnemy = true;
            }
        }
        eHPDisplay.text = "HP: " + eHP + "/" + eMaxHP;
        estatDisplay.text = "DEF: " + eDef + "\t\t\tEVA: " + eEva + "\nSPD: " + eSpd + "\t\t\tLCK: " + eLck;
        egunStatDisplay.text = "DMG: " + eDmg + "\nACC: " + eAcc + "\nRNG: " + eRng;
        for (int i = 0; i < 40; i++)
        {
            if (i <= eHP)
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
        pDmg = playerUnit.currEquip.dmg;
        pClipSize = playerUnit.currEquip.clipSize;
        pCurrClip = playerUnit.currEquip.currentClip;
        pAcc = playerUnit.currEquip.accuracy;
        pRng = playerUnit.currEquip.range;
        //Add mods later.
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
        eDmg = enemyUnit.currEquip.dmg;
        eClipSize = enemyUnit.currEquip.clipSize;
        eCurrClip = enemyUnit.currEquip.currentClip;
        eAcc = enemyUnit.currEquip.accuracy;
        eRng = enemyUnit.currEquip.range;
        //Add mods later.
    }

}
