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
    public Path[,] pathMap, pathMap2;
    public Unit target;
    public List<Unit> possibleTargets = new List<Unit>();
    public bool[] availableOptions = new bool[4];
    public SpriteRenderer spr;
    public Sprite unitFace, unitFace2;
    public string unitName = "temp";
    //Statuses, in order: elec, burn, freeze, mark, poison
    public int[] negStatus = new int[5];

    public GameObject holder, holder2, missed;
    public SpriteRenderer modifierA, tens, tenOnes, modifierB, ones, missSpr;

    public int nextIndex = 0;
    public List<int> savedPath = new List<int>();
    public bool procPath = false;
    int timer = 15;
    public int showDamageTimer = 0;

    public Color[] colorCycle;
    public int currentStatus, currentStatusTimer;

    bool isDying = false;
    int dyingFade = 45;

    //For indicators; enemy only.
    public bool displayActive = false;

    public Animator myAnim;

    public AudioClip hitSound;

    // Start is called before the first frame update
    void Start()
    {
       
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
            if (showDamageTimer <= 0)
            {
                holder.gameObject.SetActive(false);
                holder2.gameObject.SetActive(false);
                missed.gameObject.SetActive(false);
            }
            else
            {
                float moveDist = .25f / 45;
                holder.gameObject.transform.localPosition += new Vector3(0, moveDist, 0);
                holder2.gameObject.transform.localPosition += new Vector3(0, moveDist, 0);
                missed.gameObject.transform.localPosition += new Vector3(0, moveDist, 0);
                //Color transparency down.
                float fadeLeft = 1 - ((1f/45f) * (45 - showDamageTimer));
                ones.color = new Color(1f, 1f, 1f, fadeLeft);
                tens.color = new Color(1f, 1f, 1f, fadeLeft);
                tenOnes.color = new Color(1f, 1f, 1f, fadeLeft);
                modifierA.color = new Color(1f, 1f, 1f, fadeLeft);
                modifierB.color = new Color(1f, 1f, 1f, fadeLeft);
                missSpr.color = new Color(1f, 1f, 1f, fadeLeft);
                showDamageTimer--;
            }
            //Dying
            if (isDying)
            {
                if (dyingFade > 0)
                {
                    float fadeLeft = 1 - ((1f / 45f) * (45 - dyingFade));
                    spr.color = new Color(1f, 1f, 1f, fadeLeft);
                }
                else
                {
                    die();
                }
                dyingFade--;
            }
            else
            {
                cycleColors();
            }
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
        newPath.set = true;
        Pathfinder.pf.drawPath(pathMap, position, mvt + currEquip.tempMvt, newPath, unitAllegiance, currEquip.range);
        newPath.currentTile = true;
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
        newPath.set = true;
        possibleTargets = new List<Unit>();
        if (negStatus[2] > 0)
        {
            Pathfinder.pf.drawPath(pathMap, position, mvt + currEquip.tempMvt - 1, newPath, unitAllegiance, 0);
        }
        else
        {
            Pathfinder.pf.drawPath(pathMap, position, mvt + currEquip.tempMvt, newPath, unitAllegiance, 0);
        }
        newPath.currentTile = true;
        pathMap[position[0], position[1]] = newPath;
        //Now MOVE.
        findTargets();
        clearPaths(pathMap);
    }


    public void clearPaths(Path[,] givenPathMap)
    {
        //Removes all paths on a unit.
        for (int i = 0; i < givenPathMap.GetLength(0); i++)
        {
            for (int j = 0; j < givenPathMap.GetLength(1); j++)
            {
                if (givenPathMap[i, j] != null)
                {
                    givenPathMap[i, j].suicide();
                }
            }
        }
    }

    public void showMovement()
    {
        //Shows all tiles that can be moved to. Will be changed to tint later.
        if (unitAllegiance == 1)
        {
            for (int i = 0; i < pathMap.GetLength(0); i++)
            {
                for (int j = 0; j < pathMap.GetLength(1); j++)
                {
                    if (pathMap[i, j].set)
                    {
                        if (pathMap[i, j].path.Count >= 0 || pathMap[i, j].currentTile)
                        {
                            Controller.c.currMap.grid[i, j].indicatorOL.gameObject.SetActive(true);
                            Controller.c.currMap.grid[i, j].iolsr.sprite = Controller.c.currMap.grid[i, j].indicOLSpriteList[0];
                        }
                    }
                    if (pathMap[i, j].setAtk)
                    {
                        Controller.c.currMap.grid[i, j].indicatorOL.gameObject.SetActive(true);
                        Controller.c.currMap.grid[i, j].iolsr.sprite = Controller.c.currMap.grid[i, j].indicOLSpriteList[1];
                    }
                }
            }
        }
        else
        {
            //Enemy unit. Thus:
            pathMap2 = new Path[Controller.c.currMap.xBound, Controller.c.currMap.yBound];
            for (int i = 0; i < Controller.c.currMap.xBound; i++)
            {
                for (int j = 0; j < Controller.c.currMap.yBound; j++)
                {
                    Path tempPath = Instantiate(p, transform.position, Quaternion.identity);
                    pathMap2[i, j] = tempPath;
                    tempPath.fillPath(9);
                }
            }
            Path newPath = Instantiate(p, transform.position, Quaternion.identity);
            newPath.whoseSide = unitAllegiance;
            newPath.setAtk = true;
            Pathfinder.pf.drawPath(pathMap2, position, mvt + currEquip.tempMvt, newPath, unitAllegiance, currEquip.range);
            newPath.currentTile = true;
            pathMap2[position[0], position[1]] = newPath;
            //Reveal.
            for (int i = 0; i < pathMap2.GetLength(0); i++)
            {
                for (int j = 0; j < pathMap2.GetLength(1); j++)
                {
                    if (pathMap2[i, j].setAtk)
                    {
                        pathMap2[i, j].tempImmune = true;
                        Controller.c.currMap.grid[i, j].indicatorOL2.gameObject.SetActive(true);
                        Controller.c.currMap.grid[i, j].iolsr2.sprite = Controller.c.currMap.grid[i, j].indicOLSpriteList[2];
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
                    if (pathMap[i, j].set)
                    {
                        if (pathMap[i, j].path.Count >= 0)
                        {
                            Controller.c.currMap.grid[i, j].indicatorOL.gameObject.SetActive(false);
                        }
                    }
                    if (pathMap[i, j].setAtk)
                    {
                        Controller.c.currMap.grid[i, j].indicatorOL.gameObject.SetActive(false);
                    }
                }
            }
        }
        else
        {
            //Check all active enemies.
            bool[,] tempCheck = new bool[pathMap2.GetLength(0), pathMap2.GetLength(1)];
            foreach (Unit u in Controller.c.enemyUnits)
            {
                if (u.displayActive && u != this)
                {
                    for (int i = 0; i < u.pathMap2.GetLength(0); i++)
                    {
                        for (int j = 0; j < u.pathMap2.GetLength(1); j++)
                        {
                            if (u.pathMap2[i, j].setAtk)
                            {
                                tempCheck[i, j] = true;
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < pathMap2.GetLength(0); i++)
            {
                for (int j = 0; j < pathMap2.GetLength(1); j++)
                {
                    if (pathMap2[i, j].setAtk && !(tempCheck[i,j]))
                    {
                        Controller.c.currMap.grid[i, j].indicatorOL2.gameObject.SetActive(false);
                    }
                }
            }
            clearPaths(pathMap2);
        }
    }

    public void showAtkRange()
    {
        //Generate the current attack position overlay.
        if (unitAllegiance == 1)
        {
            pathMap2 = new Path[Controller.c.currMap.xBound, Controller.c.currMap.yBound];
            for (int i = 0; i < Controller.c.currMap.xBound; i++)
            {
                for (int j = 0; j < Controller.c.currMap.yBound; j++)
                {
                    Path tempPath = Instantiate(p, transform.position, Quaternion.identity);
                    pathMap2[i, j] = tempPath;
                    tempPath.fillPath(9);
                }
            }
            Path newPath = Instantiate(p, transform.position, Quaternion.identity);
            newPath.whoseSide = unitAllegiance;
            newPath.setAtk = true;
            Pathfinder.pf.drawPath(pathMap2, position, 0, newPath, unitAllegiance, currEquip.range);
            newPath.currentTile = true;
            pathMap2[position[0], position[1]] = newPath;
            //Reveal.
            for (int i = 0; i < pathMap2.GetLength(0); i++)
            {
                for (int j = 0; j < pathMap2.GetLength(1); j++)
                {
                    if (pathMap2[i, j].setAtk)
                    {
                        Controller.c.currMap.grid[i, j].indicatorOL.gameObject.SetActive(true);
                        Controller.c.currMap.grid[i, j].iolsr.sprite = Controller.c.currMap.grid[i, j].indicOLSpriteList[1];
                    }
                }
            }
        }
    }

    public void hideAtkRange()
    {
        if (unitAllegiance == 1)
        {
            for (int i = 0; i < pathMap2.GetLength(0); i++)
            {
                for (int j = 0; j < pathMap2.GetLength(1); j++)
                {
                    if (pathMap2[i, j].setAtk)
                    {
                        Controller.c.currMap.grid[i, j].indicatorOL.gameObject.SetActive(false);
                    }
                }
            }
            clearPaths(pathMap2);
        }
    }

    public void die()
    {
        //Self-explanatory.
        if (hp <= 0 && !isDying)
        {
            hp = 0;
            isDying = true;
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
                    if (inXRange && inYRange && u != this)
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
                    if (inXRange && inYRange && u != this)
                    {
                        u.hp -= (hp - (u.def + u.currEquip.tempDef));
                        Debug.Log(u.unitName + " took " + (hp - u.def) + "damage!");
                        u.die();
                    }
                }
            }
            Controller.c.playSound(Controller.c.sfx[5]);
            Controller.c.enemyUnits.Remove(this);
        }
        if (dyingFade <= 0)
        {
            if (unitAllegiance == 2)
            {
                Controller.c.enemyUnits.Remove(this);
                Controller.c.unitMap[position[0], position[1]] = 0;
                Destroy(this.gameObject);
            }
            spr.enabled = false;
            spr.color = new Color(1f, 1f, 1f, 1f);
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
                    int xToTarget = Mathf.Abs(position[0] - u.position[0]);
                    int yToTarget = Mathf.Abs(position[1] - u.position[1]);
                    int distToTarget = xToTarget + yToTarget;
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
                if (u != null && !u.isDead && !u.isDying)
                {
                    for (int i = 0; i < Controller.c.currMap.xBound; i++)
                    {
                        for (int j = 0; j < Controller.c.currMap.yBound; j++)
                        {
                            if (pathMap[i, j].set && pathMap[i, j] != null)
                            {
                                int xToTarget = Mathf.Abs(i - u.position[0]);
                                int yToTarget = Mathf.Abs(j - u.position[1]);
                                int distToTarget = xToTarget + yToTarget;
                                if (distToTarget <= currEquip.range)
                                {
                                    //Debug.Log("Targeting from " + i + "," + j);
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
                        if (target == null && !u.isDead && !u.isDying)
                        {
                            target = u;
                        }
                        else if (!u.isDead && !u.isDying)
                        {
                            int xToTarget = Mathf.Abs(position[0] - target.position[0]);
                            int yToTarget = Mathf.Abs(position[1] - target.position[1]);
                            int distToTarget = xToTarget + yToTarget;
                            int xToPotential = Mathf.Abs(position[0] - u.position[0]);
                            int yToPotential = Mathf.Abs(position[1] - u.position[1]);
                            int distToPotential = xToPotential + yToPotential;
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
                            int xToTarget = Mathf.Abs(position[0] - target.position[0]);
                            int yToTarget = Mathf.Abs(position[1] - target.position[1]);
                            int distToTarget = xToTarget + yToTarget;
                            int xToPotential = Mathf.Abs(position[0] - u.position[0]);
                            int yToPotential = Mathf.Abs(position[1] - u.position[1]);
                            int distToPotential = xToPotential + yToPotential;
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
        if (randChance <= hitChance)
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
                if (dmgTaken < 0)
                {
                    dmgTaken = 0;
                }
                target.showDamage(dmgTaken);
                target.hp -= dmgTaken;
                Debug.Log(target.unitName + " took " + dmgTaken + " damage!");
                if (dmgTaken > 0)
                {
                    Controller.c.playSound(target.hitSound);
                }
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
                if (finalDmg < 0)
                {
                    finalDmg = 0;
                }
                target.showDamage(finalDmg);
                target.hp -= finalDmg;
                if (finalDmg > 0)
                {
                    Controller.c.playSound(target.hitSound);
                }
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
            target.showMiss();
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
        Controller.c.playSound(currEquip.useSound);
        target.showStatus();
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
            int xFromTarget = Mathf.Abs(position[0] - target.position[0]);
            int yFromTarget = Mathf.Abs(position[1] - target.position[1]);
            if (xFromTarget + yFromTarget > currEquip.range)
            {
                for (int i = 0; i < Controller.c.currMap.xBound; i++)
                {
                    for (int j = 0; j < Controller.c.currMap.yBound; j++)
                    {
                        if (pathMap[i, j].set)
                        {
                            if (chosenPath == null)
                            {
                                //Check distance; can the target be shot?
                                int xToTarget = Mathf.Abs(i - target.position[0]);
                                int yToTarget = Mathf.Abs(j - target.position[1]);
                                int distToTarget = xToTarget + yToTarget;
                                if (distToTarget <= currEquip.range && Controller.c.unitMap[i, j] == 0)
                                {
                                    chosenPath = pathMap[i, j];
                                }
                            }
                            else
                            {
                                //Check distance; can the target be shot?
                                int xToTarget = Mathf.Abs(i - target.position[0]);
                                int yToTarget = Mathf.Abs(j - target.position[1]);
                                int distToTarget = xToTarget + yToTarget;
                                if (distToTarget <= currEquip.range && pathMap[i, j].hazardCount < chosenPath.hazardCount && chosenPath.path.Count > savedPath.Count && Controller.c.unitMap[i, j] == 0)
                                {
                                    chosenPath = pathMap[i, j];
                                }
                            }
                        }
                    }
                }
            }
            //Kick off processPath
            if (chosenPath != null)
            {
                savedPath = chosenPath.path;
                timer = 15;
                procPath = true;
                nextIndex = 0;
            }
            else
            {
                if (!target.isDead && !target.isDying)
                {
                    attack();
                }
                hasMoved = true;
                procPath = false;
                savedPath = null;
            }
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
                        if (Controller.c.unitMap[position[0], position[1] + 1] == 0)
                        {
                            position[1]++;
                            transform.position += new Vector3(0, 1, 0);
                        }
                        else
                        {
                            //End the chase.
                            Controller.c.unitMap[position[0], position[1]] = 2;
                            int xToTarget = Mathf.Abs(position[0] - target.position[0]);
                            int yToTarget = Mathf.Abs(position[1] - target.position[1]);
                            int distToTarget = xToTarget + yToTarget;
                            if (distToTarget <= currEquip.range && !target.isDead)
                            {
                                attack();
                            }
                            Controller.c.playSound(Controller.c.sfx[13]);
                            hasMoved = true;
                            procPath = false;
                            savedPath = null;
                        }
                        break;
                    case 1:
                        //Move right one
                        if (Controller.c.unitMap[position[0] + 1, position[1]] == 0)
                        {
                            position[0]++;
                            transform.position += new Vector3(1, 0, 0);
                        }
                        else
                        {
                            //End the chase.
                            Controller.c.unitMap[position[0], position[1]] = 2;
                            int xToTarget = Mathf.Abs(position[0] - target.position[0]);
                            int yToTarget = Mathf.Abs(position[1] - target.position[1]);
                            int distToTarget = xToTarget + yToTarget;
                            if (distToTarget <= currEquip.range && !target.isDead)
                            {
                                attack();
                            }
                            hasMoved = true;
                            procPath = false;
                            savedPath = null;
                        }
                        break;
                    case 2:
                        //Move down one
                        if (Controller.c.unitMap[position[0], position[1] - 1] == 0)
                        {
                            position[1]--;
                            transform.position -= new Vector3(0, 1, 0);
                        }
                        else
                        {
                            //End the chase.
                            Controller.c.unitMap[position[0], position[1]] = 2;
                            int xToTarget = Mathf.Abs(position[0] - target.position[0]);
                            int yToTarget = Mathf.Abs(position[1] - target.position[1]);
                            int distToTarget = xToTarget + yToTarget;
                            if (distToTarget <= currEquip.range && !target.isDead)
                            {
                                attack();
                            }
                            hasMoved = true;
                            procPath = false;
                            savedPath = null;
                        }
                        break;
                    case 3:
                        //Move left one
                        if (Controller.c.unitMap[position[0] - 1, position[1]] == 0)
                        {
                            position[0]--;
                            transform.position -= new Vector3(1, 0, 0);
                        }
                        else
                        {
                            //End the chase.
                            Controller.c.unitMap[position[0], position[1]] = 2;
                            int xToTarget = Mathf.Abs(position[0] - target.position[0]);
                            int yToTarget = Mathf.Abs(position[1] - target.position[1]);
                            int distToTarget = xToTarget + yToTarget;
                            if (distToTarget <= currEquip.range && !target.isDead)
                            {
                                attack();
                            }
                            hasMoved = true;
                            procPath = false;
                            savedPath = null;
                        }
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
                int xToTarget = Mathf.Abs(position[0] - target.position[0]);
                int yToTarget = Mathf.Abs(position[1] - target.position[1]);
                int distToTarget = xToTarget + yToTarget;
                if (distToTarget <= currEquip.range && !target.isDead)
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
        for (int i = 0; i < currEquip.mods.GetLength(0); i++)
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
        this.gameObject.SetActive(true);
        isDead = false;
        isDying = false;
        dyingFade = 45;
        hp = maxhp;
        currEquip.currentClip = currEquip.clipSize;
        hasMoved = false;
        stunned = false;
        currUnit = false;
        target = null;
        spr.enabled = true;
        showDamageTimer = 0;
        holder.gameObject.SetActive(false);
        holder2.gameObject.SetActive(false);
        missed.gameObject.SetActive(false);
        for (int i = 0; i < negStatus.Length; i++)
        {
            negStatus[i] = 0;
        }
    }

    public void showDamage(int dmgTaken)
    {
        if (dmgTaken < 10)
        {
            if (dmgTaken < 1)
            {
                dmgTaken = 0;
            }
            //Single digit only.
            holder2.gameObject.transform.localPosition = Vector3.zero;
            holder2.gameObject.SetActive(true);
            modifierB.sprite = Controller.c.damageNumbers[10];
            ones.sprite = Controller.c.damageNumbers[dmgTaken];
        }
        else
        {
            //Double digits.
            holder.gameObject.transform.localPosition = Vector3.zero;
            holder.gameObject.SetActive(true);
            int onesValue = dmgTaken % 10;
            int tensValue = dmgTaken / 10;
            modifierA.sprite = Controller.c.damageNumbers[10];
            tenOnes.sprite = Controller.c.damageNumbers[onesValue];
            tens.sprite = Controller.c.damageNumbers[tensValue];
        }
        showDamageTimer = 45;
        Controller.c.dmgDelayTimer = 45;
    }

    public void showMiss()
    {
        missed.gameObject.transform.localPosition = new Vector3(0, .48f, -1f);
        missed.gameObject.SetActive(true);
        showDamageTimer = 45;
        Controller.c.dmgDelayTimer = 45;

    }

    public void showHealing(int amtHealed)
    {
        if (amtHealed < 10)
        {
            //Single digit only.
            holder2.gameObject.transform.localPosition = Vector3.zero;
            holder2.gameObject.SetActive(true);
            modifierB.sprite = Controller.c.healNumbers[10];
            ones.sprite = Controller.c.healNumbers[amtHealed];
        }
        else
        {
            //Double digits.
            holder.gameObject.transform.localPosition = Vector3.zero;
            holder.gameObject.SetActive(true);
            int onesValue = amtHealed % 10;
            int tensValue = amtHealed / 10;
            modifierA.sprite = Controller.c.healNumbers[10];
            tenOnes.sprite = Controller.c.healNumbers[onesValue];
            tens.sprite = Controller.c.healNumbers[tensValue];
        }
        showDamageTimer = 45;
        Controller.c.dmgDelayTimer = 45;
    }

    public void onTheHunt(Unit targetUnit)
    {
        //In this scenario, the player MUST be out of range.
        //Ergo, we move toward the player up to the amount of tiles we can move (aka mvt)
        Pathfinder.pf.drawPath(pathMap, position, 10, pathMap[position[0], position[1]], unitAllegiance, 0);
        //So let's say we have a theoretical max of 10 tiles to move. Next thing to do? Move towards the target.
        Path chosenPath = null;
        Path backupPath = null;
        float backupPathDist = 3;
        for (int i = 0; i < Controller.c.currMap.xBound; i++)
        {
            for (int j = 0; j < Controller.c.currMap.yBound; j++)
            {
                if (pathMap[i, j].set)
                {
                    if (chosenPath == null)
                    {
                        //Check distance; can the target be shot?
                        int xToTarget = Mathf.Abs(i - target.position[0]);
                        int yToTarget = Mathf.Abs(j - target.position[1]);
                        int distToTarget = xToTarget + yToTarget;
                        if (distToTarget <= currEquip.range && Controller.c.unitMap[i, j] == 0)
                        {
                            chosenPath = pathMap[i, j];
                        }
                        else
                        {
                            if (backupPath == null)
                            {
                                backupPath = pathMap[i, j];
                                Vector2 tempA = new Vector2(i, j);
                                Vector2 tempB = new Vector2(target.position[0], target.position[1]);
                                backupPathDist = Vector2.Distance(tempA, tempB);
                            }
                            else
                            {
                                Vector2 tempA = new Vector2(i, j);
                                Vector2 tempB = new Vector2(target.position[0], target.position[1]);
                                if (Vector2.Distance(tempA, tempB) < backupPathDist)
                                {
                                    backupPath = pathMap[i, j];
                                    backupPathDist = Vector2.Distance(tempA, tempB);
                                }
                            }
                        }
                    }
                    else
                    {
                        //Check distance; can the target be shot?
                        int xToTarget = Mathf.Abs(i - target.position[0]);
                        int yToTarget = Mathf.Abs(j - target.position[1]);
                        int distToTarget = xToTarget + yToTarget;
                        if ((distToTarget <= currEquip.range) && pathMap[i, j].hazardCount < chosenPath.hazardCount && chosenPath.path.Count > savedPath.Count && Controller.c.unitMap[i, j] == 0)
                        {
                            chosenPath = pathMap[i, j];
                        }
                    }
                }
            }
        }
        if (chosenPath != null)
        {
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
        else
        {
            if (negStatus[2] > 0)
            {
                savedPath = backupPath.path.GetRange(0, mvt + currEquip.tempMvt - 1);
            }
            else
            {
                savedPath = backupPath.path.GetRange(0, mvt + currEquip.tempMvt);
            }
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
                    showDamage(1);
                    die();
                }
                negStatus[i]--;
            }
        }
        if (stunned)
        {
            stunned = !stunned;
        }
    }

    public void showStatus()
    {
        int colorAmt = 0;
        for (int i = 0; i < negStatus.Length; i++)
        {
            if (negStatus[i] > 0)
            {
                colorAmt++;
            }
        }
        if (stunned)
        {
            colorAmt++;
        }
        if (colorAmt == 0)
        {
            //No statuses.
            colorCycle = new Color[1];
            colorCycle[0] = new Color(1f, 1f, 1f, 1f);
            RuntimeAnimatorController temp = myAnim.runtimeAnimatorController;
            myAnim.runtimeAnimatorController = null;
            spr.color = colorCycle[0];
            myAnim.runtimeAnimatorController = temp;
        }
        else
        {
            int currentIndex = 1;
            colorCycle = new Color[colorAmt + 1];
            colorCycle[0] = new Color(1f, 1f, 1f, 1f);
            //In order, elec, burn, freeze, mark, poison, stun
            if (negStatus[0] > 0)
            {
                colorCycle[currentIndex] = new Color(1f, 1f, 0f, 1f);
                currentIndex++;
            }
            if (negStatus[1] > 0)
            {
                colorCycle[currentIndex] = new Color(1f, .6f, 0f, 1f);
                currentIndex++;
            }
            if (negStatus[2] > 0)
            {
                colorCycle[currentIndex] = new Color(0.6352941f, 0.7607843f, 0.9568627f, 1f);
                currentIndex++;
            }
            if (negStatus[3] > 0)
            {
                colorCycle[currentIndex] = new Color(1f, 0f, 0f, 1f);
                currentIndex++;
            }
            if (negStatus[4] > 0)
            {
                colorCycle[currentIndex] = new Color(.6f, 0f, 1f, 1f);
                currentIndex++;
            }
            if (stunned)
            {
                colorCycle[currentIndex] = new Color(.75f, .75f, .75f, 1f);
            }
        }
        currentStatus = 0;
        currentStatusTimer = 60;
    }

    void cycleColors()
    {
        if (colorCycle.Length > 1)
        {
            if (currentStatusTimer <= 0)
            {
                currentStatus++;
                if (currentStatus > colorCycle.Length - 1)
                {
                    currentStatus = 0;
                }
                RuntimeAnimatorController temp = myAnim.runtimeAnimatorController;
                myAnim.runtimeAnimatorController = null;
                spr.color = colorCycle[currentStatus];
                myAnim.runtimeAnimatorController = temp;
                currentStatusTimer = 60;
            }
            currentStatusTimer--;
        }
        /*else
        {
            RuntimeAnimatorController temp = myAnim.runtimeAnimatorController;
            myAnim.runtimeAnimatorController = null;
            spr.color = colorCycle[0];
            myAnim.runtimeAnimatorController = temp;
        }*/
    }
}
