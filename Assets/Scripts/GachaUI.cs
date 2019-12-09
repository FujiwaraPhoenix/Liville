using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GachaUI : MonoBehaviour
{
    public static GachaUI gaui;

    public GameObject menuPointer;
    public bool modifyingValue = false;
    //Values are from 0 -> 3, then 4 is 'go'.
    public int currentModdedValue = 0;
    public int matAVal, matBVal, matCVal, matDVal;
    public Text matADisplay, matBDisplay, matCDisplay, matDDisplay;
    public Gacha generator;
    public Sprite selectDefault, selectChosen, selectUp, selectDown;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Determine the current value being modified.
        if (modifyingValue)
        {
            switch (currentModdedValue)
            {
                case 0:
                    if (Input.GetKeyDown(KeyCode.UpArrow))
                    {

                    }
                    if (Input.GetKeyDown(KeyCode.DownArrow))
                    {

                    }
                    if (Input.GetKeyDown(KeyCode.LeftArrow))
                    {

                    }
                    if (Input.GetKeyDown(KeyCode.RightArrow))
                    {

                    }
                    break;

                case 1:
                    break;

                case 2:
                    break;

                case 3:
                    break;

                case 4:
                    //Generate gacha gun. Uwee hee hee.
                    //Confirmation message here.
                    break;
            }
        }
        //Cycle. Add selection mechanism.
        else
        {
            cycleValues();
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (!modifyingValue)
            {
                modifyingValue = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (modifyingValue)
            {
                modifyingValue = false;
            }
        }
    }

    public void cycleValues()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentModdedValue == 0)
            {
                currentModdedValue = 4;
            }
            else
            {
                currentModdedValue--;
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentModdedValue == 4)
            {
                currentModdedValue = 0;
            }
            else
            {
                currentModdedValue++;
            }
        }
    }
}
