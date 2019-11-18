using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public int hp, maxhp, spd, eva, def, lck, mvt, unitAllegiance;
    public int[] position;
    public int[] lastPosition;
    public bool isDead, hasMoved, currUnit, stunned;
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
        if (Controller.c.unitMap[position[0], position[1]] != unitAllegiance)
        {
            Controller.c.unitMap[position[0], position[1]] = unitAllegiance;
        }
    }

    public void startFinding()
    {
        //Test function; check for hazards. Player default.
        //if (currUnit)Input.GetKeyDown(KeyCode.C) && 

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
        Pathfinder.pf.drawPath(this, position, mvt, newPath, unitAllegiance);
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
        possibleTargets = new List<Unit>();
        Pathfinder.pf.drawPath(this, position, mvt, newPath, unitAllegiance);
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
                pathMap[i, j].suicide();
            }
        }
    }

    public void showMovement()
    {
        //Disables all tiles that can be moved to. Will be changed to tint later.
        for (int i = 0; i < pathMap.GetLength(0); i++)
        {
            for (int j = 0; j < pathMap.GetLength(1); j++)
            {
                if (pathMap[i, j].set == true)
                {
                    if (pathMap[i, j].path.Count >= 0 || pathMap[i, j].currentTile)
                    {
                        Controller.c.currMap.grid[i, j].gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    public void hideMovement()
    {
        //Inverse of showMovement.
        for (int i = 0; i < pathMap.GetLength(0); i++)
        {
            for (int j = 0; j < pathMap.GetLength(1); j++)
            {
                if (pathMap[i, j].set == true)
                {
                    if (pathMap[i, j].path.Count >= 0)
                    {
                        Controller.c.currMap.grid[i, j].gameObject.SetActive(true);
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
            bool kamikazeActive = false;
            foreach (int i in currEquip.mods)
            {
                if (i == 1)
                {
                    kamikazeActive = true;
                }
            }
            if (kamikazeActive)
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
                        Debug.Log(u.unitName + " took " + (hp - u.def) + "damage!");
                        u.die();
                    }
                }
                foreach (Unit u in Controller.c.enemyUnits)
                {
                    bool inXRange = (position[0] >= minExpX && position[0] <= maxExpX);
                    bool inYRange = (position[1] >= minExpY && position[1] <= maxExpY);
                    if (inXRange && inYRange)
                    {
                        u.hp -= (hp - u.def);
                        Debug.Log(u.unitName + " took " + (hp - u.def) + "damage!");
                        u.die();
                    }
                }
                spr.enabled = false;
                this.gameObject.SetActive(false);
            }
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
            foreach (Unit u in Controller.c.playerUnits)
            {
                if (u != null && !u.isDead)
                {
                    float distToTarget = Vector2.Distance(transform.position, u.transform.position);
                    if (distToTarget <= currEquip.range + mvt)
                    {
                        possibleTargets.Add(u);
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
        int hitChance = currEquip.accuracy - target.eva;
        int randChance = Random.Range(0, 100);
        int dmgTaken = 0;
        bool refundShot = false;
        if (randChance < hitChance)
        {
            //Frontloaded and Backloaded proc inside the damage window, as seen below.
            //We've hit. Check for a crit.
            if (Random.Range(0, 100) <= lck)
            {
                Debug.Log("Crit target!");
                dmgTaken = Random.Range(currEquip.minDmg, currEquip.maxDmg) +currEquip.maxDmg;
                if (checkMod(5) && currEquip.currentClip == currEquip.clipSize)
                {
                    dmgTaken = (int)(dmgTaken * 1.3);
                    Debug.Log("Frontloaded activated!");
                }
                if (checkMod(6) && currEquip.currentClip == 1)
                {
                    dmgTaken = (int)(dmgTaken * 1.5);
                    Debug.Log("Backloaded activated!");
                }
                target.hp -= (dmgTaken - target.def);
                Debug.Log(target.unitName + " took " + dmgTaken + " damage!");
            }
            else
            {
                Debug.Log("Hit target!");
                if (checkMod(5) && currEquip.currentClip == currEquip.clipSize)
                {
                    dmgTaken = (int)(dmgTaken * 1.3);
                    Debug.Log("Frontloaded activated!");
                }
                if (checkMod(6) && currEquip.currentClip == 1)
                {
                    dmgTaken = (int)(dmgTaken * 1.5);
                    Debug.Log("Backloaded activated!");
                }
                dmgTaken = Random.Range(currEquip.minDmg, currEquip.maxDmg);
                target.hp -= (dmgTaken - target.def);
                Debug.Log(target.unitName + " took " + dmgTaken + " damage!");
            }
            //This is where Stun procs.
            if (checkMod(2))
            {
                if (Random.Range(0, 100) < 25)
                {
                    target.stunned = true;
                    Debug.Log("Enemy was stunned!");
                }
            }
            //This is where Recycle procs.
            if (checkMod(3))
            {
                if (Random.Range(0, 100) < lck)
                {
                    refundShot = true;
                    Debug.Log("Recycle activated!");
                }
            }
        }
        else
        {
            Debug.Log("Missed target!");
        }
        if (!currEquip.isMelee && !refundShot)
        {
            currEquip.currentClip--;
        }
        BattleMenuUI.bmui.updatePlayerValues(BattleMenuUI.bmui.currentPlayer);
        BattleMenuUI.bmui.updateEnemyValues(BattleMenuUI.bmui.currentEnemy);
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
                            if ((distToTarget <= currEquip.range) && pathMap[i,j].hazardCount < chosenPath.hazardCount)
                            {
                                chosenPath = pathMap[i, j];
                            }
                        }
                    }
                }
            }
            //Kick off processPath
            processPath(chosenPath);
        }
        else
        {
            //Have fun chasing your target.
            //We'll make our own path.
            Debug.Log(target.unitName);
            /*clearPaths();
            Path newPath = Instantiate(p, transform.position, Quaternion.identity);
            Pathfinder.pf.drawPath(this, position, 20, newPath, unitAllegiance);*/

            //For now, though? Do nothing.
            hasMoved = true;
            Controller.c.checkTurn();
        }
    }

    public void processPath(Path route)
    {
        //Reminder:
        //0 = up, 1 = right, 2 = down, 3 = left
        //Test for enemies
        if (unitAllegiance == 2)
        {
            if (route.path.Count > 0)
            {
                Controller.c.unitMap[position[0], position[1]] = 0;
                //TODO: Process tiles individually for effects.
                switch (route.path[0])
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
                route.path.Remove(route.path[0]);
                processPath(route);
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
            }
        }
    }

    public bool checkMod(int modID)
    {
        foreach (int i in currEquip.mods)
        {
            if (modID == i)
            {
                return true;
            }
        }
        return false;
    }
}
