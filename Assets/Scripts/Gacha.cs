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
    public Item lastGeneratedGun;
    public string basicGunData;

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
        Item newItem = Instantiate(template, transform.position, Quaternion.identity);
        //This is always a gun.
        newItem.itemID = 0;

        //Placeholder stuff; output string.
        string output = "Output\n";
        string outputRarity = "";
        string outputType = "";
        string mods = "";
        string[] modList = null;
        newItem.rarity = gunRarity;
        switch (gunType)
        {
            case 0:
                outputType = "Assault Rifle\n";
                generateAR(gunRarity, newItem);
                newItem.itemType = "Assault Rifle";
                break;
            case 1:
                outputType = "Shotgun\n";
                generateSG(gunRarity, newItem);
                newItem.itemType = "Shotgun";
                break;
            case 2:
                outputType = "Handgun\n";
                generateHG(gunRarity, newItem);
                newItem.itemType = "Handgun";
                break;
            case 3:
                outputType = "Sniper Rifle\n";
                generateRF(gunRarity, newItem);
                newItem.itemType = "Sniper Rifle";
                break;
            case 4:
                outputType = "Sword\n";
                generateSWD(gunRarity, newItem);
                newItem.itemType = "Sword";
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
            output += outputRarity + outputType + "Mods:\n" + mods;
        }
        else
        {
            output += outputRarity + outputType;
        }
        Debug.Log(output);
        InvManager.im.convoy.Add(newItem);
        newItem.transform.parent = InvManager.im.transform;
        lastGeneratedGun = newItem;
        basicGunData = output;
    }

    //This generates the mods
    //TODO: Finish this.
    public void generateMods(int weaponRarity, string[] modList, bool isMelee, Item moddedItem)
    {
        int modCountChance = Random.Range(0, 100);
        int modCount = 0;
        switch (weaponRarity)
        {
            //Here, we determine the number of mods to be generated on a given gun.
            case 1:
                //Common gun.
                if (modCountChance < 30)
                {
                    modCount = 0;
                }
                else if (modCountChance < 80)
                {
                    modCount = 1;
                }
                else if (modCountChance < 90)
                {
                    modCount = 2;
                }
                else if (modCountChance < 99)
                {
                    modCount = 3;
                }
                else
                {
                    modCount = 4;
                }
                break;
            case 2:
                //Uncommon gun.
                if (modCountChance < 10)
                {
                    modCount = 0;
                }
                else if (modCountChance < 45)
                {
                    modCount = 1;
                }
                else if (modCountChance < 74)
                {
                    modCount = 2;
                }
                else if (modCountChance < 90)
                {
                    modCount = 3;
                }
                else
                {
                    modCount = 4;
                }
                break;

            case 3:
                //Rare gun.
                if (modCountChance < 5)
                {
                    modCount = 0;
                }
                else if (modCountChance < 35)
                {
                    modCount = 1;
                }
                else if (modCountChance < 75)
                {
                    modCount = 2;
                }
                else if (modCountChance < 90)
                {
                    modCount = 3;
                }
                else
                {
                    modCount = 4;
                }
                break;

            case 4:
                //Super rare gun.
                if (modCountChance < 5)
                {
                    modCount = 0;
                }
                else if (modCountChance < 25)
                {
                    modCount = 1;
                }
                else if (modCountChance < 60)
                {
                    modCount = 2;
                }
                else if (modCountChance < 80)
                {
                    modCount = 3;
                }
                else
                {
                    modCount = 4;
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
                            if (modRarity < 75)
                            {
                                modTier = 1;
                            }
                            else if (modRarity < 90)
                            {
                                modTier = 2;
                            }
                            else if (modRarity < 99)
                            {
                                modTier = 3;
                            }
                            else
                            {
                                modTier = 4;
                            }
                            break;
                        case 2:
                            if (modRarity < 55)
                            {
                                modTier = 1;
                            }
                            else if (modRarity < 80)
                            {
                                modTier = 2;
                            }
                            else if (modRarity < 95)
                            {
                                modTier = 3;
                            }
                            else
                            {
                                modTier = 4;
                            }
                            break;
                        case 3:
                            if (modRarity < 50)
                            {
                                modTier = 1;
                            }
                            else if (modRarity < 75)
                            {
                                modTier = 2;
                            }
                            else if (modRarity < 90)
                            {
                                modTier = 3;
                            }
                            else
                            {
                                modTier = 4;
                            }
                            break;
                        case 4:
                            if (modRarity < 40)
                            {
                                modTier = 1;
                            }
                            else if (modRarity < 65)
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
                    }
                    //Now we generate the mod proper.
                    switch (modTier)
                    {
                        case 1:
                            modIndex = Random.Range(1, 9);
                            if (!isMelee && modIndex != 8)
                            {
                                foundMod = true;
                            }
                            break;
                        case 2:
                            modIndex = Random.Range(1, 8);
                            if (!isMelee && modIndex != 3 && modIndex != 6 && modIndex != 7)
                            {
                                foundMod = true;
                            }
                            break;
                        case 3:
                            modIndex = Random.Range(1, 12);
                            if (!isMelee && modIndex != 1 && modIndex != 3 && modIndex != 10)
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
                moddedItem.range = Random.Range(1, 4);
                moddedItem.accuracy = Random.Range(50, 81);
                moddedItem.clipSize = Random.Range(2, 4);
                break;
            case 2:
                //Uncommon
                moddedItem.minDmg = Random.Range(1, 3);
                moddedItem.maxDmg = Random.Range(3, 6);
                moddedItem.range = Random.Range(2, 4);
                moddedItem.accuracy = Random.Range(65, 86);
                moddedItem.clipSize = 3;
                break;
            case 3:
                //Rare
                moddedItem.minDmg = Random.Range(2, 4);
                moddedItem.maxDmg = Random.Range(3, 6);
                moddedItem.range = Random.Range(2, 4);
                moddedItem.accuracy = Random.Range(70, 91);
                moddedItem.clipSize = 3;
                break;
            case 4:
                //Super Rare
                moddedItem.minDmg = 3;
                moddedItem.maxDmg = Random.Range(4, 6);
                moddedItem.range = Random.Range(2, 5);
                moddedItem.accuracy = Random.Range(75, 91);
                moddedItem.clipSize = Random.Range(3, 5);
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
                moddedItem.minDmg = Random.Range(2, 3);
                moddedItem.maxDmg = Random.Range(3, 6);
                moddedItem.range = 1;
                moddedItem.accuracy = Random.Range(50, 71);
                moddedItem.clipSize = Random.Range(2, 4);
                break;
            case 2:
                //Uncommon
                moddedItem.minDmg = 2;
                moddedItem.maxDmg = Random.Range(4, 6);
                moddedItem.range = Random.Range(1, 3);
                moddedItem.accuracy = Random.Range(65, 76);
                moddedItem.clipSize = 3;
                break;
            case 3:
                //Rare
                moddedItem.minDmg = 2;
                moddedItem.maxDmg = Random.Range(4, 6);
                moddedItem.range = 2;
                moddedItem.accuracy = Random.Range(70, 81);
                moddedItem.clipSize = 2;
                break;
            case 4:
                //Super Rare
                moddedItem.minDmg = 3;
                moddedItem.maxDmg = Random.Range(4, 6);
                moddedItem.range = Random.Range(2, 4);
                moddedItem.accuracy = Random.Range(75, 86);
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
                moddedItem.accuracy = Random.Range(50, 81);
                moddedItem.clipSize = Random.Range(3, 5);
                break;
            case 2:
                //Uncommon
                moddedItem.minDmg = Random.Range(1, 4);
                moddedItem.maxDmg = Random.Range(2, 5);
                moddedItem.range = Random.Range(2, 4);
                moddedItem.accuracy = Random.Range(65, 86);
                moddedItem.clipSize = Random.Range(3, 5);
                break;
            case 3:
                //Rare
                moddedItem.minDmg = Random.Range(2, 4);
                moddedItem.maxDmg = Random.Range(3, 5);
                moddedItem.range = Random.Range(2, 4);
                moddedItem.accuracy = Random.Range(70, 91);
                moddedItem.clipSize = 4;
                break;
            case 4:
                //Super Rare
                moddedItem.minDmg = Random.Range(2, 4);
                moddedItem.maxDmg = Random.Range(3, 6);
                moddedItem.range = Random.Range(2, 5);
                moddedItem.accuracy = Random.Range(75, 91);
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
                moddedItem.accuracy = Random.Range(50, 81);
                moddedItem.clipSize = Random.Range(1, 3);
                break;
            case 2:
                //Uncommon
                moddedItem.minDmg = Random.Range(2, 4);
                moddedItem.maxDmg = Random.Range(4, 6);
                moddedItem.range = Random.Range(3, 5);
                moddedItem.accuracy = Random.Range(65, 86);
                moddedItem.clipSize = Random.Range(1, 3);
                break;
            case 3:
                //Rare
                moddedItem.minDmg = 3;
                moddedItem.maxDmg = Random.Range(4, 7);
                moddedItem.range = Random.Range(3, 6);
                moddedItem.accuracy = Random.Range(70, 91);
                moddedItem.clipSize = 2;
                break;
            case 4:
                //Super Rare
                moddedItem.minDmg = 3;
                moddedItem.maxDmg = Random.Range(5, 7);
                moddedItem.range = Random.Range(4, 6);
                moddedItem.accuracy = Random.Range(75, 91);
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
                moddedItem.minDmg = Random.Range(2, 4);
                moddedItem.maxDmg = Random.Range(3, 6);
                moddedItem.range = 1;
                moddedItem.accuracy = Random.Range(50, 71);
                break;
            case 2:
                //Uncommon
                moddedItem.minDmg = 2;
                moddedItem.maxDmg = Random.Range(4, 6);
                moddedItem.range = 1;
                moddedItem.accuracy = Random.Range(65, 76);
                break;
            case 3:
                //Rare
                moddedItem.minDmg = 2;
                moddedItem.maxDmg = Random.Range(4, 6);
                moddedItem.range = 1;
                moddedItem.accuracy = Random.Range(70, 81);
                break;
            case 4:
                //Super Rare
                moddedItem.minDmg = 3;
                moddedItem.maxDmg = Random.Range(4, 6);
                moddedItem.range = Random.Range(1, 3);
                moddedItem.accuracy = Random.Range(75, 86);
                break;
        }
        moddedItem.currentClip = 1;
        moddedItem.clipSize = 1;
        moddedItem.isMelee = true;
    }



    /*public void generateGun()
    {
        minDmg = Random.Range(2, 5);
        maxDmg = Random.Range(minDmg, minDmg + 5);
        //Maintain clip size for now
        accuracy = Random.Range(75, 100);
        range = Random.Range(2, 4);
        //Mods are set up in a 2d array, 3x2. First number is the mod rarity, second is the mod ID of that given rarity.
        mods[0,0] = Random.Range(1, 4);
        switch (mods[0, 0])
        {
            case 1:
                mods[0, 1] = Random.Range(1, 9);
                switch (mods[0, 1])
                {
                    case 1:
                        //Fleetfoot: Spd+
                        tempSpd = 5;
                        break;
                    case 2:
                        //Ironclad: Def+
                        tempDef = 5;
                        break;
                    case 3:
                        //Aware: Eva+
                        tempEva = 5;
                        break;
                    case 4:
                        //Lucky: Lck+
                        tempLck = 5;
                        break;
                    case 5:
                        //Resistant: Res+
                        tempRes = 10;
                        break;
                    case 8:
                        //Ammo Capacity+: 1.5x Cap
                        clipSize = (int) (clipSize * 1.5f);
                        currentClip = clipSize;
                        break;
                }
                break;
            case 2:
                mods[0, 1] = Random.Range(1, 8);
                switch (mods[0, 1])
                {
                    case 1:
                        //Brutal: Dmg+
                        tempMinDmg = 5;
                        tempMaxDmg = 5;
                        break;
                    case 2:
                        //Scope: Range+
                        range += 1;
                        break;
                    case 6:
                        //Ammo Capacity++: 2x Cap
                        clipSize = (int)(clipSize * 2f);
                        currentClip = clipSize;
                        break;
                    case 7:
                        //Ammo Capacity-: .75x Cap
                        clipSize = (int)(clipSize * .75f);
                        currentClip = clipSize;
                        break;
                }
                break;
            case 3:
                mods[0, 1] = Random.Range(1, 12);
                switch (mods[0, 1])
                {
                    case 4:
                        //Gentle: Dmg-
                        tempMinDmg = -2;
                        tempMaxDmg = -2;
                        break;
                    case 5:
                        //Flatfoot: Spd-
                        tempSpd = -2;
                        break;
                    case 6:
                        //Paperclad: Def-
                        tempDef = -2;
                        break;
                    case 7:
                        //Unaware: Eva-
                        tempEva = -2;
                        break;
                    case 8:
                        //Unlucky: Lck-
                        tempLck = -2;
                        break;
                    case 9:
                        //Uncalibrated: Rng-
                        range += -1;
                        break;
                    case 10:
                        //Ammo Capacity--: .5x Cap
                        clipSize = (int)(clipSize * .5f);
                        currentClip = clipSize;
                        break;
                }
                break;
        }
        if (Random.Range(0,100) < 5)
        {
            mods[1, 0] = Random.Range(1, 4);
            switch (mods[1, 0])
            {
                case 1:
                    mods[1, 1] = Random.Range(1, 9);
                    switch (mods[1, 1])
                    {
                        case 1:
                            //Fleetfoot: Spd+
                            tempSpd = 5;
                            break;
                        case 2:
                            //Ironclad: Def+
                            tempDef = 5;
                            break;
                        case 3:
                            //Aware: Eva+
                            tempEva = 5;
                            break;
                        case 4:
                            //Lucky: Lck+
                            tempLck = 5;
                            break;
                        case 5:
                            //Resistant: Res+
                            tempRes = 10;
                            break;
                        case 8:
                            //Ammo Capacity+: 1.5x Cap
                            clipSize = (int)(clipSize * 1.5f);
                            currentClip = clipSize;
                            break;
                    }
                    break;
                case 2:
                    mods[1, 1] = Random.Range(1, 8);
                    switch (mods[1, 1])
                    {
                        case 1:
                            //Brutal: Dmg+
                            tempMinDmg = 5;
                            tempMaxDmg = 5;
                            break;
                        case 2:
                            //Scope: Range+
                            range += 1;
                            break;
                        case 6:
                            //Ammo Capacity++: 2x Cap
                            clipSize = (int)(clipSize * 2f);
                            currentClip = clipSize;
                            break;
                        case 7:
                            //Ammo Capacity-: .75x Cap
                            clipSize = (int)(clipSize * .75f);
                            currentClip = clipSize;
                            break;
                    }
                    break;
                case 3:
                    mods[1, 1] = Random.Range(1, 12);
                    switch (mods[1, 1])
                    {
                        case 4:
                            //Gentle: Dmg-
                            tempMinDmg = -2;
                            tempMaxDmg = -2;
                            break;
                        case 5:
                            //Flatfoot: Spd-
                            tempSpd = -2;
                            break;
                        case 6:
                            //Paperclad: Def-
                            tempDef = -2;
                            break;
                        case 7:
                            //Unaware: Eva-
                            tempEva = -2;
                            break;
                        case 8:
                            //Unlucky: Lck-
                            tempLck = -2;
                            break;
                        case 9:
                            //Uncalibrated: Rng-
                            range += -1;
                            break;
                        case 10:
                            //Ammo Capacity--: .5x Cap
                            clipSize = (int)(clipSize * .5f);
                            currentClip = clipSize;
                            break;
                    }
                    break;
            }
        }
    }*/
}
