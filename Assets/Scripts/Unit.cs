using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public int hp, maxhp, eva, def, lck, mvt, unitAllegiance, statusResist;
    public int[] position;
    public int[] lastPosition;
    public bool isDead, hasMoved, currUnit, stunned, determination;
    public List<Item> inventory = new List<Item>();
    //Limit inv to 5.
    public Item currEquip;
    public Path p;
    public Path[,] pathMap;
    public Unit target;
    public List<Unit> possibleTargets = new List<Unit>();
    public bool[] availableOptions = new bool[4];
    public SpriteRenderer spr;
    public Sprite unitFace;
    public string unitName = "temp";
    //Statuses, in order: elec, burn, freeze, mark, poison
    public int[] negStatus = new int[5];

    public GameObject holder, holder2;
    public SpriteRenderer modifierA, tens, tenOnes, modifierB, ones;

    public int nextIndex = 0;
    public List<int> savedPath = new List<int>();
    public bool procPath = false;
    int timer = 15;
    int showDamageTimer = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        /*pathMap = new Path[10, 10];
        for (int i = 0; i < Controller.c.currMap.xBound; i++)
        {
            for (int j = 0; j < Controller.c.currMap.yBound; j++)
            {
                Path tempPath = Instantiate(p, transform.position, Quaternion.identity);
                pathMap[i, j] = tempPath;
                tempPath.fillPath(9);
            }
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        if (Controller.c.gameMode == 4 && Controller.c.currMap.loaded)
        {
            if (Controller.c.unitMap[position[0], position[1]] != unitAllegiance)
            {
                Controller.c.unitMap[position[0], position[1]] = unitAllegiance;
            }
            //For moving enemies.
            if (procPath && savedPath != null && Controller.c.currentMovingEnemy < Controller.c.enemyUnits.Count)
            {
                if (Controller.c.enemyUnits[Controller.c.currentMovingEnemy] == this)
                {
                    if (timer <= 0)
                    {
                        processPath();
                    }
                    else
                    {
                        timer--;
                    }
                }
            }
            //Showing damage
            if (showDamageTimer < 0)
            {
                holder.gameObject.SetActive(false);
                holder2.gameObject.SetActive(false);
            }
            showDamageTimer--;
        }
    }

    public void startFinding()
    {
        //Test function; check for hazards. Player default.

        pathMap = new Path[Controller.c.currMap.xBound, Controller.c.currMap.yBound];
        for (int i = 0; i < Controller.c.currMap.xBound; i++)
        {
            for (int j = 0; j < Controller.c.currMap.yBound; j++)
            {
                Path tempPath = Instantiate(p, transform.position, Quaternion.identity);
                pathMap[i, j] = tempPath;
                tempPath.fillPath(9);
            }
        }
        Path newPath = Instantiate(p, transform.position, Quaternion.identity);
        newPath.whoseSide = unitAllegiance;
        Pathfinder.pf.drawPath(this, position, mvt + currEquip.tempMvt, newPath, unitAllegiance);
        newPath.currentTile = true;
        newPath.set = true;
        pathMap[position[0], position[1]] = newPath;
        showMovement();
    }

    public void huntPlayers()
    {
        //Test function; check for hazards. Enemy default.
        pathMap = new Path[Controller.c.currMap.xBound, Controller.c.currMap.yBound];
        for (int i = 0; i < Controller.c.currMap.xBound; i++)
        {
            for (int j = 0; j < Controller.c.currMap.yBound; j++)
            {
                Path tempPath = Instantiate(p, transform.position, Quaternion.identity);
                pathMap[i, j] = tempPath;
                tempPath.fillPath(9);
            }
        }
        Path newPath = Instantiate(p, transform.position, Quaternion.identity);
        newPath.whoseSide = unitAllegiance;
        possibleTargets = new List<Unit>();
        if (negStatus[2] > 0)
        {
            Pathfinder.pf.drawPath(this, position, mvt + currEquip.tempMvt - 1, newPath, unitAllegiance);
        }
        else
        {
            Pathfinder.pf.drawPath(this, position, mvt + currEquip.tempMvt, newPath, unitAllegiance);
        }
        newPath.currentTile = true;
        newPath.set = true;
        pathMap[position[0], position[1]] = newPath;
        //Now MOVE.
        findTargets();
        clearPaths();
    }


    public void clearPaths()
    {
        //Removes all paths on a unit.
        for (int i = 0; i < pathMap.GetLength(0); i++)
        {
            for (int j = 0; j < pathMap.GetLength(1); j++)
            {
                if (pathMap[i, j] != null)
                {
                    pathMap[i, j].suicide();
                }
            }
        }
    }

    public void showMovement()
    {
        //Disables all tiles that can be moved to. Will be changed to tint later.
        if (unitAllegiance == 1)
        {
            for (int i = 0; i < pathMap.GetLength(0); i++)
            {
                for (int j = 0; j < pathMap.GetLength(1); j++)
                {
                    if (pathMap[i, j].set == true)
                    {
                        if (pathMap[i, j].path.Count >= 0 || pathMap[i, j].currentTile)
                        {
                            Controller.c.currMap.grid[i, j].indicatorOL.gameObject.SetActive(true);
                            Controller.c.currMap.grid[i, j].iolsr.sprite = Controller.c.currMap.grid[i, j].indicOLSpriteList[0];
                        }
                    }
                }
            }
        }
    }

    public void hideMovement()
    {
        //Inverse of showMovement.
        if (unitAllegiance == 1)
        {
            for (int i = 0; i < pathMap.GetLength(0); i++)
            {
                for (int j = 0; j < pathMap.GetLength(1); j++)
                {
                    if (pathMap[i, j].set == true)
                    {
                        if (pathMap[i, j].path.Count >= 0)
                        {
                            Controller.c.currMap.grid[i, j].indicatorOL.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }
    }

    public void die()
    {
        //Self-explanatory.
        if (hp <= 0)
        {
            hp = 0;
            isDead = true;
            Controller.c.unitMap[position[0], position[1]] = 0;
            hasMoved = true;
            //If kamikaze, boom.
            if (checkMod(3, 2))
            {
                //Hits in a 3x3 radius around the character.
                //Check location. We have the position, so we need min/max X/Y.
                int minExpX = position[0] - 1;
                int maxExpX = position[0] + 1;
                int minExpY = position[1] - 1;
                int maxExpY = position[1] + 1;
                //Edge cases: next to a tile bound.
                if (minExpX < 0)
                {
                    minExpX = 0;
                }
                if (minExpY < 0)
                {
                    minExpY = 0;
                }
                if (maxExpX > Controller.c.currMap.xBound)
                {
                    maxExpX = Controller.c.currMap.xBound;
                }
                if (minExpY > Controller.c.currMap.yBound)
                {
                    maxExpY = Controller.c.currMap.yBound;
                }
                //Given these bounds, check for all units present. Give them hell.
                foreach (Unit u in Controller.c.playerUnits)
                {
                    bool inXRange = (position[0] >= minExpX && position[0] <= maxExpX);
                    bool inYRange = (position[1] >= minExpY && position[1] <= maxExpY);
                    if (inXRange && inYRange)
                    {
                        u.hp -= (hp - u.def);
                        Debug.Log(u.unitName + " took " + (u.def + u.currEquip.tempDef) + "damage!");
                        u.die();
                    }
                }
                foreach (Unit u in Controller.c.enemyUnits)
                {
                    bool inXRange = (position[0] >= minExpX && position[0] <= maxExpX);
                    bool inYRange = (position[1] >= minExpY && position[1] <= maxExpY);
                    if (inXRange && inYRange)
                    {
                        u.hp -= (hp - (u.def+ u.currEquip.tempDef));
                        Debug.Log(u.unitName + " took " + (hp - u.def) + "damage!");
                        u.die();
                    }
                }
            }
            if (unitAllegiance == 2)
            {
                Controller.c.enemyUnits.Remove(this);
                Controller.c.unitMap[position[0], position[1]] = 0;
                Destroy(this.gameObject);
            }
            spr.enabled = false;
            this.gameObject.SetActive(false);
        }
    }

    public void findTargets()
    {
        //Clean the list.
        possibleTargets = new List<Unit>();

        if (unitAllegiance == 1)
        {
            //This is a player unit; search for enemies AFTER moving.
            foreach (Unit u in Controller.c.enemyUnits)
            {
                if (u != null)
                {
                    float distToTarget = Vector2.Distance(transform.position, u.transform.position);
                    if (distToTarget <= currEquip.range)
                    {
                        possibleTargets.Add(u);
                    }
                }
            }
        }
        if (unitAllegiance == 2) 
        {
            //This is an enemy unit; search for players BEFORE moving.
            //First, reset target. Just in case.
            target = null;
            foreach (Unit u in Controller.c.playerUnits)
            {
                if (u != null && !u.isDead)
                {
                    for (int i = 0; i < Controller.c.currMap.xBound; i++)
                    {
                        for (int j = 0; j < Controller.c.currMap.yBound; j++)
                        {
                            if (pathMap[i, j].set && pathMap[i, j] != null)
                            {
                                float distToTarget = Vector2.Distance(new Vector2(i, j), u.transform.position);
                                if (distToTarget <= currEquip.range)
                                {
                                    Debug.Log("Targeting from " + i + "," + j);
                                    possibleTargets.Add(u);
                                }
                            }
                        }
                    }
                }
            }
            //PossibleTargets is now populated. Let's find a target.
            //If the list is empty (no units in range)
            if (possibleTargets.Count == 0)
            {
                //Find a target.
                foreach (Unit u in Controller.c.playerUnits)
                {
                    if (u != null)
                    {
                        if (target == null)
                        {
                            target = u;
                        }
                        else
                        {
                            float distToTarget = Vector2.Distance(transform.position, target.transform.position);
                            float distToPotential = Vector2.Distance(transform.position, u.transform.position);
                            if (distToTarget > distToPotential)
                            {
                                target = u;
                            }
                            else if (distToTarget == distToPotential)
                            {
                                //Target the squishier of the two.
                                int targetHP = target.hp;
                                int pTargetHP = u.hp;
                                int targetDef = target.def;
                                int pTargetDef = u.def;
                                if (targetHP - (currEquip.minDmg - targetDef) > pTargetHP - (currEquip.minDmg - pTargetDef))
                                {
                                    target = u;
                                }
                            }
                        }
                    }
                }
                //Okay, now we have a target. Let's path towards them.
                findRouteToTarget(false);
            }
            //The list has at least ONE unit in range.
            else
            {
                //Find a target.
                foreach (Unit u in possibleTargets)
                {
                    if (u != null)
                    {
                        if (target == null)
                        {
                            target = u;
                        }
                        else
                        {
                            int targetHP = target.hp;
                            int pTargetHP = u.hp;
                            int targetDef = target.def;
                            int pTargetDef = u.def;
                            //Target closest.
                            float distToTarget = Vector2.Distance(transform.position, target.transform.position);
                            float distToPotential = Vector2.Distance(transform.position, u.transform.position);
                            if (distToTarget > distToPotential)
                            {
                                target = u;
                            }
                            //Target possible kill targets
                            else if (distToPotential == distToTarget)
                            {
                                if (targetHP - (currEquip.minDmg - targetDef) > pTargetHP - (currEquip.minDmg - pTargetDef))
                                {
                                    target = u;
                                }
                                else if (targetHP - (currEquip.minDmg - targetDef) == pTargetHP - (currEquip.minDmg - pTargetDef))
                                {
                                    //Target squishy.
                                    if (currEquip.minDmg - targetDef < currEquip.minDmg - pTargetDef)
                                    {
                                        target = u;
                                    }
                                }
                            }
                        }
                    }
                }
                findRouteToTarget(true);
            }
        }
    }

    public void attack()
    {
        //So, we have a target, we have a gun. Use them.
        int hitChance = currEquip.accuracy - (target.eva + target.currEquip.tempEva);
        int randChance = Random.Range(0, 100);
        int dmgTaken = 0;
        bool refundShot = false;
        if (randChance < hitChance)
        {
            //Frontloaded and Backloaded proc inside the damage window, as seen below.
            //We've hit. Check for a crit.
            if (Random.Range(0, 100) <= (lck + currEquip.tempLck) || (target.negStatus[3] > 0 && Random.Range(0, 100) <= (lck + currEquip.tempLck + 10) ))
            {
                Debug.Log("Crit target!");
                dmgTaken = Random.Range(currEquip.minDmg + currEquip.tempMinDmg, currEquip.maxDmg + currEquip.tempMaxDmg);
                if (checkMod(2, 3) && currEquip.currentClip == currEquip.clipSize)
                {
                    dmgTaken = (int)(dmgTaken * 1.3f);
                    Debug.Log("Frontloaded activated!");
                }
                if (checkMod(3, 3) && currEquip.currentClip == 1)
                {
                    dmgTaken = (int)(dmgTaken * 1.5f);
                    Debug.Log("Backloaded activated!");
                }
                if (target.negStatus[3] > 0)
                {
                    dmgTaken = (int) (dmgTaken * 1.25f);
                }
                dmgTaken -= (target.def + target.currEquip.tempDef);
                target.hp -= dmgTaken;
                Debug.Log(target.unitName + " took " + dmgTaken + " damage!");
            }
            else
            {
                Debug.Log("Hit target!");
                dmgTaken = Random.Range(currEquip.minDmg + currEquip.tempMinDmg, currEquip.maxDmg + currEquip.tempMaxDmg);
                if (checkMod(2, 3) && currEquip.currentClip == currEquip.clipSize)
                {
                    dmgTaken = (int)(dmgTaken * 1.3f);
                    Debug.Log("Frontloaded activated!");
                }
                if (checkMod(3, 3) && currEquip.currentClip == 1)
                {
                    dmgTaken = (int)(dmgTaken * 1.5f);
                    Debug.Log("Backloaded activated!");
                }
                if (target.negStatus[3] > 0)
                {
                    dmgTaken = (int) (dmgTaken *1.25f);
                }
                int finalDmg = dmgTaken - (target.def + target.currEquip.tempDef);
                target.showDamage(finalDmg);
                target.hp -= finalDmg;
                Debug.Log(target.unitName + " took " + finalDmg + " damage!");
            }
            //Elec proc.
            if (negStatus[0] > 0 && Random.Range(0,100) < 25 - (statusResist + currEquip.tempRes))
            {
                target.stunned = true;
                Debug.Log("Enemy was stunned!");
            }
            //This is where Stun procs.
            if (checkMod(2, 5))
            {
                if (Random.Range(0, 100) < (statusResist + currEquip.tempRes))
                {
                    target.stunned = true;
                    Debug.Log("Enemy was stunned!");
                }
            }
            //This is where Recycle procs.
            if (checkMod(3, 1))
            {
                if (Random.Range(0, 100) < (lck + currEquip.tempLck))
                {
                    refundShot = true;
                    Debug.Log("Recycle activated!");
                }
            }
            //Check, in order: elec, fire, ice, mark, poison.
            if (checkMod(1, 9))
            {
                if (Random.Range(0, 100) < 20 || negStatus[0] > 0)
                {
                    negStatus[0] = 3;
                    Debug.Log("Electric activated!");
                }
            }
            if (checkMod(1, 10))
            {
                if (Random.Range(0, 100) < 20 || negStatus[1] > 0)
                {
                    negStatus[1] = 3;
                    Debug.Log("Fire activated!");
                }
            }
            if (checkMod(1, 11) || negStatus[2] > 0)
            {
                if (Random.Range(0, 100) < 20)
                {
                    negStatus[2] = 3;
                    Debug.Log("Ice activated!");
                }
            }
            if (checkMod(1, 12) || negStatus[3] > 0)
            {
                if (Random.Range(0, 100) < 20)
                {
                    negStatus[3] = 3;
                    Debug.Log("Mark activated!");
                }
            }
            if (checkMod(1, 13) || negStatus[4] > 0)
            {
                if (Random.Range(0, 100) < 20)
                {
                    negStatus[4] = 3;
                    Debug.Log("Poison activated!");
                }
            }
        }
        else
        {
            Debug.Log("Missed target!");
            if (checkMod(1, 6) && (Random.Range(0, 100) < (lck + currEquip.tempLck) / 2))
            {
                determination = true;
                Debug.Log("Determined activated! Move again!");
            }
        }
        if (!currEquip.isMelee && !refundShot)
        {
            currEquip.currentClip--;
        }
        if (unitAllegiance == 1)
        {
            BattleMenuUI.bmui.updatePlayerValues(this);
            BattleMenuUI.bmui.updateEnemyValues(target);
        }
        else
        {
            BattleMenuUI.bmui.updatePlayerValues(target);
            BattleMenuUI.bmui.updateEnemyValues(this);
        }
        Controller.c.mp.currX = target.position[0];
        Controller.c.mp.currY = target.position[1];
        Controller.c.mp.transform.position = new Vector3(Controller.c.mp.currX, Controller.c.mp.currY + .5f, -3);
        target.die();
    }


    public void addItemToInv(int convoyIndex)
    {
        Item tempItem = InvManager.im.convoy[convoyIndex];
        InvManager.im.convoy.RemoveAt(convoyIndex);
        inventory.Add(tempItem);
    }

    public void swapGun(int armoryIndex)
    {
        //Get the gun at location and save it.
        Item tempGun = InvManager.im.armory[armoryIndex];
        InvManager.im.armory.RemoveAt(armoryIndex);
        InvManager.im.armory.Add(currEquip);
        currEquip = tempGun;
    }

    //given the pathmap, find a route to get to within attack range of the target.
    public void findRouteToTarget(bool inRange)
    {
        if (inRange)
        {
            //Check every set location on the map
            Path chosenPath = null;
            for (int i = 0; i < Controller.c.currMap.xBound; i++)
            {
                for (int j = 0; j < Controller.c.currMap.yBound; j++)
                {
                    if (pathMap[i, j].set)
                    {
                        if (chosenPath == null)
                        {
                            Vector3 tempLoc = new Vector3(i, j, -1);
                            //Check distance; can the target be shot?
                            float distToTarget = Vector2.Distance(tempLoc, target.transform.position);
                            if (distToTarget <= currEquip.range)
                            {
                                chosenPath = pathMap[i, j];
                            }
                        }
                        else
                        {
                            Vector3 tempLoc = new Vector3(i, j, -1);
                            //Check distance; can the target be shot?
                            float distToTarget = Vector2.Distance(tempLoc, target.transform.position);
                            if ((distToTarget <= currEquip.range) && pathMap[i,j].hazardCount < chosenPath.hazardCount && chosenPath.path.Count > savedPath.Count)
                            {
                                chosenPath = pathMap[i, j];
                            }
                        }
                    }
                }
            }
            //Kick off processPath
            savedPath = chosenPath.path;
            timer = 15;
            procPath = true;
            nextIndex = 0;
        }
        else
        {
            //Have fun chasing your target.
            //We'll make our own path.
            if (!Controller.c.saidWL)
            {
                onTheHunt(target);
                timer = 15;
                procPath = true;
                nextIndex = 0;
            }
        }
    }

    public void processPath()
    {
        //Reminder:
        //0 = up, 1 = right, 2 = down, 3 = left
        //Test for enemies
        //Current problem: the function recurses. 
        if (unitAllegiance == 2)
        {
            if (savedPath.Count > nextIndex)
            {
                Controller.c.unitMap[position[0], position[1]] = 0;
                //TODO: Process tiles individually for effects.
                switch (savedPath[nextIndex])
                {
                    case 0:
                        //Move up one
                        position[1]++;
                        transform.position += new Vector3(0, 1, 0);
                        break;
                    case 1:
                        //Move right one
                        position[0]++;
                        transform.position += new Vector3(1, 0, 0);
                        break;
                    case 2:
                        //Move down one
                        position[1]--;
                        transform.position -= new Vector3(0, 1, 0);
                        break;
                    case 3:
                        //Move left one
                        position[0]--;
                        transform.position -= new Vector3(1, 0, 0);
                        break;
                }
                timer = 15;
                nextIndex++;
            }
            else
            {
                //Count == 0
                //In other words, we've reached our destination. Time to shoot.
                Controller.c.unitMap[position[0], position[1]] = 2;
                float distToTarget = Vector2.Distance(transform.position, target.transform.position);
                if (distToTarget <= currEquip.range)
                {
                    attack();
                }
                hasMoved = true;
                procPath = false;
                savedPath = null;
            }
        }
    }

    public bool checkMod(int modTier, int modID)
    {
        for (int i = 0; i < 3; i++)
        {
            if (currEquip.mods[i, 0] == modTier && currEquip.mods[i, 1] == modID)
            {
                return true;
            }
        }
        return false;
    }

    public void setUnitStats(int newHP, int newEva, int newDef, int newLck, int newMvt, int newRes)
    {
        hp = newHP;
        maxhp = newHP;
        eva = newEva;
        def = newDef;
        lck = newLck;
        mvt = newMvt;
        statusResist = newRes;
    }

    public void setUnitWeaponStats(int newMinDmg, int newMaxDmg, int newClip, int newAcc, int newRng, int newHealAmt, int newTempAcc, int newTempEva, int newTempDef, int newTempLck, int newTempRes, int newTempMin, int newTempMax)
    {
        currEquip.minDmg = newMinDmg;
        currEquip.maxDmg = newMaxDmg;
        currEquip.clipSize = newClip;
        currEquip.currentClip = newClip;
        currEquip.accuracy = newAcc;
        currEquip.range = newRng;
        currEquip.healAmt = newHealAmt;
        currEquip.tempAcc = newTempAcc;
        currEquip.tempDef = newTempDef;
        currEquip.tempEva = newTempEva;
        currEquip.tempLck = newTempLck;
        currEquip.tempRes = newTempRes;
        currEquip.tempMinDmg = newTempMin;
        currEquip.tempMaxDmg = newTempMax;
    }

    public void resetChar()
    {
        isDead = false;
        hp = maxhp;
        currEquip.currentClip = currEquip.clipSize;
        hasMoved = false;
        stunned = false;
        currUnit = false;
        target = null;
        spr.enabled = true;
    }

    public void showDamage(int dmgTaken)
    {
        if (dmgTaken < 10 && dmgTaken > 0)
        {
            //Single digit only.
            holder2.gameObject.SetActive(true);
            ones.sprite = Controller.c.damageNumbers[dmgTaken];
            showDamageTimer = 30;
        }
        else if (dmgTaken > 9)
        {
            //Double digits.
            holder.gameObject.SetActive(true);
            int onesValue = dmgTaken % 10;
            int tensValue = dmgTaken / 10;
            tenOnes.sprite = Controller.c.damageNumbers[onesValue];
            tens.sprite = Controller.c.damageNumbers[tensValue];
            showDamageTimer = 30;
        }
    }

    public void onTheHunt(Unit targetUnit)
    {
        //In this scenario, the player MUST be out of range.
        //Ergo, we move toward the player up to the amount of tiles we can move (aka mvt)
        Pathfinder.pf.drawPath(this, position, 10, pathMap[position[0], position[1]], unitAllegiance);
        //So let's say we have a theoretical max of 10 tiles to move. Next thing to do? Move towards the target.
        Path chosenPath = null;
        for (int i = 0; i < Controller.c.currMap.xBound; i++)
        {
            for (int j = 0; j < Controller.c.currMap.yBound; j++)
            {
                if (pathMap[i, j].set)
                {
                    if (chosenPath == null)
                    {
                        Vector3 tempLoc = new Vector3(i, j, -1);
                        //Check distance; can the target be shot?
                        float distToTarget = Vector2.Distance(tempLoc, target.transform.position);
                        if (distToTarget <= currEquip.range)
                        {
                            chosenPath = pathMap[i, j];
                        }
                    }
                    else
                    {
                        Vector3 tempLoc = new Vector3(i, j, -1);
                        //Check distance; can the target be shot?
                        float distToTarget = Vector2.Distance(tempLoc, target.transform.position);
                        if ((distToTarget <= currEquip.range) && pathMap[i, j].hazardCount < chosenPath.hazardCount && chosenPath.path.Count > savedPath.Count)
                        {
                            chosenPath = pathMap[i, j];
                        }
                    }
                }
            }
        }
        //Now that we have a path, let's cull it a bit.
        //They're out of range, so let's shave the list down a bit.
        if (negStatus[2] > 0)
        {
            savedPath = chosenPath.path.GetRange(0, mvt + currEquip.tempMvt - 1);
        }
        else
        {
            savedPath = chosenPath.path.GetRange(0, mvt + currEquip.tempMvt);
        }
    }

    public void tickDownStatus()
    {
        //Statuses, in order: para, burn, freeze, mark, poison
        for (int i = 0; i < negStatus.Length; i++)
        {
            if (negStatus[i] > 0)
            {
                if (i == 4)
                {
                    hp -= (int)(maxhp * .1f);
                    die();
                }
                i--;
            }
        }
    }
}
