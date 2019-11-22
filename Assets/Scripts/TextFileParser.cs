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

    //This is temporary for the sake of testing. Wipe this when we start for real.
    private void Start()
    {
        readString("Assets/Text Files/ModNames.txt");
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
    void readString(string pathName)
    {
        StreamReader reader = new StreamReader(pathName);
        heldData = reader.ReadToEnd();
        reader.Close();
        tStringToList();
    }

    //This is just to make things more readable. We take the string and split it. Easy.
    void tStringToList()
    {
        itemList = heldData.Split(new string[] { "\r\n", "\n" }, System.StringSplitOptions.None);
    }

    //Reads a map.
    public int[,] readMap(int mapNo)
    {
        string mapName = "Assets/Text Files/Map" + mapNo + ".csv";
        readString(mapName);
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
}
