using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TextFileParser : MonoBehaviour {
    //Static instance: THERE CAN ONLY BE ONE.
    public static TextFileParser tfp;

    //For storing what we read from a string/output for other scripts.
    public string heldData;
    public string[] itemList;

    //Because Assets doesn't exist during runtime..
    public TextAsset[] mapList;
    public TextAsset[] missionDataList;
    public TextAsset[] missionSelectList;
    public TextAsset namePrefixes, nameSuffixes;

    //This is temporary for the sake of testing. Wipe this when we start for real.
    private void Start()
    {

    }

    void Awake()
    {
        if (tfp == null)
        {
            DontDestroyOnLoad(gameObject);
            tfp = this;
        }
        else if (tfp != this)
        {
            Destroy(gameObject);
        }
    }

    //Exactly what it sounds like. We read a string and... Well, turn the output into a list for us to use.
    public void readString(string pathName)
    {
        StreamReader reader = new StreamReader(pathName);
        heldData = reader.ReadToEnd();
        reader.Close();
        tStringToList();
    }

    //This is just to make things more readable. We take the string and split it. Easy.
    public void tStringToList()
    {
        itemList = heldData.Split(new string[] { "\r\n", "\n" }, System.StringSplitOptions.None);
    }

    //Reads a map.
    public int[,] readMap(int mapNo)
    {
        //string mapName = "Assets/Text Files/Map" + mapNo + ".csv";
        //readString(mapName);
        heldData = mapList[mapNo - 1].text;
        //heldData is now the map. Let's turn this into a 2D array.
        itemList = heldData.Split(new string[] { "\r\n", "\n" }, System.StringSplitOptions.None);
        int[,] output = new int[10, 10];
        for (int i = 0; i < 10; i++)
        {
            string[] tempList = itemList[i].Split(new string[] { "," }, System.StringSplitOptions.None);
            for (int j = 0; j < 10; j++)
            {
                output[i, j] = int.Parse(tempList[j]);
            }
        }
        //Debug.Log("Map read!");
        return output;
    }

    //Reads a text file for information on a given mission.
    public void readMissionData(int missionID)
    {
        //We're splitting the data with new lines. It might get a bit painful to read, but that's a sacrifice I'm willing to take.
        /*string missionName = "Assets/Text Files/Mission" + missionID + ".txt";
        readString(missionName);*/
        heldData = missionDataList[missionID].text;
        //heldData is now what we just read from the mission file. Let's split that.
        itemList = heldData.Split(new string[] { "\n", "\r\n" }, System.StringSplitOptions.None);
        //So, in order; string 0 is the map ID, followed by the max player count.
        //String 1 values are the possible starting locations of the player units.
        //String 2 are the values of the gacha resources to be obtained upon mission completion.
        //String 3 is the enemy count. Slightly useful.
        //String 4 and on are the enemy variants, followed by coordinates, then stats.
        //First is the variant type, then coordinates. 3 values.
        //These stats are, in order: HP, Evasion, Def, Luck, Mvt, Status Resist. 6 in all.
        //Weapon stats: min dmg, max dmg, clip size, accuracy, range, heal amount, temp mvt/def/eva/lck/res/min/max (if needed). 13 in total.
    }

    public void loadResources()
    {
        string[] tempHolder = itemList[2].Split(new string[] { "," }, System.StringSplitOptions.None);
        int[] convertedValues = new int[4];
        for (int i = 0; i < 4; i++)
        {
            convertedValues[i] = int.Parse(tempHolder[i]);
        }
        Controller.c.materialAGain = convertedValues[0];
        Controller.c.materialBGain = convertedValues[1];
        Controller.c.materialCGain = convertedValues[2];
        Controller.c.materialDGain = convertedValues[3];
    }

    public int[,] availablePlayerCoords(int playerCount)
    {
        string[] tempHolder = itemList[1].Split(new string[] { " " }, System.StringSplitOptions.None);
        int[,] output = new int[playerCount, 2];
        for (int i = 0; i < tempHolder.Length; i++)
        {
            string[] tempHolder2 = tempHolder[i].Split(new string[] { "," }, System.StringSplitOptions.None);
            output[i, 0] = int.Parse(tempHolder2[0]);
            output[i, 1] = int.Parse(tempHolder2[1]);
        }
        return output;
    }

    public int[] numbersToUnitValues(string input)
    {
        //Splits the data up.
        string[] tempHolder = input.Split(new string[] { "," }, System.StringSplitOptions.None);
        int[] valHolder = new int[23];
        for (int i = 0; i < tempHolder.Length; i++)
        {
            valHolder[i] = int.Parse(tempHolder[i]);
        }
        return valHolder;
    }


    public int playerCap()
    {
        string[] tempA = itemList[0].Split(new string[] { "," }, System.StringSplitOptions.None);
        return int.Parse(tempA[1]);
    }

}
