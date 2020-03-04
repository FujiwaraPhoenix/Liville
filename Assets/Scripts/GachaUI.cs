using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GachaUI : MonoBehaviour
{
    public static GachaUI gaUI;

    public GameObject menuPointer;
    public bool modifyingValue = false;
    //Values are from 0 -> 3, then 4 is 'go'.
    public int currentModdedValue = 0;
    public int matAVal, matBVal, matCVal, matDVal;
    public Text[] matInputDisplays = new Text[4];
    public Text[] matOwnedDisplays = new Text[4];
    public Image[] inputObjects = new Image[4];
    public Gacha generator;
    public bool confirm = false;
    public Sprite selectDefault, selectChosen, selectUp, selectDown;
    public float delayTimer, frameCounter;
    public float stallSpeed = .5f;
    public int frameDelay = 5;
    public Gacha gachaMachine;

    public Sprite[] inputIndicSprites = new Sprite[4];

    public Text itemOutput;

    void Awake()
    {
        if (gaUI == null)
        {
            DontDestroyOnLoad(gameObject);
            gaUI = this;
        }
        else if (gaUI != this)
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
        if (Controller.c.gameMode == 3)
        {
            updateUIValues();
            //Determine the current value being modified.
            if (modifyingValue)
            {
                updateValues();
            }
            //Cycle. Add selection mechanism.
            else
            {
                cycleValues();
            }
            updateSprites();
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

        if (Input.GetKeyDown(KeyCode.Z))
        {
            modifyingValue = true;
        }
    }

    public void updateUIValues()
    {
        //Update owned amount
        matOwnedDisplays[0].text = InvManager.im.materialA.ToString();
        matOwnedDisplays[1].text = InvManager.im.materialB.ToString();
        matOwnedDisplays[2].text = InvManager.im.materialC.ToString();
        matOwnedDisplays[3].text = InvManager.im.materialD.ToString();
        //Updates input amount.
        matInputDisplays[0].text = matAVal.ToString();
        matInputDisplays[1].text = matBVal.ToString();
        matInputDisplays[2].text = matCVal.ToString();
        matInputDisplays[3].text = matDVal.ToString();
    }

    public void updateValues()
    {
        switch (currentModdedValue)
        {
            case 0:
                if (Input.GetKeyDown(KeyCode.UpArrow) && matAVal < InvManager.im.materialA && matAVal < 100)
                {
                    matAVal++;
                    delayTimer = 0;
                }
                if (Input.GetKey(KeyCode.UpArrow) && matAVal < InvManager.im.materialA && matAVal < 100)
                {
                    if (delayTimer < stallSpeed)
                    {
                        delayTimer += Time.deltaTime;
                    }
                    else
                    {
                        if (frameCounter < frameDelay)
                        {
                            frameCounter++;
                        }
                        else
                        {
                            frameCounter = 0;
                            matAVal++;
                        }
                    }
                }
                if (Input.GetKeyDown(KeyCode.DownArrow) && matAVal > 0)
                {
                    matAVal--;
                    delayTimer = 0;
                }
                if (Input.GetKey(KeyCode.DownArrow) && matAVal > 0)
                {
                    if (delayTimer < stallSpeed)
                    {
                        delayTimer += Time.deltaTime;
                    }
                    else
                    {
                        if (frameCounter < frameDelay)
                        {
                            frameCounter++;
                        }
                        else
                        {
                            frameCounter = 0;
                            matAVal--;
                        }
                    }
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    if (matAVal < 10)
                    {
                        matAVal = 0;
                    }
                    else
                    {
                        matAVal -= 10;
                    }
                }
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    if (matAVal + 10 > InvManager.im.materialA)
                    {
                        matAVal = InvManager.im.materialA;
                    }
                    else if (matAVal > 90)
                    {
                        matAVal = 100;
                    }
                    else
                    {
                        matAVal += 10;
                    }
                }
                break;

            case 1:
                if (Input.GetKeyDown(KeyCode.UpArrow) && matBVal < InvManager.im.materialB && matBVal < 100)
                {
                    matBVal++;
                    delayTimer = 0;
                }
                if (Input.GetKey(KeyCode.UpArrow) && matBVal < InvManager.im.materialB && matBVal < 100)
                {
                    if (delayTimer < stallSpeed)
                    {
                        delayTimer += Time.deltaTime;
                    }
                    else
                    {
                        if (frameCounter < frameDelay)
                        {
                            frameCounter++;
                        }
                        else
                        {
                            frameCounter = 0;
                            matBVal++;
                        }
                    }
                }
                if (Input.GetKeyDown(KeyCode.DownArrow) && matBVal > 0)
                {
                    matBVal--;
                    delayTimer = 0;
                }
                if (Input.GetKey(KeyCode.DownArrow) && matBVal > 0)
                {
                    if (delayTimer < stallSpeed)
                    {
                        delayTimer += Time.deltaTime;
                    }
                    else
                    {
                        if (frameCounter < frameDelay)
                        {
                            frameCounter++;
                        }
                        else
                        {
                            frameCounter = 0;
                            matBVal--;
                        }
                    }
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    if (matBVal < 10)
                    {
                        matBVal = 0;
                    }
                    else
                    {
                        matBVal -= 10;
                    }
                }
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    if (matBVal + 10 > InvManager.im.materialB)
                    {
                        matBVal = InvManager.im.materialB;
                    }
                    else if (matBVal > 90)
                    {
                        matBVal = 100;
                    }
                    else
                    {
                        matBVal += 10;
                    }
                }
                break;

            case 2:
                if (Input.GetKeyDown(KeyCode.UpArrow) && matCVal < InvManager.im.materialC && matCVal < 100)
                {
                    matCVal++;
                    delayTimer = 0;
                }
                if (Input.GetKey(KeyCode.UpArrow) && matCVal < InvManager.im.materialC && matCVal < 100)
                {
                    if (delayTimer < stallSpeed)
                    {
                        delayTimer += Time.deltaTime;
                    }
                    else
                    {
                        if (frameCounter < frameDelay)
                        {
                            frameCounter++;
                        }
                        else
                        {
                            frameCounter = 0;
                            matCVal++;
                        }
                    }
                }
                if (Input.GetKeyDown(KeyCode.DownArrow) && matCVal > 0)
                {
                    matCVal--;
                    delayTimer = 0;
                }
                if (Input.GetKey(KeyCode.DownArrow) && matCVal > 0)
                {
                    if (delayTimer < stallSpeed)
                    {
                        delayTimer += Time.deltaTime;
                    }
                    else
                    {
                        if (frameCounter < frameDelay)
                        {
                            frameCounter++;
                        }
                        else
                        {
                            frameCounter = 0;
                            matCVal--;
                        }
                    }
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    if (matCVal < 10)
                    {
                        matCVal = 0;
                    }
                    else
                    {
                        matCVal -= 10;
                    }
                }
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    if (matCVal + 10 > InvManager.im.materialC)
                    {
                        matCVal = InvManager.im.materialC;
                    }
                    else if (matCVal > 90)
                    {
                        matCVal = 100;
                    }
                    else
                    {
                        matCVal += 10;
                    }
                }
                break;

            case 3:
                if (Input.GetKeyDown(KeyCode.UpArrow) && matDVal < InvManager.im.materialD && matDVal < 100)
                {
                    matDVal++;
                    delayTimer = 0;
                }
                if (Input.GetKey(KeyCode.UpArrow) && matDVal < InvManager.im.materialD && matDVal < 100)
                {
                    if (delayTimer < stallSpeed)
                    {
                        delayTimer += Time.deltaTime;
                    }
                    else
                    {
                        if (frameCounter < frameDelay)
                        {
                            frameCounter++;
                        }
                        else
                        {
                            frameCounter = 0;
                            matDVal++;
                        }
                    }
                }
                if (Input.GetKeyDown(KeyCode.DownArrow) && matDVal > 0)
                {
                    matDVal--;
                    delayTimer = 0;
                }
                if (Input.GetKey(KeyCode.DownArrow) && matDVal > 0)
                {
                    if (delayTimer < stallSpeed)
                    {
                        delayTimer += Time.deltaTime;
                    }
                    else
                    {
                        if (frameCounter < frameDelay)
                        {
                            frameCounter++;
                        }
                        else
                        {
                            frameCounter = 0;
                            matDVal--;
                        }
                    }
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    if (matDVal < 10)
                    {
                        matDVal = 0;
                    }
                    else
                    {
                        matDVal -= 10;
                    }
                }
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    if (matDVal + 10 > InvManager.im.materialD)
                    {
                        matDVal = InvManager.im.materialD;
                    }
                    else if (matDVal > 90)
                    {
                        matDVal = 100;
                    }
                    else
                    {
                        matDVal += 10;
                    }
                }
                break;

            case 4:
                //Generate gacha gun. Uwee hee hee.
                //Confirmation message here.
                break;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            modifyingValue = false;
            Debug.Log("No longer modifying.");
        }
    }

    //Updates the pointer's position and highlights any selected input value elements.
    public void updateSprites()
    {
        switch (currentModdedValue)
        {
            case 0:
                menuPointer.transform.localPosition = new Vector3(-285, 100, 0);
                break;
            case 1:
                menuPointer.transform.localPosition = new Vector3(-285, 45, 0);
                break;
            case 2:
                menuPointer.transform.localPosition = new Vector3(-285, -10, 0);
                break;
            case 3:
                menuPointer.transform.localPosition = new Vector3(-285, -65, 0);
                break;
            case 4:
                menuPointer.transform.localPosition = new Vector3(-125, -150, 0);
                break;
        }
        if (modifyingValue && currentModdedValue != 4)
        {
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.RightArrow))
            {
                inputObjects[currentModdedValue].sprite = selectUp;
            }
            else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftArrow))
            {
                inputObjects[currentModdedValue].sprite = selectDown;
            }
            else
            {
                inputObjects[currentModdedValue].sprite = selectChosen;
            }
        }
        if (Input.GetKeyDown(KeyCode.X) && currentModdedValue != 4)
        {
            inputObjects[currentModdedValue].sprite = selectDefault;
        }
        if (Input.GetKeyDown(KeyCode.Z) && currentModdedValue == 4)
        {
            //Make a gun.
            if (matAVal <= InvManager.im.materialA && matBVal <= InvManager.im.materialB && matCVal <= InvManager.im.materialC && matDVal <= InvManager.im.materialD)
            {
                gachaMachine.generateItem(matAVal, matBVal, matCVal, matDVal);
                modifyingValue = false;
                //Update the text.
                itemOutput.text = gachaMachine.basicGunData;
                InvManager.im.materialA -= matAVal;
                InvManager.im.materialB -= matBVal;
                InvManager.im.materialC -= matCVal;
                InvManager.im.materialD -= matDVal;
                if (InvManager.im.materialA < matAVal)
                {
                    matAVal = InvManager.im.materialA;
                }
                if (InvManager.im.materialB < matBVal)
                {
                    matBVal = InvManager.im.materialB;
                }
                if (InvManager.im.materialC < matCVal)
                {
                    matCVal = InvManager.im.materialC;
                }
                if (InvManager.im.materialD < matDVal)
                {
                    matDVal = InvManager.im.materialD;
                }
            }
        }
    }
}
