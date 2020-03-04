using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutUI : MonoBehaviour
{
    public static LoadoutUI lUI;

    //Determines which loadout stuff to run. 0 = default, 1 = char select, 2 = gun select, 3 = inventory mgmt
    public int currentLoadoutMenu;

    //Which character are we looking at?
    public int characterID;

    //X/Y coords
    public int currentX, currentY;

    //Limits on X/Y
    public int xCap = 2;
    public int yCap = 3;

    public int xHardCap = 3;
    public int yHardCap = 4;

    //UI stuff holders.
    public GameObject baseMenu, charMenu, invMenu, gunMenu;
    //In order: playerBGs, invs, next.
    public Sprite[] baseLoadoutSprList = new Sprite[6];
    public Sprite[] gunEquippedBorders = new Sprite[5];
    public Sprite[] gunEquippedBordersHL = new Sprite[5];

    //For base menu; all GameObjects present here.
    public Image[] baseLoadoutPlayers = new Image[4];
    public Image[] baseLoadoutInvs = new Image[4];
    public GunInfo[] baseLoadoutGuns = new GunInfo[4];
    public Image nextButton;

    //For gun select; need 2D array of guns
    public LoadoutGunMini template;
    public LoadoutGunMini[,] gunListing = new LoadoutGunMini[3, 4];

    public GameObject displayBG;

    //For gun select; what page are we on?
    public int gunPage = 1;
    //And the cap?
    public int totalPages;

    //Whose gun are we fiddling with?
    public int currentPlayer;

    //Contains all borders.
    public Sprite[] borderList = new Sprite[9];

    //Molecules for the detailed stat listings
    //For the gun swap screen:
    public GunInfo equipped, hovered;

    bool gunSelectLoaded = false;

    void Awake()
    {
        if (lUI == null)
        {
            DontDestroyOnLoad(gameObject);
            lUI = this;
        }
        else if (lUI != this)
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
        switch (currentLoadoutMenu)
        {
            case 0:
                navigateBaseMenu();
                break;
            case 2:
                
                navigateGunUI();
                break;
        }
    }

    public void initializeGunListing()
    {
        for (int i = 0; i < xHardCap; i++)
        {
            for (int j = 0; j < yHardCap; j++)
            {
                LoadoutGunMini newPiece = Instantiate(template, Vector2.zero, Quaternion.identity);
                gunListing[i, j] = newPiece;
                newPiece.transform.SetParent(displayBG.transform, false);
                newPiece.transform.localPosition = new Vector2(-32.5f + (32.5f * i), 37.5f - (20 * j));
            }
        }
        //First, let's load up our equipped char's gun.
        equipped.updateStats(Controller.c.playerUnits[currentPlayer].currEquip);
        hovered.updateStats(InvManager.im.armory[0]);
    }

    public void loadGunSwap()
    {
        //Determine page count.
        totalPages = (InvManager.im.armory.Count / 12) + 1;

        //Reset currX/Y
        currentX = 0;
        currentY = 0;

        //Set current page to 1.
        gunPage = 1;
    }

    public void loadNextPage()
    {
        bool incompletePage = false;
        //For use only if last page
        int firstNullX = 0;
        int firstNullY = 0;
        //First, load the data.
        for (int i = 0; i < gunListing.GetLength(1); i++)
        {
            for (int j = 0; j < gunListing.GetLength(0); j++)
            {
                int armoryPos = 12*(gunPage - 1) + j + 3*(i);
                //If the gun at this position exists:
                if (armoryPos < InvManager.im.armory.Count)
                {
                    gunListing[j, i].gameObject.SetActive(true);
                    gunListing[j, i].gunName.text = InvManager.im.armory[armoryPos].itemName;
                    gunListing[j, i].nameDrop.text = InvManager.im.armory[armoryPos].itemName;
                    gunListing[j, i].gunType.text = InvManager.im.armory[armoryPos].itemType;
                    gunListing[j, i].gunDrop.text = InvManager.im.armory[armoryPos].itemType;
                    gunListing[j, i].gunRarity = InvManager.im.armory[armoryPos].rarity;
                    gunListing[j, i].ammoCount = InvManager.im.armory[armoryPos].clipSize;
                }
                //If it doesn't:
                else
                {
                    gunListing[j, i].gameObject.SetActive(false);
                    if (!incompletePage)
                    {
                        firstNullX = j;
                        firstNullY = i;
                        incompletePage = true;
                    }
                }
            }
        }
        //Now, check for out of bounds.
        //Basically, anything from this point forward cannot be accessed.
        if (incompletePage)
        {
            xCap = firstNullX;
            yCap = firstNullY;
        }
        else
        {
            xCap = 2;
            yCap = 3;
        }
        checkBorders();
    }

    public void navigateGunUI()
    {
        bool wasPressed = false;
        //Arrow keys to navigate; Z confirm, X return to previous screen.
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            //First, we check if we're at the top of the screen.
            if (currentY == 0)
            {
                //We are, so we cycle to the bottom.
                //Check if xCap is 0; if it is, then the last row is the maximum, so go there.
                //Otherwise, you're in the clear.
                if (xCap == 0)
                {
                    //That being said, this means that the entire row is accessible; don't worry about repositioning it.
                    currentY = yCap - 1;
                }
                else
                {
                    //Because the cap's not at 0 and thus the entire row isn't filled... We need to fix it, too.
                    currentY = yCap;
                    if (currentX >= xCap && xCap != 2)
                    {
                        currentX = xCap - 1;
                    }
                }
            }
            else
            {
                currentY--;
            }
            wasPressed = true;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            //We're already at the bottom, so...
            if ((currentY == yCap && xCap != 0) || (currentY == yCap - 1 && xCap == 0))
            {
                currentY = 0;
            }
            else
            {
                //Make sure that we aren't out of bounds somehow.
                currentY++;
                if (currentX >= xCap && xCap != 2)
                {
                    currentX = xCap - 1;
                }
            }
            wasPressed = true;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentX == 0)
            {
                //Cycle page -1;
                if (gunPage == 1)
                {
                    gunPage = totalPages;
                    loadNextPage();
                }
                else
                {
                    gunPage--;
                    loadNextPage();
                }
                //Now, we check where on the page we end up.
                //First, we examine if we're past the y-bound... Which also means checking the x-bound first.
                //If it's 0, then just shift the y position to the lowest possible and flip the X.
                if (xCap == 0)
                {
                    currentX = 2;
                    if (currentY > yCap - 1)
                    {
                        currentY = yCap - 1;
                    }
                }
                //Otherwise, we attempt to line up the y and force the x.
                else
                {
                    if (currentY < yCap)
                    {
                        currentX = 2;
                    }
                    else
                    {
                        currentY = yCap;
                        if (currentX >= xCap)
                        {
                            currentX = xCap - 1;
                        }
                    }
                }
            }
            else
            {
                currentX--;
            }
            wasPressed = true;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if ((currentX == 2) || (currentY == yCap && currentX == xCap - 1 && xCap < 2))
            {
                //Cycle page +1
                if (gunPage == totalPages)
                {
                    gunPage = 1;
                    loadNextPage();
                }
                else
                {
                    gunPage++;
                    loadNextPage();
                }
                //First, let's flip to 0. We'll end on 0 anyways, so that makes things a bit simple.
                currentX = 0;
                //As with before, check the y-bound to see if we're out of bounds; if we are, fix that.
                if (xCap == 0 && currentY > yCap - 1)
                {
                    currentY = yCap - 1;
                }
                else if (xCap != 0 && currentY > yCap)
                {
                    currentY = yCap;
                }
            }
            else
            {
                currentX++;
            }
            wasPressed = true;
        }
        //Though it's a bit weird, we'll update the hover stuff here.
        int currentGunIndex = 12 * (gunPage - 1) + currentX + 3 * (currentY);
        if (wasPressed)
        {
            hovered.updateStats(InvManager.im.armory[currentGunIndex]);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            swapGun(currentGunIndex);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            //Return to base loadout.
            currentLoadoutMenu = 0;
            baseMenu.gameObject.SetActive(true);
            gunMenu.gameObject.SetActive(false);
            loadoutLimitSetup();
            currentX = currentPlayer;
            currentY = 2;
        }
        checkBorders();
    }
    
    public void swapGun(int gunIndex)
    {
        InvManager.im.armory.Add(Controller.c.playerUnits[currentPlayer].currEquip);
        Controller.c.playerUnits[currentPlayer].currEquip.transform.SetParent(InvManager.im.gameObject.transform, false);
        Controller.c.playerUnits[currentPlayer].currEquip = InvManager.im.armory[gunIndex];
        InvManager.im.armory[gunIndex].transform.SetParent(Controller.c.playerUnits[currentPlayer].transform, false);
        InvManager.im.armory.RemoveAt(gunIndex);
        loadNextPage();
        equipped.updateStats(Controller.c.playerUnits[currentPlayer].currEquip);
        hovered.updateStats(InvManager.im.armory[12 * (gunPage - 1) + currentX + 3 * (currentY)]);
    }

    public void checkBorders()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                gunListing[i, j].border.gameObject.SetActive(true);
                if (currentX == i && currentY == j)
                {
                    gunListing[i, j].border.sprite = borderList[gunListing[i, j].gunRarity - 1];

                }
                else
                {
                    if (gunListing[i, j].gunRarity == 1)
                    {
                        gunListing[i, j].border.gameObject.SetActive(false);
                    }
                    else
                    {
                        gunListing[i, j].border.sprite = borderList[gunListing[i, j].gunRarity + 3];
                    }
                }
            }
        }
    }

    public void loadoutLimitSetup()
    {
        switch (currentLoadoutMenu)
        {
            case 0:
                //Default menu.
                //The x cap is based on how many players there are; y is always 3.
                xHardCap = Controller.c.playerUnits.Length;
                yHardCap = 3;
                break;
            case 1:
                //Character select.
                //Nothing right now.
                break;
            case 2:
                //Gun choice.
                xHardCap = 3;
                yHardCap = 4;
                break;
            case 3:
                //Inventory.
                //Nothing right now.
                break;
        }
        if (Controller.c.missionSelected)
        {
            nextButton.gameObject.SetActive(true);
        }
        else
        {
            nextButton.gameObject.SetActive(false);
        }
    }

    void navigateBaseMenu()
    {
        //Should only be run while currentLoadout == 0
        bool hasChanged = false;
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentY == 0)
            {
                if (Controller.c.missionSelected)
                {
                    //The battle button is available.
                    currentY = -1;
                }
                else
                {
                    currentY = 2;
                }
            }
            else if (currentY == -1)
            {
                currentY = 2;
            }
            else
            {
                currentY--;
            }
            hasChanged = true;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentY == 2)
            {
                if (Controller.c.missionSelected)
                {
                    //The battle button is available.
                    currentY = -1;
                }
                else
                {
                    currentY = 0;
                }
            }
            else
            {
                currentY++;
            }
            hasChanged = true;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentX == 0)
            {
                currentX = xHardCap - 1;
            }
            else
            {
                currentX--;
            }
            hasChanged = true;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentX == xHardCap - 1)
            {
                currentX = 0;
            }
            else
            {
                currentX++;
            }
            hasChanged = true;
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            switch (currentY)
            {
                case 0:
                    //Goto char select.
                    //For now, nothing.
                    break;
                case 1:
                    //Goto inv select.
                    //For now, nothing.
                    break;
                case 2:
                    //Goto gun select.
                    currentLoadoutMenu = 2;
                    gunPage = 1;
                    currentPlayer = currentX;
                    baseMenu.gameObject.SetActive(false);
                    gunMenu.gameObject.SetActive(true);
                    currentX = 0;
                    currentY = 0;
                    loadoutLimitSetup();
                    if (!gunSelectLoaded)
                    {
                        initializeGunListing();
                        loadGunSwap();
                        gunSelectLoaded = true;
                    }
                    loadNextPage();
                    break;
            }
        }
        if (hasChanged)
        {
            updateBaseLoadoutSpr();
        }
    }

    public void updateBaseLoadoutSpr()
    {
        for (int i = 0; i < Controller.c.playerUnits.Length; i++)
        {
            baseLoadoutPlayers[i].sprite = baseLoadoutSprList[0];
            baseLoadoutInvs[i].sprite = baseLoadoutSprList[2];
            baseLoadoutGuns[i].border.sprite = gunEquippedBorders[Controller.c.playerUnits[i].currEquip.rarity - 1];
        }
        nextButton.sprite = baseLoadoutSprList[6];
        switch (currentY)
        {
            case -1:
                nextButton.sprite = baseLoadoutSprList[7];
                break;
            case 0:
                //Highlight the char select.
                baseLoadoutPlayers[currentX].sprite = baseLoadoutSprList[1];
                break;
            case 1:
                baseLoadoutInvs[currentX].sprite = baseLoadoutSprList[3];
                break;
            case 2:
                baseLoadoutGuns[currentX].border.sprite = gunEquippedBordersHL[Controller.c.playerUnits[currentX].currEquip.rarity - 1];
                break;
        }
    }
}
