﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPointer : MonoBehaviour
{
    //Bound these to the map size.
    public int currX, currY;
    //Bounds for the aforementioned values
    public int boundX, boundY;
    //Are we in a menu?
    public bool menuActive = false;
    public bool choosingTarget = false;
    public bool selectingItem = false;
    public Unit targetUnit = null;
    public Unit[] targetUnitTargetList;

    //Which menu option are we choosing?
    public int currentMenuChoice = 0;
    public int menuBound = 3;
    public int currentTargetIndex = 0;
    public int totalPossibleTargets = 0;
    public int invSize = 3;
    public int currentInvChoice = 0;

    //Keep tabs on players.
    public int lookingAtPlayerNo = -1;

    public SpriteRenderer spr;

    //Bruteforce
    public bool waitForNext = false;
    int timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        currX = 0;
        currY = 0;
        //updateBounds();
    }

    // Update is called once per frame
    void Update()
    {
        if (Controller.c.gameMode == 4)
        {
            if (Controller.c.playerTurn)
            {
                if (targetUnit == null || (targetUnit != null && !targetUnit.procPath))
                {
                    moveCursor();
                }
                if (boundX == 0 || boundY == 0)
                {
                    updateBounds();
                }
                selectTile();
                if (waitForNext)
                {
                    if (timer < 0)
                    {
                        Controller.c.checkTurn();
                        waitForNext = false;
                    }
                    timer--;
                }
            }
        }
    }

    public void updateBounds()
    {
        boundX = Controller.c.currMap.xBound;
        boundY = Controller.c.currMap.yBound;
    }

    public void flipSpriteActive(bool isActive)
    {
        spr.enabled = isActive;
    }

    void moveCursor()
    {
        //If we're on the map and don't have a menu open (and can thus mess with the map while on the menu):
        if (!menuActive && !choosingTarget && !selectingItem)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (currY < boundY - 1)
                {
                    currY++;
                    transform.position += new Vector3(0, 1, 0);
                }
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (currY > 0)
                {
                    currY--;
                    transform.position += new Vector3(0, -1, 0);
                }
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (currX > 0)
                {
                    currX--;
                    transform.position += new Vector3(-1, 0, 0);
                }
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (currX < boundX - 1)
                {
                    currX++;
                    transform.position += new Vector3(1, 0, 0);
                }
            }
            checkCurrentUnitIndex();
        }
        //If the menu IS active:
        else if (!choosingTarget && !selectingItem)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (currentMenuChoice == 0)
                {
                    currentMenuChoice = menuBound;
                    Controller.c.playSound(Controller.c.sfx[0], .25f);
                }
                else
                {
                    currentMenuChoice--;
                    Controller.c.playSound(Controller.c.sfx[0], .25f);
                }
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (currentMenuChoice == menuBound)
                {
                    currentMenuChoice = 0;
                    Controller.c.playSound(Controller.c.sfx[0], .25f);
                }
                else
                {
                    currentMenuChoice++;
                    Controller.c.playSound(Controller.c.sfx[0], .25f);
                }
            }
        }
        else if (choosingTarget)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (currentTargetIndex == 0)
                {
                    currentTargetIndex = totalPossibleTargets - 1;
                }
                else
                {
                    currentTargetIndex--;
                }
                Controller.c.playSound(Controller.c.sfx[0], .25f);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (currentTargetIndex == totalPossibleTargets - 1)
                {
                    currentTargetIndex = 0;
                }
                else
                {
                    currentTargetIndex++;
                }
                Controller.c.playSound(Controller.c.sfx[0], .25f);
            }
            currX = targetUnitTargetList[currentTargetIndex].position[0];
            currY = targetUnitTargetList[currentTargetIndex].position[1];
            transform.position = new Vector3(currX, currY + .5f, -3);
        }
        else if (selectingItem)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (currentInvChoice == 0)
                {
                    currentInvChoice = invSize - 1;
                }
                else
                {
                    currentInvChoice--;
                }
                Controller.c.playSound(Controller.c.sfx[0], .25f);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (currentInvChoice == invSize - 1)
                {
                    currentInvChoice = 0;
                }
                else
                {
                    currentInvChoice++;
                }
                Controller.c.playSound(Controller.c.sfx[0], .25f);
            }
        }
    }

    void selectTile()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            //Turn enemy indicators on/off
            if (!choosingTarget)
            {
                Unit eCheck = null;
                foreach (Unit u in Controller.c.enemyUnits)
                {
                    if (u.position[0] == currX && u.position[1] == currY)
                    {
                        eCheck = u;
                    }
                }
                if (eCheck != null)
                {
                    //Reveal your secrets.
                    if (eCheck.displayActive)
                    {
                        eCheck.hideMovement();
                        Controller.c.playSound(Controller.c.sfx[2], .25f);
                    }
                    else
                    {
                        eCheck.showMovement();
                        Controller.c.playSound(Controller.c.sfx[1], .25f);
                    }
                    eCheck.displayActive = !eCheck.displayActive;
                }
            }
            if (targetUnit == null)
            {
                Unit tempCheck = null;
                foreach (Unit u in Controller.c.playerUnits)
                {
                    if (u.position[0] == currX && u.position[1] == currY)
                    {
                        tempCheck = u;
                    }
                }
                if (tempCheck != null)
                {
                    if (!tempCheck.hasMoved)
                    {
                        tempCheck.currUnit = true;
                        targetUnit = tempCheck;
                        targetUnit.startFinding();
                        targetUnit.showMovement();
                        Controller.c.playSound(Controller.c.sfx[1], .25f);
                    }
                }
            }
            else if (!menuActive)
            {
                //Pick a position!
                //if targetUnit's pathMap has a path at given location, follow path.
                //For the sake of this current version, just jump straight to location.
                //Afterwards, re-enable all blacked out tiles and set unit to have moved. 
                //After that, make currUnit and targetUnit false/null, then check via Controller if we move to next turn.
                if (targetUnit.pathMap[currX,currY].set == true && !targetUnit.inPosition)
                {
                    targetUnit.nextIndex = 0;
                    targetUnit.savedPath = targetUnit.pathMap[currX, currY].path;
                    targetUnit.procPath = true;
                    targetUnit.lastPosition[0] = targetUnit.position[0];
                    targetUnit.lastPosition[1] = targetUnit.position[1];
                    /*//Fix unit map
                    Controller.c.unitMap[targetUnit.position[0], targetUnit.position[1]] = 0;
                    //Move unit to position
                    //Later: replace this with a function that moves through each tile in order.
                    targetUnit.transform.position = Controller.c.currMap.grid[currX, currY].transform.position;
                    targetUnit.lastPosition[0] = targetUnit.position[0];
                    targetUnit.lastPosition[1] = targetUnit.position[1];
                    targetUnit.position[0] = currX;
                    targetUnit.position[1] = currY;
                    //Update unit map again
                    Controller.c.unitMap[targetUnit.position[0], targetUnit.position[1]] = targetUnit.unitAllegiance;
                    //Temporarily keep unit here until battle stuff is complete.*/
                    targetUnit.hideMovement();
                    Controller.c.playSound(Controller.c.sfx[1], .25f);
                }
            }
            else if (menuActive && !choosingTarget && !selectingItem)
            {
                selectAction();
            }
            else if (choosingTarget)
            {
                //Choose a target.
                //At this point, you are hovering over a target. So just, like attack them?
                targetUnit.target = targetUnitTargetList[currentTargetIndex];
                targetUnit.attack();
                if (!targetUnit.determination)
                {
                    targetUnit.hasMoved = true;
                }
                targetUnit.determination = false;
                //Then, as usual, run the wait stuff.
                targetUnit.currUnit = false;
                //Reset pathmap.
                targetUnit.clearPaths(targetUnit.pathMap);
                //Reset tiles
                for (int i = 0; i < Controller.c.currMap.grid.GetLength(0); i++)
                {
                    for (int j = 0; j < Controller.c.currMap.grid.GetLength(1); j++)
                    {
                        Controller.c.currMap.grid[i, j].gameObject.SetActive(true);
                    }
                }
                targetUnit.hideAtkRange();
                targetUnit.inPosition = false;
                targetUnit = null;
                choosingTarget = false;
                menuActive = false;
                currentMenuChoice = 0;
                timer = 45;
                waitForNext = true;
                Controller.c.playSound(Controller.c.sfx[1], .25f);
            }
            else if (selectingItem)
            {
                //Pick an item.
                bool itemUseStatus = targetUnit.inventory[currentInvChoice].useItem(targetUnit);
                if (itemUseStatus)
                {
                    Item tempItem = targetUnit.inventory[currentInvChoice];
                    targetUnit.inventory.RemoveAt(currentInvChoice);
                    Destroy(tempItem.gameObject);
                    targetUnit.hasMoved = true;
                    targetUnit.currUnit = false;
                    //Reset pathmap.
                    targetUnit.clearPaths(targetUnit.pathMap);
                    //Reset tiles
                    for (int i = 0; i < Controller.c.currMap.grid.GetLength(0); i++)
                    {
                        for (int j = 0; j < Controller.c.currMap.grid.GetLength(1); j++)
                        {
                            Controller.c.currMap.grid[i, j].gameObject.SetActive(true);
                        }
                    }
                    targetUnit.inPosition = false;
                    targetUnit = null;
                    menuActive = false;
                    currentMenuChoice = 0;
                    selectingItem = false;
                    Controller.c.checkTurn();
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            if (targetUnit != null)
            {
                if (menuActive && !choosingTarget)
                {
                    menuActive = !menuActive;
                    targetUnit.inPosition = false;
                    currentMenuChoice = 0;
                    targetUnit.showMovement();
                    Controller.c.unitMap[targetUnit.position[0], targetUnit.position[1]] = 0;
                    targetUnit.position[0] = targetUnit.lastPosition[0];
                    targetUnit.position[1] = targetUnit.lastPosition[1];
                    targetUnit.transform.position = Controller.c.currMap.grid[targetUnit.position[0], targetUnit.position[1]].transform.position;
                }
                else if (choosingTarget)
                {
                    //Go back to loc select.
                    choosingTarget = !choosingTarget;
                    //Move this back to the player.
                    currX = targetUnit.position[0];
                    currY = targetUnit.position[1];
                    transform.position = new Vector3(targetUnit.position[0], targetUnit.position[1] + 0.5f, -3);
                    currentTargetIndex = 0;
                    targetUnit.hideAtkRange();
                }
                else
                {
                    //Reset tiles
                    targetUnit.hideMovement();
                    //Reset pathmap.
                    targetUnit.clearPaths(targetUnit.pathMap);
                    //The rest.
                    targetUnit.currUnit = false;
                    targetUnit = null;
                }
                Controller.c.playSound(Controller.c.sfx[2], .25f);
            }
            if (selectingItem)
            {
                selectingItem = false;
                Controller.c.playSound(Controller.c.sfx[2], .25f);
                currentInvChoice = 0;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            cycleNext();
            Controller.c.playSound(Controller.c.sfx[0], .25f);
        }
        if (targetUnit != null)
        {
            if (targetUnit.inPosition)
            {
                menuActive = true;
            }
        }
    }

    public void selectAction()
    {
        if (menuActive)
        {
            switch (currentMenuChoice)
            {
                case 0:
                    //Attack
                    //Run attack stuff
                    //First: reset currentTargetIndex.
                    currentTargetIndex = 0;
                    if (targetUnit.currEquip.currentClip > 0)
                    {
                        totalPossibleTargets = 0;
                        targetUnit.findTargets();
                        bool targetsAvailable = false;
                        for (int i = 0; i < targetUnit.possibleTargets.Count; i++)
                        {
                            if (targetUnit.possibleTargets[i] != null)
                            {
                                targetsAvailable = true;
                                totalPossibleTargets++;
                            }
                        }
                        if (targetsAvailable)
                        {
                            targetUnit.showAtkRange();
                            targetUnitTargetList = new Unit[totalPossibleTargets];
                            for (int i = 0; i < totalPossibleTargets; i++)
                            {
                                targetUnitTargetList[i] = targetUnit.possibleTargets[i];
                            }
                            choosingTarget = true;
                            Controller.c.playSound(Controller.c.sfx[1], .25f);
                        }
                    }
                    else
                    {
                        Controller.c.playSound(Controller.c.sfx[10], .25f);
                    }
                    break;
                case 1:
                    //Reload
                    //Run reload stuff
                    //Check if you CAN reload.
                    if (targetUnit.currEquip.currentClip < targetUnit.currEquip.clipSize)
                    {
                        targetUnit.currEquip.currentClip = targetUnit.currEquip.clipSize;

                        //Move wait stuff here.
                        targetUnit.hasMoved = true;
                        targetUnit.currUnit = false;
                        //Reset pathmap.
                        targetUnit.clearPaths(targetUnit.pathMap);
                        //Reset tiles
                        for (int i = 0; i < Controller.c.currMap.grid.GetLength(0); i++)
                        {
                            for (int j = 0; j < Controller.c.currMap.grid.GetLength(1); j++)
                            {
                                Controller.c.currMap.grid[i, j].gameObject.SetActive(true);
                            }
                        }
                        BattleMenuUI.bmui.updatePlayerValues(BattleMenuUI.bmui.currentPlayer);
                        targetUnit = null;
                        menuActive = false;
                        Controller.c.playSound(Controller.c.sfx[11], .25f);
                        currentMenuChoice = 0;
                        Controller.c.checkTurn();
                    }
                    else
                    {
                        Debug.Log("Ammo full; no reloading needed!");
                        Controller.c.playSound(Controller.c.sfx[10], .25f);
                    }
                    break;
                case 2:
                    //Inv
                    //Run inv stuff
                    invSize = targetUnit.inventory.Count;
                    if (invSize > 0)
                    {
                        selectingItem = true;
                        currentInvChoice = 0;
                        //Populate list
                        string tempStr = "";
                        foreach (Item i in targetUnit.inventory)
                        {
                            tempStr += i.itemName + "\t\t\t+ " + i.healAmt + " HP\n";
                        }
                        InvManager.im.currInvShown.text = tempStr;
                        Controller.c.playSound(Controller.c.sfx[1], .25f);
                    }
                    else
                    {
                        Debug.Log("Inventory empty!");
                        Controller.c.playSound(Controller.c.sfx[10], .25f);
                    }
                    break;
                case 3:
                    //Wait
                    //Move wait stuff here.
                    targetUnit.hasMoved = true;
                    targetUnit.currUnit = false;
                    //Reset pathmap.
                    targetUnit.clearPaths(targetUnit.pathMap);
                    //Reset tiles
                    for (int i = 0; i < Controller.c.currMap.grid.GetLength(0); i++)
                    {
                        for (int j = 0; j < Controller.c.currMap.grid.GetLength(1); j++)
                        {
                            Controller.c.currMap.grid[i, j].gameObject.SetActive(true);
                        }
                    }
                    targetUnit = null;
                    menuActive = false;
                    currentMenuChoice = 0;
                    Controller.c.checkTurn();
                    Controller.c.playSound(Controller.c.sfx[1], .25f);
                    break;
            }
        }
    }

    public void checkCurrentUnitIndex()
    {
        //Updates lookingAtPlayerNo.
        bool foundCurrent = false;
        int givenIndex = 0;
        foreach (Unit u in Controller.c.playerUnits){
            if (!foundCurrent)
            {
                if (u.position[0] == currX && u.position[1] == currY && !u.isDead && !u.hasMoved)
                {
                    foundCurrent = true;
                    lookingAtPlayerNo = givenIndex;
                }
                givenIndex++;
            }
        }
        if (!foundCurrent)
        {
            lookingAtPlayerNo = -1;
        }
    }

    public void cycleNext()
    {
        bool foundNext = false;
        int givenIndex = 0;
        Unit tempUnit = null;
        if (!menuActive && Controller.c.playerTurn)
        {
            //First, check if we're on a player.
            if (lookingAtPlayerNo != -1)
            {
                //We are, so check to see what's next.
                if (lookingAtPlayerNo == Controller.c.playerUnits.Length - 1)
                {
                    for (int i = 0; i < Controller.c.playerUnits.Length; i++)
                    {
                        if (!foundNext)
                        {
                            if (!(Controller.c.playerUnits[i].isDead) && !(Controller.c.playerUnits[i].hasMoved))
                            {
                                foundNext = true;
                                givenIndex = i;
                            }
                        }
                    }
                    //Move to that position.
                    lookingAtPlayerNo = givenIndex;
                    tempUnit = Controller.c.playerUnits[givenIndex];
                    currX = tempUnit.position[0];
                    currY = tempUnit.position[1];
                    transform.position = new Vector3(tempUnit.position[0], tempUnit.position[1] + 0.5f, -3);
                }
                else
                {
                    //We're not at the end, so first look at what's next.
                    givenIndex = lookingAtPlayerNo;
                    for (int i = lookingAtPlayerNo + 1; i < Controller.c.playerUnits.Length; i++)
                    {
                        if (!(Controller.c.playerUnits[i].hasMoved) && !(Controller.c.playerUnits[i].isDead) && !foundNext)
                        {
                            foundNext = true;
                            givenIndex = i;
                        }
                    }
                    if (!foundNext)
                    {
                        for (int i = 0; i < Controller.c.playerUnits.Length; i++)
                        {
                            if (!foundNext)
                            {
                                if (!(Controller.c.playerUnits[i].isDead) && !(Controller.c.playerUnits[i].hasMoved))
                                {
                                    foundNext = true;
                                    givenIndex = i;
                                }
                            }
                        }
                    }
                    //Move to that position.
                    lookingAtPlayerNo = givenIndex;
                    tempUnit = Controller.c.playerUnits[givenIndex];
                    currX = tempUnit.position[0];
                    currY = tempUnit.position[1];
                    transform.position = new Vector3(tempUnit.position[0], tempUnit.position[1] + 0.5f, -3);
                }
            }
            else
            {
                //If we're not on a player, move to the first character that hasn't moved.
                for (int i = 0; i < Controller.c.playerUnits.Length; i++)
                {
                    if (!foundNext)
                    {
                        if (!(Controller.c.playerUnits[i].isDead) && !(Controller.c.playerUnits[i].hasMoved))
                        {
                            foundNext = true;
                            givenIndex = i;
                        }
                    }
                }
                //Move to that position.
                lookingAtPlayerNo = givenIndex;
                tempUnit = Controller.c.playerUnits[givenIndex];
                currX = tempUnit.position[0];
                currY = tempUnit.position[1];
                transform.position = new Vector3(tempUnit.position[0], tempUnit.position[1] + 0.5f, -3);
            }
        }
    }
}
