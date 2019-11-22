using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gacha : MonoBehaviour
{
    //In order: AR, SG, HG, RF, SWD
    public bool[] activePools = new bool[5];
    public bool[] rareChanceUpActive = new bool[5];
    public Item template;
    public float c, uc, r, sr;
    public int testA, testB, testC, testD;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Controller.c.gameMode == 3)
        {
            generateItem(testA, testB, testC, testD);
        }
    }

    public void determinePools(int matA, int matB, int matC, int matD)
    {
        checkAR(matA, matB, matC, matD);
        checkHG(matA, matB, matC, matD);
        checkSG(matA, matB, matC, matD);
        checkRF(matA, matB, matC, matD);
        checkSWD(matA, matB, matC, matD);
    }

    void checkAR(int matA, int matB, int matC, int matD)
    {
        //Cull condition: no mat is > 75.
        if (matA > 75 || matB > 75 || matC > 75 || matD > 75)
        {
            activePools[0] = false;
        }
        else if (matA >= 20 && matB >= 20 && matC >= 20 && matD >= 20)
        {
            activePools[0] = true;
            if (matA >= 50 && matB >= 50 && matC >= 50 && matD >= 50)
            {
                rareChanceUpActive[0] = true;
            }
        }
        else
        {
            activePools[0] = false;
        }
    }

    void checkSG(int matA, int matB, int matC, int matD)
    {
        //Cull condition: sum < 100.
        if (matA + matB + matC + matD < 100)
        {
            activePools[1] = false;
        }
        else if (matA >= 35 && matB >= 20 && matC >= 15 && matD >= 30)
        {
            activePools[1] = true;
            if (matA >= 75 && matB >= 42 && matC >= 25 && matD >= 60)
            {
                rareChanceUpActive[1] = true;
            }
        }
        else
        {
            activePools[1] = false;
        }
    }

    void checkHG(int matA, int matB, int matC, int matD)
    {
        //Cull condition: sum > 100.
        if (matA + matB + matC + matD > 100)
        {
            activePools[2] = false;
        }
        else if (matA >= 5 && matB >= 5 && matC >= 5 && matD >= 5)
        {
            activePools[2] = true;
            if (matA >= 25 && matB >= 25 && matC >= 25 && matD >= 25)
            {
                rareChanceUpActive[2] = true;
            }
        }
        else
        {
            activePools[2] = false;
        }
    }

    void checkRF(int matA, int matB, int matC, int matD)
    {
        //Cull condition: abs(mat3-mat4) > 50
        if (Mathf.Abs(matC - matD) > 50)
        {
            activePools[3] = false;
        }
        else if (matA >= 25 && matB >= 10 && matC >= 40 && matD >= 15)
        {
            activePools[3] = true;
            if (matA >= 50 && matB >= 30 && matC >= 80 && matD >= 35)
            {
                rareChanceUpActive[3] = true;
            }
        }
        else
        {
            activePools[3] = false;
        }
    }

    void checkSWD(int matA, int matB, int matC, int matD)
    {
        //Cull condition: Mat3 > 20 || Mat4 > 20
        if (matC > 20 || matD > 20)
        {
            activePools[4] = false;
        }
        else if (matA >= 30 && matB >= 10)
        {
            activePools[4] = true;
            if (matA >= 50 && matB >= 20)
            {
                rareChanceUpActive[4] = true;
            }
        }
        else
        {
            activePools[4] = false;
        }
    }

    public void generateItem(int matA, int matB, int matC, int matD)
    {
        determinePools(matA, matB, matC, matD);

        Item newItem = null;

        //What kind of gun are we making?
        int gunType = -1;
        bool validSet = false;
        foreach (bool b in activePools)
        {
            if (b)
            {
                validSet = true;
            }
        }
        if (validSet) {
            gunType = Random.Range(0, 5);
            while (!activePools[gunType])
            {
                int tempValue = Random.Range(0, 5);
                if (activePools[tempValue])
                {
                    gunType = tempValue;
                }
            }
            //Now the gunType is in hand; let's generate the ratios!
            if (rareChanceUpActive[gunType])
            {
                c = 20;
                uc = 50;
                r = 75;
                sr = 87.5f;
            }
            else
            {
                c = 40;
                uc = 65;
                r = 85;
                sr = 95;
            }
            float gunRarityChk = Random.Range(0, 100);
            int gunRarity = 0;
            if (gunRarityChk <= c)
            {
                gunRarity = 1;
            }
            else if (gunRarityChk <= uc)
            {
                gunRarity = 2;
            }
            else if (gunRarityChk <= r)
            {
                gunRarity = 3;
            }
            else if (gunRarityChk <= sr)
            {
                gunRarity = 4;
            }
            else
            {
                //Unique time, baby!
                gunRarity = 5;
            }
            generateGun(gunType, gunRarity);
        }
        else
        {
            //Somehow you failed to get the recipe working. Good job. Spit out failure.
            Debug.Log("No results. Oops.");
        }
        
    }

    public void generateGun(int gunType, int gunRarity)
    {
        /*Item newItem = null;
        newItem = Instantiate(template, transform.position, Quaternion.identity);
        //This is always a gun.
        newItem.itemID = 0;*/

        //Placeholder stuff; output string.
        string output = "Result: ";
        string outputRarity = "";
        string outputType = "";
        string mods = "";
        switch (gunRarity)
        {
            case 1:
                outputRarity = "Common ";
                mods = generateMods(1);
                break;
            case 2:
                outputRarity = "Uncommon ";
                mods = generateMods(2);
                break;
            case 3:
                outputRarity = "Rare ";
                mods = generateMods(3);
                break;
            case 4:
                outputRarity = "Super Rare ";
                mods = generateMods(4);
                break;
            case 5:
                outputRarity = "Unique ";
                break;
        }
        switch (gunType)
        {
            case 0:
                outputType = "Assault Rifle";
                break;
            case 1:
                outputType = "Shotgun";
                break;
            case 2:
                outputType = "Handgun";
                break;
            case 3:
                outputType = "Sniper Rifle";
                break;
            case 4:
                outputType = "Sword";
                break;
        }
        //Assemble output:
        if (gunRarity != 5)
        {
            output += outputRarity + outputType + "; Mods: " + mods;
        }
        else
        {
            output += outputRarity + outputType;
        }
        Debug.Log(output);
    }

    //Rework this into generating mods properly.
    public string generateMods(int noOfMods)
    {
        string[] mods = TextFileParser.tfp.itemList;
        string modOutput = "";
        for (int i = 0; i < noOfMods; i++)
        {
            int randMod = Random.Range(0, mods.Length);
            modOutput += mods[randMod] + ", ";
        }
        modOutput = modOutput.Substring(0, modOutput.Length - 2);
        return modOutput;
    }
}
