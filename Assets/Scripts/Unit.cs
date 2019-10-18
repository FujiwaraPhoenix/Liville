using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public int hp, maxhp, spd, eva, def, lck, mvt, unitAllegiance;
    public int[] position;
    public int[] lastPosition;
    public bool isDead, hasMoved, currUnit;
    public List<Item> inventory = new List<Item>();
    //Limit inv to 5.
    public Item currEquip;
    public Path p;
    public Path[,] pathMap;
    public int[,] attackRangeMap;
    public Unit target;
    public Unit[] possibleTargets = new Unit[15];
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

       if (Input.GetKeyDown(KeyCode.V))
        {
            die();
        }
    }

    public void startFinding()
    {
        //Test function
        //if (currUnit)Input.GetKeyDown(KeyCode.C) && 

        pathMap = new Path[Controller.c.currMap.xBound, Controller.c.currMap.yBound];
        attackRangeMap = new int[Controller.c.currMap.xBound, Controller.c.currMap.yBound];
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

    public void clearPaths()
    {
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
        if (hp <= 0)
        {
            hp = 0;
            isDead = true;
            Controller.c.unitMap[position[0], position[1]] = 0;
            hasMoved = true;
            spr.enabled = false;
            this.gameObject.SetActive(false);
        }
    }

    public void findTargets()
    {
        //Clean the list.
        possibleTargets = new Unit[15];

        if (unitAllegiance == 1)
        {
            //This is a player unit; search enemies.
            foreach (Unit u in Controller.c.enemyUnits)
            {
                if (u != null)
                {
                    float distToTarget = Vector2.Distance(transform.position, u.transform.position);
                    if (distToTarget <= currEquip.range)
                    {
                        bool foundSpace = false;
                        for (int i = 0; i < possibleTargets.Length; i++)
                        {
                            if (possibleTargets[i] == null && !foundSpace)
                            {
                                foundSpace = true;
                                possibleTargets[i] = u;
                            }
                        }
                    }
                }
            }
        }
        if (unitAllegiance == 2)
        {
            //This is an enemy unit; search players.
            foreach (Unit u in Controller.c.playerUnits)
            {
                if (u != null)
                {
                    float distToTarget = Vector2.Distance(transform.position, u.transform.position);
                    if (distToTarget <= currEquip.range)
                    {
                        bool foundSpace = false;
                        for (int i = 0; i < possibleTargets.Length; i++)
                        {
                            if (possibleTargets[i] == null && !foundSpace)
                            {
                                foundSpace = true;
                                possibleTargets[i] = u;
                            }
                        }
                    }
                }
            }
        }
    }

    public void attack()
    {
        //So, we have a target, we have a gun. Use them.
        int hitChance = currEquip.accuracy - target.eva;
        int randChance = Random.Range(0, 100);
        if (randChance < hitChance)
        {
            target.hp -= (currEquip.dmg - target.def);
        }
        currEquip.currentClip--;
        target.die();
    }

    
    public void addItemToInv(int convoyIndex)
    {
        Item tempItem = InvManager.im.convoy[convoyIndex];
        

    }

    public void swapGun(int armoryIndex)
    {
        //Get the gun at location and save it.
        Item tempGun = InvManager.im.armory[armoryIndex];
        InvManager.im.armory.RemoveAt(armoryIndex);
        InvManager.im.armory.Add(currEquip);
        currEquip = tempGun;
    }
}
