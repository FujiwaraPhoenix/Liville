using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerData : MonoBehaviour
{
    public int pID;
    public Text pName, pHPDisplay, pStats;
    public int pHP, pMaxHP, pSpd, pEva, pDef, pLck, pMvt;
    public Image pImg;
    public Image[] pHPPips;
    public bool preloaded = false;
    public Image hpPip;
    public GameObject pipHolder;
    // Start is called before the first frame update
    void Start()
    {
        if (!preloaded)
        {
            updateValues();
            preloaded = !preloaded;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void updateValues()
    {
        //Name and face
        Unit playerUnit = Controller.c.playerUnits[pID];
        pName.text = playerUnit.unitName;
        pImg.sprite = playerUnit.unitFace;

        //Player stats
        pHP = playerUnit.hp;
        pMaxHP = playerUnit.maxhp;
        pEva = playerUnit.eva;
        pDef = playerUnit.def;
        pLck = playerUnit.lck;
        pMvt = playerUnit.mvt;

        //Now, strings.
        pHPDisplay.text = "HP: " + pHP + "/" + pMaxHP;
        pStats.text = statDisplay();

        //And HP.
        initializeHP();
    }

    public string statDisplay()
    {
        string output = "";
        output += "DEF: " + pDef + "\t\t\t";
        output += "EVA: " + pEva + "\n";
        output += "MVT: " + pMvt + "\t\t";
        output += "LCK: " + pLck;
        return output;
    }

    public void initializeHP()
    {
        if (pHPPips.Length != 0)
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
            temp.transform.SetParent(pipHolder.transform, false);
            if (i < 20)
            {
                temp.transform.localPosition = new Vector3(-75+(7.5f * i), 83, 0);
            }
            else
            {
                temp.transform.localPosition = new Vector3(-75 + (7.5f * (i-20)), 73, 0);
            }
        }
    }
}
