using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleMenuUI : MonoBehaviour
{
    public static BattleMenuUI bmui;
    
    public GameObject menuItems;
    public GameObject enemyDisplay, playerDisplay;
    public GameObject pHPPipHolder, eHPPipHolder, pAmmoPipHolder, eAmmoPipHolder;

    public Sprite pipOn, pipOff;
    public Sprite ammoPipOn, ammoPipOff;
    public Image hpPip, ammoPip;

    public bool loadInitial = false;

    //For selection of options.
    public BattleMenuButton[] buttons = new BattleMenuButton[4];
    

    //Player Stats
    public Unit currentPlayer;
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
    public Unit currentEnemy;
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
        if (Controller.c.gameMode == 4 && Controller.c.currMap.loaded)
        {
            updateMenuPosition();
            updatePlayerDisplay();
            updateEnemyDisplay();
        }
    }

    public void updateMenuPosition()
    {
        if (Controller.c.mp.menuActive)
        {
            menuItems.gameObject.SetActive(true);
            foreach (BattleMenuButton bmb in buttons)
            {
                bmb.updateSprite();
            }
        }
        else
        {
            menuItems.gameObject.SetActive(false);
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
        bool mustUpdatePips = false;
        if (Controller.c.mp.targetUnit != null)
        {
            updatePlayerValues(Controller.c.mp.targetUnit);
            foundPlayer = true;
        }
        //Check if you're hovering over a player unit:
        else
        {
            foreach (Unit u in Controller.c.playerUnits)
            {
                if ((u.position[0] == Controller.c.mp.currX) && u.position[1] == Controller.c.mp.currY)
                {
                    //If you are, update the data.
                    if (currentPlayer != u)
                    {
                        currentPlayer = u;
                        updatePlayerValues(currentPlayer);
                        foundPlayer = true;
                        mustUpdatePips = true;
                    }
                }
            }
        }
        if (mustUpdatePips)
        {
            initializeHP();
            initializePAmmo();
        }
        //Generate display text.
        //HP is usually left unmodified, so leave this as is.
        HPDisplay.text = "HP: " + pHP + "/" + pMaxHP;
        //Stats are returned in the corresponding fxn below.
        statDisplay.text = genPlayerStatDisplay();
        gunStatDisplay.text = "DMG: " + pMinDmg + "-" + pMaxDmg + "\nACC: " + pAcc + "\nRNG: " + pRng;
        modDisplay.text = "Mod 1: " + pMod1Name + "\nMod 2: " + pMod2Name + "\nMod 3: " + pMod3Name;
        forceUpdatePips();
    }


    public void updateEnemyDisplay()
    {
        if (!foundEnemy || currentEnemy == null)
        {
            enemyDisplay.SetActive(false);
        }
        else
        {
            enemyDisplay.SetActive(true);
        }

        //Check if you're hovering over an enemy unit:
        bool mustUpdatePips = false;
        foreach (Unit u in Controller.c.enemyUnits)
        {
            if ((u.position[0] == Controller.c.mp.currX) && u.position[1] == Controller.c.mp.currY)
            {
                //If you are, update the data.
                if (currentEnemy != u)
                {
                    currentEnemy = u;
                    updateEnemyValues(currentEnemy);
                    foundEnemy = true;
                    mustUpdatePips = true;
                }
            }
        }
        if (mustUpdatePips)
        {
            initializeEHP();
            initializeEAmmo();
        }
        //Generate display text.
        //HP is usually left unmodified, so leave this as is.
        eHPDisplay.text = "HP: " + eHP + "/" + eMaxHP;
        //Stats are returned in the corresponding fxn below.
        estatDisplay.text = genEnemyStatDisplay();
        egunStatDisplay.text = "DMG: " + eMinDmg + "-" + eMaxDmg + "\nACC: " + eAcc + "\nRNG: " + eRng;
        emodDisplay.text = "Mod 1: " + eMod1Name + "\nMod 2: " + eMod2Name + "\nMod 3: " + eMod3Name;
        forceUpdateEPips();
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
        pMod1Name = Controller.c.determineModName(pMods[0,0], pMods[0,1]);
        pMod2Name = Controller.c.determineModName(pMods[1, 0], pMods[1, 1]);
        pMod3Name = Controller.c.determineModName(pMods[2, 0], pMods[2, 1]);
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
        eMod1Name = Controller.c.determineModName(eMods[0, 0], eMods[0, 1]);
        eMod2Name = Controller.c.determineModName(eMods[0, 0], eMods[0, 1]);
        eMod3Name = Controller.c.determineModName(eMods[0, 0], eMods[0, 1]);
    }

    //The following function updates the HP and ammo pips of either a player or enemy unit.

    public void initializeHP()
    {
        if (pHPPips.Length > 0)
        {
            for (int i = 0; i < pHPPips.Length; i++)
            {
                Destroy(pHPPips[i].gameObject);
            }
        }
        pHPPips = new Image[pMaxHP];
        for (int i = 0; i < pMaxHP; i++)
        {
            Image temp = Instantiate(hpPip, Vector3.zero, Quaternion.identity);
            pHPPips[i] = temp;
            temp.transform.SetParent(pHPPipHolder.transform, false);
            if (i < 20)
            {
                temp.transform.localPosition = new Vector3(-72.5f + (7.5f * i), 55, 0);
            }
            else
            {
                temp.transform.localPosition = new Vector3(-72.5f + (7.5f * (i - 20)), 45, 0);
            }
        }
    }

    public void initializeEHP()
    {
        if (eHPPips.Length != 0)
        {
            for (int i = 0; i < eHPPips.Length; i++)
            {
                Destroy(eHPPips[i].gameObject);
            }
        }
        eHPPips = new Image[eMaxHP];
        for (int i = 0; i < eMaxHP; i++)
        {
            Image temp = Instantiate(hpPip, Vector3.zero, Quaternion.identity);
            eHPPips[i] = temp;
            temp.transform.SetParent(eHPPipHolder.transform, false);
            if (i < 20)
            {
                temp.transform.localPosition = new Vector3(-72.5f + (7.5f * i), 55, 0);
            }
            else
            {
                temp.transform.localPosition = new Vector3(-72.5f + (7.5f * (i - 20)), 45, 0);
            }
        }
    }

    //The following two functions generate ammopips.

    public void initializePAmmo()
    {
        //Clear ammo pips
        if (pAmmoPips.Length != 0)
        {
            for (int i = 0; i < pAmmoPips.Length; i++)
            {
                Destroy(pAmmoPips[i].gameObject);
            }
        }
        pAmmoPips = new Image[pClipSize];
        int isOdd = pAmmoPips.Length % 2;
        for (int i = 0; i < pAmmoPips.Length; i++)
        {
            Image temp = Instantiate(ammoPip, Vector3.zero, Quaternion.identity);
            pAmmoPips[i] = temp;
            temp.transform.SetParent(pAmmoPipHolder.transform, false);
            temp.transform.localScale = new Vector3(0.1f, 0.1f, 1);
            //Make a new image for each, offset by as much as you need.
            if (isOdd == 0)
            {
                //Even; formula is (-10(pip array size/2) + 10n -5)
                temp.transform.localPosition = new Vector3((-10 * Mathf.Floor(pAmmoPips.Length % 2)) + (10 * i) - 5, -30, 0);
            }
            else if (pAmmoPips.Length == 1)
            {
                temp.transform.localPosition = new Vector3(0, -30, 0);
            }
            else
            {
                temp.transform.localPosition = new Vector3((-10 * Mathf.Floor(pAmmoPips.Length % 2)) + (10 * i), -30, 0);
            }
        }
    }

    public void initializeEAmmo()
    {
        //Clear ammo pips
        if (eAmmoPips.Length != 0)
        {
            for (int i = 0; i < eAmmoPips.Length; i++)
            {
                Destroy(eAmmoPips[i].gameObject);
            }
        }
        eAmmoPips = new Image[eClipSize];
        int isOdd = eAmmoPips.Length % 2;
        for (int i = 0; i < eAmmoPips.Length; i++)
        {
            Image temp = Instantiate(ammoPip, Vector3.zero, Quaternion.identity);
            eAmmoPips[i] = temp;
            temp.transform.SetParent(eAmmoPipHolder.transform, false);
            temp.transform.localScale = new Vector3(0.1f, 0.1f, 1);
            //Make a new image for each, offset by as much as you need.
            if (isOdd == 0)
            {
                //Even; formula is (-10(pip array size/2) + 10n -5)
                temp.transform.localPosition = new Vector3((-10 * Mathf.Floor(eAmmoPips.Length % 2)) + (10 * i) - 5, -30, 0);
            }
            else if (eAmmoPips.Length == 1)
            {
                temp.transform.localPosition = new Vector3(0, -30, 0);
            }
            else
            {
                temp.transform.localPosition = new Vector3((-10 * Mathf.Floor(eAmmoPips.Length % 2)) + (10 * i), -30, 0);
            }
        }
    }

    //The following two functions generate stat displays.

    public string genPlayerStatDisplay()
    {
        string output = "";
        output += "DEF: " + pDef + "\t\t\t";
        output += "EVA: " + pEva + "\n";
        output += "SPD: " + pSpd + "\t\t\t";
        output += "LCK: " + pLck;
        return output;
    }

    public string genEnemyStatDisplay()
    {
        string output = "";
        output += "DEF: " + eDef + "\t\t\t";
        output += "EVA: " + eEva + "\n";
        output += "SPD: " + eSpd + "\t\t\t";
        output += "LCK: " + eLck;
        return output;
    }

    public void forceUpdatePips()
    {
        for (int i = 0; i < pHPPips.Length; i++)
        {
            if (i < pHP)
            {
                pHPPips[i].sprite = pipOn;
            }
            else if (i < pMaxHP)
            {
                pHPPips[i].sprite = pipOff;
            }
        }
        if (!pIsMelee)
        {
            for (int i = 0; i < pAmmoPips.Length; i++)
            {
                if (i < pCurrClip)
                {
                    pAmmoPips[i].sprite = ammoPipOn;
                }
                else
                {
                    pAmmoPips[i].sprite = ammoPipOff;
                }
            }
        }
    }

    public void forceUpdateEPips()
    {
        for (int i = 0; i < eHPPips.Length; i++)
        {
            if (i < eHP)
            {
                eHPPips[i].sprite = pipOn;
            }
            else if (i < eMaxHP)
            {
                eHPPips[i].sprite = pipOff;
            }
        }
        if (!eIsMelee)
        {
            for (int i = 0; i < eAmmoPips.Length; i++)
            {
                if (i <= eCurrClip)
                {
                    eAmmoPips[i].sprite = ammoPipOn;
                }
                else
                {
                    eAmmoPips[i].sprite = ammoPipOff;
                }
            }
        }
    }

}
