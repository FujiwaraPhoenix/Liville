using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionInfo : MonoBehaviour
{
    public int missionNo;
    public string missionName, missionAbstract, missionForecast;
    public Image infoBG;
    public Sprite bgDef, bgHL;
    public Text mNameDis;
    public bool preloaded = false;
    

    // Start is called before the first frame update
    void Start()
    {
       if (!preloaded)
        {
            loadInfo();
            preloaded = !preloaded;
        } 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void loadInfo()
    {
        string pathname = "Assets/Text Files/Map Select/Mission" + missionNo + ".txt";
        TextFileParser.tfp.readString(pathname);
        string[] data = TextFileParser.tfp.itemList;
        missionName = data[0];
        missionAbstract = data[1];
        missionForecast = data[2];
        mNameDis.text = missionName;
    }
}
