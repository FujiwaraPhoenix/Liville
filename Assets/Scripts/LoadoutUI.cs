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
    public int yHardCap = 5;

    //UI stuff holders.
    public GameObject baseMenu, invMenu, gunMenu;
    //In order: playerBGs, invs, next.
    public Sprite[] baseLoadoutSprList = new Sprite[6];
    public Sprite[] gunEquippedBorders = new Sprite[5];
    public Sprite[] gunEquippedBordersHL = new Sprite[5];

    //For base menu; all GameObjects present here.
    public Image[] baseLoadoutPlayers = new Image[4];
    public Image[] baseLoadoutInvs = new Image[4];
    public GunInfo[] baseLoadoutGuns = new GunInfo[4];
    public Image nextButton;
    public PlayerData[] pData = new PlayerData[4];

    //For gun select; need 2D array of guns
    public LoadoutGunMini template;
    public LoadoutGunMini[,] gunListing;

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

    //The inventory menu is actually pretty minimal; what we need is simple.
    //InvMenu should be active and have the following pieces of information:
    public InventoryUI iUI;
    public bool hoveringPInv, invSelected;
    public int currentValue;

    public bool[] activePlayers = new bool[4];

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
            case 1:
                navigateInvUI();
                break;
            case 2:
                navigateGunUI();
                break;
            case 3:
                break;
        }
    }
    //Initialization functions
    //----------------------------------------------------------------------------------------------------------------------------
    
    public void loadoutLimitSetup()
    {
        switch (currentLoadoutMenu)
        {
            case 0:
                //Default menu.
                //The x cap is based on how many players there are; y is always 3.
                xHardCap = Controller.c.playerRoster.Length;
                yHardCap = 3;
                break;
            case 1:
                //Character select.
                //Nothing right now.
                break;
            case 2:
                //Gun choice.
                xHardCap = 3;
                yHardCap = 5;
                break;
            case 3:
                //Inventory.
                //The only thing that matters is the inventory count of a player, so this is pretty irrelevant.

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

    public void initializeGunListing()
    {
        gunListing = new LoadoutGunMini[xHardCap, yHardCap];
        for (int i = 0; i < xHardCap; i++)
        {
            for (int j = 0; j < yHardCap; j++)
            {
                LoadoutGunMini newPiece = Instantiate(template, Vector2.zero, Quaternion.identity);
                gunListing[i, j] = newPiece;
                newPiece.transform.SetParent(displayBG.transform, false);
                newPiece.transform.localPosition = new Vector2(-180f + (180f * i), 160f - (80 * j));
            }
        }
        //First, let's load up our equipped char's gun.
        equipped.updateStats(Controller.c.playerRoster[currentPlayer].currEquip);
        equipped.updateMods(Controller.c.playerRoster[currentPlayer].currEquip);
        hovered.updateMods(InvManager.im.armory[0]);
        hovered.updateMods(InvManager.im.armory[0]);
        equipped.updateBorder(Controller.c.playerRoster[currentPlayer].currEquip);
        hovered.updateBorder(InvManager.im.armory[0]);
    }

    public void loadGunSwap()
    {
        //Determine page count.
        totalPages = (InvManager.im.armory.Count / 15) + 1;

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
                int armoryPos = 15 *(gunPage - 1) + j + 3*(i);
                //If the gun at this position exists:
                if (armoryPos < InvManager.im.armory.Count)
                {
                    Item tempGun = InvManager.im.armory[armoryPos];
                    gunListing[j, i].gameObject.SetActive(true);
                    gunListing[j, i].gunName.text = tempGun.itemName;
                    gunListing[j, i].nameDrop.text = tempGun.itemName;
                    gunListing[j, i].gunType.text = tempGun.itemType;
                    gunListing[j, i].gunDrop.text = tempGun.itemType;
                    gunListing[j, i].gunRarity = tempGun.rarity;
                    gunListing[j, i].ammoCount = tempGun.clipSize;
                    if (tempGun.mods.GetLength(0) > 0)
                    {
                        gunListing[j, i].modIcons[0].sprite = Controller.c.determineModIcon(tempGun.mods[0, 0], tempGun.mods[0, 1] - 1);
                    }
                    else
                    {
                        gunListing[j, i].modIcons[0].sprite = Controller.c.blankMod;
                    }
                    if (tempGun.mods.GetLength(0) > 1)
                    {
                        gunListing[j, i].modIcons[1].sprite = Controller.c.determineModIcon(tempGun.mods[1, 0], tempGun.mods[1, 1] - 1);
                    }
                    else
                    {
                        gunListing[j, i].modIcons[1].sprite = Controller.c.blankMod;
                    }
                    if (tempGun.mods.GetLength(0) > 2)
                    {
                        gunListing[j, i].modIcons[2].sprite = Controller.c.determineModIcon(tempGun.mods[2, 0], tempGun.mods[2, 1] - 1);
                    }
                    else
                    {
                        gunListing[j, i].modIcons[2].sprite = Controller.c.blankMod;
                    }
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
            xCap = 3;
            yCap = 4;
        }
        checkBorders();
    }

    //Navigation functions
    //----------------------------------------------------------------------------------------------------------------------------


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
                    //Enable/disable character.
                    deReActivateChar(currentX);
                    break;
                case 1:
                    //Goto inv select.
                    currentLoadoutMenu = 1;
                    currentPlayer = currentX;
                    baseMenu.gameObject.SetActive(false);
                    invMenu.gameObject.SetActive(true);
                    invSelected = false;
                    hoveringPInv = true;
                    iUI.currentHL = 0;
                    iUI.updateCountTotals(currentPlayer);
                    if (iUI.itemCountC > 15)
                    {
                        iUI.currentMax = 14;
                    }
                    else
                    {
                        iUI.currentMax = iUI.itemCountC;
                    }
                    iUI.updatePlayerInvText(currentPlayer);
                    iUI.updateText();
                    iUI.invBG.sprite = iUI.invHL;
                    currentValue = 0;
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
                case 3:
                    break;
            }
        }
        if (hasChanged)
        {
            updateBaseLoadoutSpr();
        }
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
                    if (currentX >= xCap && xCap >= 2)
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
                if (currentX >= xCap && xCap >= 2 )
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
            if ((currentX == 2) || (currentY == yCap && currentX == xCap - 1 && xCap < 3))
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
        int currentGunIndex = 15 * (gunPage - 1) + currentX + 3 * (currentY);
        if (wasPressed)
        {
            hovered.updateStats(InvManager.im.armory[currentGunIndex]);
            hovered.updateMods(InvManager.im.armory[currentGunIndex]);
            hovered.updateBorder(InvManager.im.armory[currentGunIndex]);
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
            updateBaseLoadoutSpr();

        }
        checkBorders();
    }
    
    public void navigateInvUI()
    {
        //Honestly, most of this can be moved into inventory UI for convenience's sake. Now is not the time for that.
        //Only run when currentLoadout = 1.
        if ((Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)) && !invSelected)
        {
            hoveringPInv = !hoveringPInv;
            if (hoveringPInv)
            {
                iUI.invBG.sprite = iUI.invHL;
                iUI.convoyBG.sprite = iUI.convoyBase;
            }
            else
            {
                iUI.invBG.sprite = iUI.invBase;
                iUI.convoyBG.sprite = iUI.convoyHL;
            }
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) && invSelected)
        {
            if (currentValue > 0)
            {
                currentValue--;
                //Highlight the current value
                if (hoveringPInv)
                {
                    iUI.currentHL--;
                    iUI.invIcons[iUI.currentHL + 1].sprite = iUI.basePotion;
                    iUI.invIcons[iUI.currentHL].sprite = iUI.litPotion;
                    iUI.updateData(Controller.c.playerRoster[currentPlayer].inventory[currentValue]);
                }
                else
                {
                    //Figure out where the current HL is.
                    if (iUI.currentHL > 0)
                    {
                        //Not at the top.
                        iUI.convoyIcons[iUI.currentHL].sprite = iUI.basePotion;
                        iUI.currentHL--;
                        iUI.convoyIcons[iUI.currentHL].sprite = iUI.litPotion;
                    }
                    else
                    {
                        //If the list is smaller than the cap...
                        if (iUI.currentMax < 14)
                        {
                            iUI.currentHL = iUI.currentMax - 1;
                        }
                        //Otherwise...
                        else
                        {
                            if (iUI.currentMax > 14)
                            {
                                iUI.currentMax--;
                                iUI.offset--;
                                iUI.updateText();
                            }
                            else
                            {
                                iUI.currentHL = 14;
                            }
                        }
                        iUI.convoyIcons[0].sprite = iUI.basePotion;
                        iUI.convoyIcons[iUI.currentHL].sprite = iUI.litPotion;
                    }
                }
                iUI.updateData(InvManager.im.convoy[currentValue]);
            }
            else
            {
                //We're at the top here.
                if (hoveringPInv)
                {
                    currentValue = iUI.itemCountI - 1;
                    iUI.currentHL = iUI.itemCountI - 1;
                    iUI.invIcons[0].sprite = iUI.basePotion;
                    iUI.invIcons[iUI.currentHL].sprite = iUI.litPotion;
                    iUI.updateData(Controller.c.playerRoster[currentPlayer].inventory[currentValue]);
                }
                else
                {
                    currentValue = iUI.itemCountC - 1;
                    iUI.offset = currentValue - iUI.currentMax;
                    iUI.currentMax = currentValue;
                    iUI.convoyIcons[0].sprite = iUI.basePotion;
                    if (currentValue >= 15)
                    {
                        iUI.currentHL = 14;
                        iUI.convoyIcons[iUI.currentHL].sprite = iUI.litPotion;
                    }
                    else
                    {
                        iUI.currentHL = iUI.currentMax;
                        iUI.convoyIcons[iUI.currentHL].sprite = iUI.litPotion;
                    }
                    iUI.updateText();
                    iUI.updateData(InvManager.im.convoy[currentValue]);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) && invSelected)
        {
            //First, check if we're in inventory or convoy.
            if (hoveringPInv)
            {
                //Inv, thus limit is 5.
                if (currentValue < iUI.itemCountI - 1)
                {
                    currentValue++;
                    iUI.currentHL++;
                    //Highlight the current value
                    iUI.invIcons[iUI.currentHL - 1].sprite = iUI.basePotion;
                    iUI.invIcons[iUI.currentHL].sprite = iUI.litPotion;
                }
                else
                {
                    currentValue = 0;
                    iUI.invIcons[iUI.currentHL].sprite = iUI.basePotion;
                    iUI.currentHL = 0;
                    iUI.invIcons[iUI.currentHL].sprite = iUI.litPotion;
                }
                iUI.updateData(Controller.c.playerRoster[currentPlayer].inventory[currentValue]);
            }
            else
            {
                //Convoy, so we need to check for limits.
                if (iUI.currentMax >= 14 && iUI.currentMax < iUI.itemCountC - 1)
                {
                    //For the weird intermediary area where we're between n and n+14, where n > 0.
                    currentValue++;
                    if (iUI.currentHL < 14)
                    {
                        iUI.currentHL++;
                        iUI.convoyIcons[iUI.currentHL - 1].sprite = iUI.basePotion;
                        iUI.convoyIcons[iUI.currentHL].sprite = iUI.litPotion;
                    }
                    else
                    {
                        //We're at the bottom.
                        iUI.currentMax++;
                        iUI.offset++;
                        iUI.updateText();
                    }
                    iUI.updateData(InvManager.im.convoy[currentValue]);
                }
                else if (iUI.currentMax == iUI.itemCountC - 1)
                {
                    if (iUI.currentHL == 14 || (iUI.currentHL == iUI.currentMax))
                    {
                        currentValue = 0;
                        iUI.convoyIcons[iUI.currentHL].sprite = iUI.basePotion;
                        iUI.currentHL = 0;
                        iUI.convoyIcons[iUI.currentHL].sprite = iUI.litPotion;
                        if (iUI.currentMax > 14)
                        {
                            iUI.currentMax = 14;
                        }
                        iUI.offset = 0;
                        iUI.updateText();
                    }
                    else
                    {
                        currentValue++;
                        iUI.convoyIcons[iUI.currentHL].sprite = iUI.basePotion;
                        iUI.currentHL++;
                        iUI.convoyIcons[iUI.currentHL].sprite = iUI.litPotion;
                    }
                }
                else
                {
                    iUI.convoyIcons[iUI.currentHL].sprite = iUI.basePotion;
                    iUI.currentHL++;
                    iUI.convoyIcons[iUI.currentHL].sprite = iUI.litPotion;
                    currentValue++;
                }
                iUI.updateData(InvManager.im.convoy[currentValue]);
            }

        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (!invSelected)
            {
                if (hoveringPInv)
                {
                    if (Controller.c.playerRoster[currentPlayer].inventory.Count > 0)
                    {
                        invSelected = true;
                        currentValue = 0;
                        iUI.currentHL = 0;
                        iUI.invIcons[iUI.currentHL].sprite = iUI.litPotion;
                        iUI.updateData(Controller.c.playerRoster[currentPlayer].inventory[currentValue]);

                    }
                }
                else
                {
                    if (InvManager.im.convoy.Count > 0)
                    {
                        invSelected = true;
                        currentValue = 0;
                        iUI.currentHL = 0;
                        if (iUI.itemCountC > 14)
                        {
                            iUI.currentMax = 14;
                        }
                        else
                        {
                            iUI.currentMax = iUI.itemCountC - 1;
                        }
                        iUI.updateCountTotals(currentPlayer);
                        iUI.convoyIcons[iUI.currentHL].sprite = iUI.litPotion;
                        iUI.updateText();
                        iUI.updateData(InvManager.im.convoy[currentValue]);
                    }
                }
            }
            else
            {
                if (hoveringPInv)
                {
                    //Adding from player inventory to convoy.
                    Item temp = Controller.c.playerRoster[currentPlayer].inventory[currentValue];
                    Controller.c.playerRoster[currentPlayer].inventory.Remove(temp);
                    InvManager.im.convoy.Add(temp);
                    temp.transform.parent = InvManager.im.transform;
                    iUI.updateCountTotals(currentPlayer);
                    if (currentValue > 0)
                    {
                        iUI.invIcons[iUI.currentHL].sprite = iUI.basePotion;
                        iUI.currentHL--;
                        iUI.invIcons[iUI.currentHL].sprite = iUI.litPotion;
                        currentValue--;
                    }
                    iUI.updateData(Controller.c.playerRoster[currentPlayer].inventory[currentValue]);
                }
                else
                {
                    if (iUI.itemCountI < 5)
                    {
                        //Adding from convoy to player inventory.
                        Item temp = InvManager.im.convoy[currentValue];
                        Controller.c.playerRoster[currentPlayer].inventory.Add(temp);
                        InvManager.im.convoy.Remove(temp);
                        temp.transform.parent = Controller.c.playerRoster[currentPlayer].transform;
                        iUI.updateCountTotals(currentPlayer);
                        if (currentValue > 0)
                        {
                            if (iUI.currentMax < 14)
                            {
                                iUI.invIcons[iUI.currentHL].sprite = iUI.basePotion;
                                iUI.currentHL--;
                                iUI.invIcons[iUI.currentHL].sprite = iUI.litPotion;
                                currentValue--;
                            }
                            else
                            {
                                //Maintain current HL.
                                //CurrentVal remains constant unless it was removed at the end.
                                if (currentValue > iUI.itemCountC - 1)
                                {
                                    //If it was removed at the end (in this case), reduce CV by 1 but keep 14 on the board.
                                    currentValue--;
                                    iUI.offset--;
                                    iUI.currentMax--;
                                }
                            }
                        }
                    }
                    iUI.updateData(InvManager.im.convoy[currentValue]);
                }
                iUI.updateText();
                iUI.updatePlayerInvText(currentPlayer);
                if (iUI.itemCountI == 0 || iUI.itemCountC == 0)
                {
                    Debug.Log("Nothing in this set.");
                    iUI.invIcons[0].sprite = iUI.basePotion;
                    iUI.convoyIcons[0].sprite = iUI.basePotion;
                    currentValue = 0;
                    iUI.currentHL = 0;
                    invSelected = !invSelected;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (invSelected)
            {
                invSelected = !invSelected;
                if (!hoveringPInv)
                {
                    iUI.convoyIcons[iUI.currentHL].sprite = iUI.basePotion;
                    iUI.offset = 0;
                }
                else
                {
                    iUI.invIcons[iUI.currentHL].sprite = iUI.basePotion;
                }
                iUI.updateData(null);
            }
            else
            {
                //Return to base loadout.
                currentLoadoutMenu = 0;
                baseMenu.gameObject.SetActive(true);
                invMenu.gameObject.SetActive(false);
                loadoutLimitSetup();
                currentX = currentPlayer;
                currentY = 1;
            }
        }
    }

    public void swapGun(int gunIndex)
    {
        InvManager.im.armory.Add(Controller.c.playerRoster[currentPlayer].currEquip);
        Controller.c.playerRoster[currentPlayer].currEquip.transform.SetParent(InvManager.im.gameObject.transform, false);
        Controller.c.playerRoster[currentPlayer].currEquip = InvManager.im.armory[gunIndex];
        InvManager.im.armory[gunIndex].transform.SetParent(Controller.c.playerRoster[currentPlayer].transform, false);
        InvManager.im.armory.RemoveAt(gunIndex);
        loadNextPage();
        equipped.updateStats(Controller.c.playerRoster[currentPlayer].currEquip);
        equipped.updateMods(Controller.c.playerRoster[currentPlayer].currEquip);
        hovered.updateStats(InvManager.im.armory[15 * (gunPage - 1) + currentX + 3 * (currentY)]);
        hovered.updateMods(InvManager.im.armory[15 * (gunPage - 1) + currentX + 3 * (currentY)]);
        equipped.updateBorder(Controller.c.playerRoster[currentPlayer].currEquip);
        int currentGunIndex = 15 * (gunPage - 1) + currentX + 3 * (currentY);
        hovered.updateBorder(InvManager.im.armory[currentGunIndex]);
    }

    public void checkBorders()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 5; j++)
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



    public void updateBaseLoadoutSpr()
    {
        for (int i = 0; i < Controller.c.playerRoster.Length; i++)
        {
            baseLoadoutPlayers[i].sprite = baseLoadoutSprList[0];
            baseLoadoutInvs[i].sprite = baseLoadoutSprList[2];
            baseLoadoutGuns[i].border.sprite = gunEquippedBorders[Controller.c.playerRoster[i].currEquip.rarity - 1];
        }
        nextButton.sprite = baseLoadoutSprList[4];
        switch (currentY)
        {
            case -1:
                nextButton.sprite = baseLoadoutSprList[5];
                break;
            case 0:
                //Highlight the char select.
                baseLoadoutPlayers[currentX].sprite = baseLoadoutSprList[1];
                break;
            case 1:
                baseLoadoutInvs[currentX].sprite = baseLoadoutSprList[3];
                break;
            case 2:
                baseLoadoutGuns[currentX].border.sprite = gunEquippedBordersHL[Controller.c.playerRoster[currentX].currEquip.rarity - 1];
                break;
        }
    }

    public int activePlayerCount()
    {
        int active = 0;
        foreach (bool b in activePlayers)
        {
            if (b)
            {
                active++;
            }
        }
        return active;
    }

    public void deReActivateChar(int charID)
    {
        activePlayers[charID] = !activePlayers[charID];
        pData[charID].isActive = !pData[charID].isActive;
        pData[charID].updateValues();
    }

    public Unit[] currentPList()
    {
        Unit[] output = new Unit[activePlayerCount()];
        int currentVal = 0;
        for (int i = 0; i < Controller.c.playerRoster.Length; i++)
        {
            if (activePlayers[i])
            {
                Controller.c.playerRoster[i].gameObject.SetActive(true);
                output[currentVal] = Controller.c.playerRoster[i];
                currentVal++;
            }
            else
            {
                Controller.c.playerRoster[i].gameObject.SetActive(false);
            }
        }
        return output;
    }
}
