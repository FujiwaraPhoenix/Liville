using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapSelect : MonoBehaviour
{
    public static MapSelect ms;
    public MissionInfo[] availableMissions = new MissionInfo[3];
    public int currentChoice = 0;
    public Text missionDetails, missionForecast, missionName, missionCap;
    public Sprite def, hl;

    void Awake()
    {
        if (ms == null)
        {
            DontDestroyOnLoad(gameObject);
            ms = this;
        }
        else if (ms != this)
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
        navigateMenu();
    }

    void navigateMenu()
    {
        if (Controller.c.gameMode == 1)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow)){
                availableMissions[currentChoice].infoBG.sprite = def;
                if (currentChoice == 0)
                {
                    currentChoice = 2;
                }
                else
                {
                    currentChoice--;
                }
                Controller.c.playSound(Controller.c.sfx[0], .25f);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow)) {
                availableMissions[currentChoice].infoBG.sprite = def;
                if (currentChoice == 2)
                {
                    currentChoice = 0;
                }
                else
                {
                    currentChoice++;
                }
                Controller.c.playSound(Controller.c.sfx[0], .25f);
            }
            updateDetails();
        }
    }

    void updateDetails()
    {
        missionDetails.text = availableMissions[currentChoice].missionAbstract;
        missionName.text = availableMissions[currentChoice].missionName;
        missionForecast.text = availableMissions[currentChoice].missionForecast;
        missionCap.text = availableMissions[currentChoice].playerCap;
        availableMissions[currentChoice].infoBG.sprite = hl;
    }
}
