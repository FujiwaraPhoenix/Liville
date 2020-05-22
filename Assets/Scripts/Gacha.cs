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
    public Item lastGeneratedGun;
    public string basicGunData;
    //In order: AR, HG, SG, RF, SWD.
    public AudioClip[] wepSounds = new AudioClip[5];
    public AudioClip potionSound;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
        else if (matA >= 30 && matB >= 20 && matC >= 10 && matD >= 30)
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
        if (validSet)
        {
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
                /*
                 * This is the default set of values (with uniques)
                c = 20;
                uc = 50;
                r = 75;
                sr = 87.5f;*/

                c = 40;
                uc = 70;
                r = 90;
                sr = 100;

            }
            else
            {
                /*
                 * This is the default set of values (with uniques)
                c = 40;
                uc = 65;
                r = 85;
                sr = 95;*/

                c = 20;
                uc = 50;
                r = 80;
                sr = 100;
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
                //Temporarily not in the game.
                gunRarity = 5;
            }
            generateGun(gunType, gunRarity);
        }
        else
        {
            //Generate an item if the weapon gen fails.
            generatePotion(matA, matB, matC, matD);
        }

    }

    public void generateGun(int gunType, int gunRarity)
    {
        Item newItem = Instantiate(template, transform.position, Quaternion.identity);
        //This is always a gun.
        newItem.itemID = 0;

        //Placeholder stuff; output string.
        string output = "";
        string outputRarity = "";
        string outputType = "";
        string mods = "";
        string generatedName = generateName(newItem);
        newItem.itemName = generatedName;
        string[] modList = null;
        newItem.rarity = gunRarity;
        switch (gunType)
        {
            case 0:
                outputType = "Assault Rifle\n";
                generateAR(gunRarity, newItem);
                newItem.itemType = "Assault Rifle";
                newItem.useSound = wepSounds[0];
                break;
            case 1:
                outputType = "Shotgun\n";
                generateSG(gunRarity, newItem);
                newItem.itemType = "Shotgun";
                newItem.useSound = wepSounds[2];
                break;
            case 2:
                outputType = "Handgun\n";
                generateHG(gunRarity, newItem);
                newItem.itemType = "Handgun";
                newItem.useSound = wepSounds[1];
                break;
            case 3:
                outputType = "Sniper Rifle\n";
                generateRF(gunRarity, newItem);
                newItem.itemType = "Sniper Rifle";
                newItem.useSound = wepSounds[3];
                break;
            case 4:
                outputType = "Sword\n";
                generateSWD(gunRarity, newItem);
                newItem.itemType = "Sword";
                newItem.useSound = wepSounds[4];
                break;
        }
        //TODO: Add mod output into... Well, the output. Again.
        switch (gunRarity)
        {
            case 1:
                outputRarity = "Common ";
                generateMods(1, modList, newItem.isMelee, newItem);
                break;
            case 2:
                outputRarity = "Uncommon ";
                generateMods(2, modList, newItem.isMelee, newItem);
                break;
            case 3:
                outputRarity = "Rare ";
                generateMods(3, modList, newItem.isMelee, newItem);
                break;
            case 4:
                outputRarity = "Super Rare ";
                generateMods(4, modList, newItem.isMelee, newItem);
                break;
            case 5:
                outputRarity = "Unique ";
                break;
        }
        for (int i = 0; i < newItem.mods.Length / 2; i++)
        {
            mods += Controller.c.determineModName(newItem.mods[i, 0], newItem.mods[i, 1]);
            if (newItem.mods.Length / 2 - i > 1)
            {
                mods += "\n";
            }
        }
        //Assemble output:
        if (gunRarity != 5)
        {
            output += generatedName + "\n" + outputRarity + outputType + "Mods:\n" + mods;
        }
        else
        {
            output += outputRarity + outputType;
        }
        Debug.Log(output);
        applyMods(newItem);
        InvManager.im.armory.Add(newItem);
        newItem.transform.parent = InvManager.im.transform;
        lastGeneratedGun = newItem;
        basicGunData = output;
    }

    //This generates the mods
    public void generateMods(int weaponRarity, string[] modList, bool isMelee, Item moddedItem)
    {
        int modCountChance = Random.Range(0, 100);
        int modCount = 0;
        switch (weaponRarity)
        {
            //Here, we determine the number of mods to be generated on a given gun.
            case 1:
                //Common gun.
                if (modCountChance < 25)
                {
                    modCount = 0;
                }
                else if (modCountChance < 95)
                {
                    modCount = 1;
                }
                else
                {
                    modCount = 2;
                }
                break;
            case 2:
                //Uncommon gun.
                if (modCountChance < 14)
                {
                    modCount = 0;
                }
                else if (modCountChance < 51)
                {
                    modCount = 1;
                }
                else if (modCountChance < 83)
                {
                    modCount = 2;
                }
                else
                {
                    modCount = 3;
                }
                break;

            case 3:
                //Rare gun.
                if (modCountChance < 5)
                {
                    modCount = 0;
                }
                else if (modCountChance < 40)
                {
                    modCount = 1;
                }
                else if (modCountChance < 85)
                {
                    modCount = 2;
                }
                else
                {
                    modCount = 3;
                }
                break;

            case 4:
                //Super rare gun.
                if (modCountChance < 5)
                {
                    modCount = 0;
                }
                else if (modCountChance < 37)
                {
                    modCount = 1;
                }
                else if (modCountChance < 80)
                {
                    modCount = 2;
                }
                else
                {
                    modCount = 3;
                }
                break;
        }

        //Now that we have a number of the mods that we need to make... Well, let's generate them.
        if (modCount != 0)
        {
            int[,] modsToAdd = new int[modCount, 2];
            for (int i = 0; i < modCount; i++)
            {
                bool foundMod = false;
                int modTier = 0;
                int modIndex = -1;
                while (!foundMod)
                {
                    int modRarity = Random.Range(0, 100);
                    //First, we generate the mod tier.
                    switch (weaponRarity)
                    {
                        case 1:
                            if (modRarity < 10)
                            {
                                modTier = -1;
                            }
                            else if (modRarity < 70)
                            {
                                modTier = 1;
                            }
                            else if (modRarity < 95)
                            {
                                modTier = 2;
                            }
                            else
                            {
                                modTier = 3;
                            }
                            break;
                        case 2:
                            if (modRarity < 10)
                            {
                                modTier = -1;
                            }
                            else if (modRarity < 42)
                            {
                                modTier = 1;
                            }
                            else if (modRarity < 72)
                            {
                                modTier = 2;
                            }
                            else if (modRarity < 86)
                            {
                                modTier = 3;
                            }
                            else
                            {
                                modTier = 4;
                            }
                            break;
                        case 3:
                            if (modRarity < 5)
                            {
                                modTier = -1;
                            }
                            else if (modRarity < 10)
                            {
                                modTier = 1;
                            }
                            else if (modRarity < 42)
                            {
                                modTier = 2;
                            }
                            else if (modRarity < 85)
                            {
                                modTier = 3;
                            }
                            else
                            {
                                modTier = 4;
                            }
                            break;
                        case 4:
                            if (modRarity < 5)
                            {
                                modTier = 1;
                            }
                            else if (modRarity < 37)
                            {
                                modTier = 2;
                            }
                            else if (modRarity < 80)
                            {
                                modTier = 3;
                            }
                            else
                            {
                                modTier = 4;
                            }
                            break;
                    }
                    //Now we generate the mod proper.
                    switch (modTier)
                    {
                        case -1:
                            modIndex = Random.Range(1, 9);
                            if (!isMelee && modIndex != 7 && modIndex != 8)
                            {
                                foundMod = true;
                            }
                            break;
                        case 1:
                            modIndex = Random.Range(1, 13);
                            if (!isMelee && modIndex != 8)
                            {
                                foundMod = true;
                            }
                            break;
                        case 2:
                            modIndex = Random.Range(1, 7);
                            if (!isMelee && modIndex != 3 && modIndex != 6)
                            {
                                foundMod = true;
                            }
                            break;
                        case 3:
                            modIndex = Random.Range(1, 5);
                            if (!isMelee && modIndex != 1 && modIndex != 3)
                            {
                                foundMod = true;
                            }
                            break;
                        case 4:
                            //No mods for T4 yet, so.
                            break;
                    }
                }
                modsToAdd[i, 0] = modTier;
                modsToAdd[i, 1] = modIndex;
            }
            moddedItem.mods = modsToAdd;
            verifyMods(moddedItem);
        }
    }

    //The proceeding 5 functions are for the stat generation of guns.

    public void generateAR(int rarityValue, Item moddedItem)
    {
        switch (rarityValue)
        {
            case 1:
                //Common
                moddedItem.minDmg = Random.Range(1, 3);
                moddedItem.maxDmg = Random.Range(3, 5);
                moddedItem.range = Random.Range(2, 4);
                moddedItem.accuracy = Random.Range(70, 86);
                moddedItem.clipSize = Random.Range(2, 4);
                break;
            case 2:
                //Uncommon
                moddedItem.minDmg = Random.Range(1, 3);
                moddedItem.maxDmg = Random.Range(3, 6);
                moddedItem.range = Random.Range(2, 4);
                moddedItem.accuracy = Random.Range(70, 91);
                moddedItem.clipSize = 3;
                break;
            case 3:
                //Rare
                moddedItem.minDmg = Random.Range(2, 4);
                moddedItem.maxDmg = Random.Range(3, 6);
                moddedItem.range = Random.Range(2, 5);
                moddedItem.accuracy = Random.Range(75, 96);
                moddedItem.clipSize = Random.Range(3, 5);
                break;
            case 4:
                //Super Rare
                moddedItem.minDmg = 3;
                moddedItem.maxDmg = Random.Range(5, 9);
                moddedItem.range = Random.Range(3, 6);
                moddedItem.accuracy = Random.Range(85, 101);
                moddedItem.clipSize = Random.Range(3, 6);
                break;
        }
        moddedItem.currentClip = moddedItem.clipSize;
    }
    public void generateSG(int rarityValue, Item moddedItem)
    {
        switch (rarityValue)
        {
            case 1:
                //Common
                moddedItem.minDmg = Random.Range(3, 4);
                moddedItem.maxDmg = Random.Range(4, 7);
                moddedItem.range = 1;
                moddedItem.accuracy = Random.Range(70, 81);
                moddedItem.clipSize = 2;
                break;
            case 2:
                //Uncommon
                moddedItem.minDmg = 4;
                moddedItem.maxDmg = Random.Range(5, 7);
                moddedItem.range = Random.Range(1, 3);
                moddedItem.accuracy = Random.Range(75, 86);
                moddedItem.clipSize = 3;
                break;
            case 3:
                //Rare
                moddedItem.minDmg = 4;
                moddedItem.maxDmg = Random.Range(6, 9);
                moddedItem.range = 2;
                moddedItem.accuracy = Random.Range(85, 96);
                moddedItem.clipSize = 2;
                break;
            case 4:
                //Super Rare
                moddedItem.minDmg = 5;
                moddedItem.maxDmg = Random.Range(7, 11);
                moddedItem.range = Random.Range(2, 4);
                moddedItem.accuracy = Random.Range(90, 106);
                moddedItem.clipSize = Random.Range(3, 5);
                break;
        }
        moddedItem.currentClip = moddedItem.clipSize;
    }
    public void generateHG(int rarityValue, Item moddedItem)
    {
        switch (rarityValue)
        {
            case 1:
                //Common
                moddedItem.minDmg = Random.Range(1, 4);
                moddedItem.maxDmg = Random.Range(2, 5);
                moddedItem.range = Random.Range(1, 4);
                moddedItem.accuracy = Random.Range(60, 81);
                moddedItem.clipSize = Random.Range(3, 5);
                break;
            case 2:
                //Uncommon
                moddedItem.minDmg = Random.Range(2, 4);
                moddedItem.maxDmg = Random.Range(4, 6);
                moddedItem.range = Random.Range(2, 4);
                moddedItem.accuracy = Random.Range(65, 86);
                moddedItem.clipSize = Random.Range(3, 5);
                break;
            case 3:
                //Rare
                moddedItem.minDmg = Random.Range(2, 4);
                moddedItem.maxDmg = Random.Range(4, 7);
                moddedItem.range = Random.Range(2, 4);
                moddedItem.accuracy = Random.Range(75, 91);
                moddedItem.clipSize = 4;
                break;
            case 4:
                //Super Rare
                moddedItem.minDmg = Random.Range(3, 6);
                moddedItem.maxDmg = Random.Range(7, 9);
                moddedItem.range = Random.Range(2, 4);
                moddedItem.accuracy = Random.Range(85, 96);
                moddedItem.clipSize = Random.Range(4, 6);
                break;
        }
        moddedItem.currentClip = moddedItem.clipSize;
    }
    public void generateRF(int rarityValue, Item moddedItem)
    {
        switch (rarityValue)
        {
            case 1:
                //Common
                moddedItem.minDmg = Random.Range(2, 4);
                moddedItem.maxDmg = Random.Range(4, 6);
                moddedItem.range = 3;
                moddedItem.accuracy = Random.Range(80, 91);
                moddedItem.clipSize = Random.Range(1, 3);
                break;
            case 2:
                //Uncommon
                moddedItem.minDmg = Random.Range(4, 7);
                moddedItem.maxDmg = Random.Range(7, 10);
                moddedItem.range = Random.Range(3, 5);
                moddedItem.accuracy = Random.Range(85, 91);
                moddedItem.clipSize = Random.Range(1, 3);
                break;
            case 3:
                //Rare
                moddedItem.minDmg = Random.Range(7, 9);
                moddedItem.maxDmg = Random.Range(9, 11);
                moddedItem.range = Random.Range(3, 6);
                moddedItem.accuracy = Random.Range(85, 101);
                moddedItem.clipSize = 2;
                break;
            case 4:
                //Super Rare
                moddedItem.minDmg = Random.Range(10, 12);
                moddedItem.maxDmg = Random.Range(12, 15);
                moddedItem.range = Random.Range(4, 6);
                moddedItem.accuracy = Random.Range(95, 106);
                moddedItem.clipSize = 2;
                break;
        }
        moddedItem.currentClip = moddedItem.clipSize;
    }
    public void generateSWD(int rarityValue, Item moddedItem)
    {
        switch (rarityValue)
        {
            case 1:
                //Common
                moddedItem.minDmg = 4;
                moddedItem.maxDmg = Random.Range(5, 7);
                moddedItem.range = 1;
                moddedItem.accuracy = Random.Range(75, 86);
                break;
            case 2:
                //Uncommon
                moddedItem.minDmg = 4;
                moddedItem.maxDmg = Random.Range(5, 8);
                moddedItem.range = 1;
                moddedItem.accuracy = Random.Range(80, 96);
                break;
            case 3:
                //Rare
                moddedItem.minDmg = 5;
                moddedItem.maxDmg = Random.Range(6, 10);
                moddedItem.range = 1;
                moddedItem.accuracy = Random.Range(90, 101);
                break;
            case 4:
                //Super Rare
                moddedItem.minDmg = 8;
                moddedItem.maxDmg = Random.Range(10, 18);
                moddedItem.range = Random.Range(1, 3);
                moddedItem.accuracy = Random.Range(100, 121);
                break;
        }
        moddedItem.currentClip = 1;
        moddedItem.clipSize = 1;
        moddedItem.isMelee = true;
    }

    public string generateName(Item newItem)
    {
        string output = null;
        TextFileParser.tfp.heldData = TextFileParser.tfp.namePrefixes.text;
        TextFileParser.tfp.tStringToList();
        string[] data = TextFileParser.tfp.itemList;
        output = data[Random.Range(0, data.Length)];
        TextFileParser.tfp.heldData = TextFileParser.tfp.nameSuffixes.text;
        TextFileParser.tfp.tStringToList();
        data = TextFileParser.tfp.itemList;
        output += " " + data[Random.Range(0, data.Length)];
        return output;
    }

    public void verifyMods(Item itemToCheck)
    {
        //Check mod list length;
        if (itemToCheck.mods.GetLength(0) > 1)
        {
            //We have at least two mods here. Let's check.
            int modCount = itemToCheck.mods.GetLength(0);
            if (modCount == 2)
            {
                //Compare the two.
                bool isWepMelee = itemToCheck.isMelee;
                int[] modSetA = { itemToCheck.mods[0, 0], itemToCheck.mods[0, 1] };
                int[] modSetB = { itemToCheck.mods[1, 0], itemToCheck.mods[1, 1] };
                int newModB = modSetB[1];
                if (compareModSets(modSetA, modSetB))
                {
                    newModB = modConfirmTwo(modSetA, modSetB, isWepMelee);
                }
                itemToCheck.mods[1, 1] = newModB;
            }
            else if (modCount == 3)
            {
                //Has to have 3 mods, so.
                //Compare them all to each other.
                bool isWepMelee = itemToCheck.isMelee;
                int[] modSetA = { itemToCheck.mods[0, 0], itemToCheck.mods[0, 1] };
                int[] modSetB = { itemToCheck.mods[1, 0], itemToCheck.mods[1, 1] };
                int[] modSetC = { itemToCheck.mods[2, 0], itemToCheck.mods[2, 1] };
                bool compAB = compareModSets(modSetA, modSetB);
                bool compAC = compareModSets(modSetA, modSetC);
                bool compBC = compareModSets(modSetB, modSetC);
                int newModA = modSetA[1];
                int newModB = modSetB[1];
                int newModC = modSetC[1];
                //Checks, in order:
                //If A == B, change B. Then run 3Check on A/B/C.
                //Then, check if A == C. If so, change C, then run 3Check on A/C/B.
                //Else, A != B and A != C, so run 2Check on B/C, then 3Check on B/C/A.
                if (compAB)
                {
                    newModB = modConfirmTwo(modSetA, modSetB, isWepMelee);
                    newModC = modConfirmThree(modSetA, modSetB, modSetC, isWepMelee);
                }
                else if (compAC)
                {
                    newModC = modConfirmTwo(modSetA, modSetC, isWepMelee);
                    newModB = modConfirmThree(modSetA, modSetC, modSetB, isWepMelee);
                }
                else if (compBC)
                {
                    newModC = modConfirmTwo(modSetB, modSetC, isWepMelee);
                    newModA = modConfirmThree(modSetB, modSetC, modSetB, isWepMelee);
                }
                itemToCheck.mods[0, 1] = newModA;
                itemToCheck.mods[1, 1] = newModB;
                itemToCheck.mods[2, 1] = newModC;
            }
        }
    }

    //Compares to sets of mods, and returns true if they are the same, false if they are not.
    public bool compareModSets(int[] setA, int[] setB)
    {
        if ((setA[0] == setB[0]) && (setA[1] == setB[1]))
        {
            return true;
        }
        return false;
    }

    public int modConfirmTwo(int[] comparison, int[] toChange, bool isThisMelee)
    {
        //Let's be nice and reroll to same tier.
        int modTier = comparison[0];
        bool foundMod = false;
        int newMod = toChange[1];
        while ((comparison[1] == newMod) && !foundMod)
        {
            switch (modTier)
            {
                case -1:
                    newMod = Random.Range(1, 9);
                    if (!(isThisMelee) && newMod != 7 && newMod != 8 && newMod != comparison[1])
                    {
                        foundMod = true;
                    }
                    break;
                case 1:
                    newMod = Random.Range(1, 13);
                    if (!(isThisMelee) && newMod != 8 && newMod != comparison[1])
                    {
                        foundMod = true;
                    }
                    break;
                case 2:
                    newMod = Random.Range(1, 7);
                    if (!(isThisMelee) && newMod != 3 && newMod != 6 && newMod != comparison[1])
                    {
                        foundMod = true;
                    }
                    break;
                case 3:
                    newMod = Random.Range(1, 5);
                    if (!(isThisMelee) && newMod != 1 && newMod != 3 && newMod != comparison[1])
                    {
                        foundMod = true;
                    }
                    break;
                case 4:
                    //No mods for T4 yet, so.
                    break;
            }
        }
        return newMod;
    }

    public int modConfirmThree(int[] comparison, int[] comparisonTwo, int[] toChange, bool isThisMelee)
    {
        //Let's be nice and reroll to same tier.
        int modTier = comparison[0];
        bool foundMod = false;
        int newMod = toChange[1];
        bool sameTier = (comparisonTwo[0] == toChange[0]);
        bool sameModVal = (comparisonTwo[1] == newMod);
        while ((comparison[1] == newMod) && !foundMod)
        {
            switch (modTier)
            {
                case -1:
                    newMod = Random.Range(1, 9);
                    if (!(isThisMelee) && newMod != 7 && newMod != 8 && newMod != comparison[1])
                    {
                        if (sameTier)
                        {
                            if (newMod != comparisonTwo[1])
                            {
                                foundMod = true;
                            }
                        }
                        else
                        {
                            foundMod = true;
                        }
                    }
                    break;
                case 1:
                    newMod = Random.Range(1, 13);
                    if (!(isThisMelee) && newMod != 8 && newMod != comparison[1])
                    {
                        //If modTier == sameTier, make sure it's not sameModVal.
                        if (sameTier)
                        {
                            if (newMod != comparisonTwo[1])
                            {
                                foundMod = true;
                            }
                        }
                        else
                        {
                            foundMod = true;
                        }
                    }
                    break;
                case 2:
                    newMod = Random.Range(1, 8);
                    if (!(isThisMelee) && newMod != 3 && newMod != 6 && newMod != comparison[1])
                    {
                        if (sameTier)
                        {
                            if (newMod != comparisonTwo[1])
                            {
                                foundMod = true;
                            }
                        }
                        else
                        {
                            foundMod = true;
                        }
                    }
                    break;
                case 3:
                    newMod = Random.Range(1, 12);
                    if (!(isThisMelee) && newMod != 1 && newMod != 3 && newMod != comparison[1])
                    {
                        if (sameTier)
                        {
                            if (newMod != comparisonTwo[1])
                            {
                                foundMod = true;
                            }
                        }
                        else
                        {
                            foundMod = true;
                        }
                    }
                    break;
                case 4:
                    //No mods for T4 yet, so.
                    break;
            }
        }
        return newMod;
    }

    void applyMods(Item generatedGun)
    {
        for (int i = 0; i < generatedGun.mods.GetLength(0); i++)
        {
            switch (generatedGun.mods[i, 0])
            {
                case -1:
                    switch (generatedGun.mods[i, 1])
                    {
                        case 1:
                            //Feeble. Dmg down.
                            generatedGun.tempMinDmg--;
                            generatedGun.tempMaxDmg--;
                            break;
                        case 2:
                            //Flatfoot. Mvt down.
                            generatedGun.tempMvt--;
                            break;
                        case 3:
                            //Paperclad. Def down.
                            generatedGun.tempDef -= 5;
                            break;
                        case 4:
                            //Unaware. Eva down.
                            generatedGun.tempEva -= 5;
                            break;
                        case 5:
                            //Unlucky. Lck down.
                            generatedGun.tempLck -= 5;
                            break;
                        case 6:
                            //Uncalibrated. Acc down.
                            generatedGun.tempAcc -= 5;
                            break;
                        case 7:
                            //Ammo-.
                            if (generatedGun.clipSize > 1)
                            {
                                generatedGun.clipSize--;
                                generatedGun.currentClip--;
                            }
                            break;
                        case 8:
                            //Ammo--.
                            if (generatedGun.clipSize > 3)
                            {
                                generatedGun.clipSize -= 2;
                                generatedGun.currentClip = generatedGun.clipSize;
                            }
                            else
                            {
                                generatedGun.clipSize = 1;
                                generatedGun.currentClip = generatedGun.clipSize;
                            }
                            break;
                    }
                    break;
                case 1:
                    switch (generatedGun.mods[i, 1])
                    {
                        case 1:
                            //Fleetfoot. Spd up.
                            generatedGun.tempMvt++;
                            break;
                        case 2:
                            //Ironclad. Def up.
                            generatedGun.tempDef += 5;
                            break;
                        case 3:
                            //Aware. EVA up.
                            generatedGun.tempEva += 5;
                            break;
                        case 4:
                            //Lucky. LCK up.
                            generatedGun.tempLck += 5;
                            break;
                        case 8:
                            //Ammo+.
                            generatedGun.clipSize++;
                            generatedGun.currentClip = generatedGun.clipSize;
                            break;
                    }
                    break;
                case 2:
                    switch (generatedGun.mods[i, 1])
                    {
                        case 1:
                            //Brutal. Dmg+
                            generatedGun.tempMinDmg++;
                            generatedGun.tempMaxDmg++;
                            break;
                        case 2:
                            //Scope. Acc+
                            generatedGun.tempAcc += 5;
                            break;
                        case 6:
                            //Ammo++
                            generatedGun.clipSize += 2;
                            generatedGun.currentClip = generatedGun.clipSize;
                            break;
                    }
                    break;
                case 3:
                    //No stat modifiers here.
                    break;
            }
        }
    }

    void generatePotion(int matA, int matB, int matC, int matD)
    {
        int valSum = (matA + matB + matC + matD) / 2;
        if (valSum > 0)
        {
            //Make a potion.
            Item newPotion = Instantiate(template, transform.position, Quaternion.identity);
            newPotion.itemName = "Potion";
            newPotion.itemID = 1;
            newPotion.healAmt = valSum;
            string output = "Potion created.\nHeals for "+ valSum +" HP.";
            lastGeneratedGun = null;
            basicGunData = output;
            InvManager.im.convoy.Add(newPotion);
            newPotion.transform.parent = InvManager.im.transform;
            newPotion.useSound = potionSound;
        }
        else
        {
            string output = "Item creation failed.\nInsufficient resources.";
            lastGeneratedGun = null;
            basicGunData = output;
            Controller.c.playSound(Controller.c.sfx[10], .25f);
        }
    }
}
