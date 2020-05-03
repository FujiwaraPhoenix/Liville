using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Pathfinder : MonoBehaviour
{
    public static Pathfinder pf;
    public List<int> currP = new List<int>();

    void Awake()
    {
        if (pf == null)
        {
            DontDestroyOnLoad(gameObject);
            pf = this;
        }
        else if (pf != this)
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

    }

    //This function, when given a starting location, determines paths to all possible locations within distance.
    //0 = up, 1 = right, 2 = down, 3 = left
    public void drawPath(Path[,] currentMap, int[] startLocation, int steps, Path currentPath, int unitAllegiance, int atkDistance)
    {
        int[] newStart = new int[2];
        if (steps + atkDistance > 0)
        {
            //Check up
            //Out of bounds?
            if (startLocation[1] + 1 < Controller.c.currMap.yBound)
            {
                newStart[0] = startLocation[0];
                newStart[1] = startLocation[1] + 1;
                setToMvtHelper(currentMap, newStart, steps, currentPath, unitAllegiance, atkDistance, 0);
            }

            //Check right
            //Out of bounds?
            if (startLocation[0] + 1 < Controller.c.currMap.xBound)
            {
                newStart[0] = startLocation[0] + 1;
                newStart[1] = startLocation[1];
                setToMvtHelper(currentMap, newStart, steps, currentPath, unitAllegiance, atkDistance, 1);
            }

            //Check down
            //Out of bounds?
            if (startLocation[1] - 1 >= 0)
            {
                newStart[0] = startLocation[0];
                newStart[1] = startLocation[1] - 1;
                setToMvtHelper(currentMap, newStart, steps, currentPath, unitAllegiance, atkDistance, 2);
            }

            //Check left
            //Out of bounds?
            if (startLocation[0] - 1 >= 0)
            {
                newStart[0] = startLocation[0] - 1;
                newStart[1] = startLocation[1];
                setToMvtHelper(currentMap, newStart, steps, currentPath, unitAllegiance, atkDistance, 3);
            }
        }
        else
        {
            return;
        }
    }

    void setToAtkHelper(Path[,] givenMap, int[] coordinates, Path givenPath, int allegiance, int atkDist)
    {
        if (givenMap[coordinates[0], coordinates[1]] == null || !(givenMap[coordinates[0], coordinates[1]].set) && atkDist != 0)
        {
            //Nothing here, which is necessary. If something WAS here, it'd be set.
            givenMap[coordinates[0], coordinates[1]] = givenPath.copyPath();
            givenMap[coordinates[0], coordinates[1]].set = false;
            givenMap[coordinates[0], coordinates[1]].setAtk = true;
        }
        drawPath(givenMap, coordinates, 0, givenMap[coordinates[0], coordinates[1]], allegiance, atkDist - 1);
    }

    void setToMvtHelper(Path[,] givenMap, int[] coordinates, int movement, Path givenPath, int allegiance, int atkDist, int direction)
    {
        if (movement > 0)
        {
            if (Controller.c.currMap.grid[coordinates[0], coordinates[1]].isPassable)
            {
                bool wasHazardous = false;
                if (Controller.c.unitMap[coordinates[0], coordinates[1]] == 0)
                {
                    //If I can move here:
                    if (movement - Controller.c.currMap.grid[coordinates[0], coordinates[1]].mvtPenalty >= 0)
                    {
                        givenPath.path.Add(direction);
                        if (Controller.c.currMap.grid[coordinates[0], coordinates[1]].isHazardous)
                        {
                            givenPath.hazardCount++;
                        }
                        //Check the path map
                        if (givenMap[coordinates[0], coordinates[1]] == null || givenMap[coordinates[0], coordinates[1]].setAtk)
                        {
                            givenMap[coordinates[0], coordinates[1]] = givenPath.copyPath();
                        }
                        else
                        {
                            //Is the old path less hazardous?
                            if (givenMap[coordinates[0], coordinates[1]].hazardCount > givenPath.hazardCount)
                            {
                                givenMap[coordinates[0], coordinates[1]] = givenPath.copyPath();
                            }
                            //Is it longer?
                            else if (givenMap[coordinates[0], coordinates[1]].hazardCount == givenPath.hazardCount)
                            {
                                if (givenMap[coordinates[0], coordinates[1]].path.Count > givenPath.path.Count)
                                {
                                    givenMap[coordinates[0], coordinates[1]] = givenPath.copyPath();
                                }
                            }
                        }
                        drawPath(givenMap, coordinates, movement - Controller.c.currMap.grid[coordinates[0], coordinates[1]].mvtPenalty, givenPath, allegiance, atkDist);
                        givenPath.path.RemoveAt(givenPath.path.Count - 1);
                        if (wasHazardous)
                        {
                            givenPath.hazardCount--;
                        }
                    }
                    //If I can't:
                    else
                    {
                        //Set steps to 0.
                        setToAtkHelper(givenMap, coordinates, givenPath, allegiance, atkDist);
                    }
                }
                //'Ally' unit
                else if (Controller.c.unitMap[coordinates[0], coordinates[1]] == allegiance)
                {
                    //If I can move here:
                    if (movement - Controller.c.currMap.grid[coordinates[0], coordinates[1]].mvtPenalty >= 1)
                    {
                        givenPath.path.Add(direction);
                        if (Controller.c.currMap.grid[coordinates[0], coordinates[1]].isHazardous)
                        {
                            givenPath.hazardCount++;
                        }
                        drawPath(givenMap, coordinates, movement - Controller.c.currMap.grid[coordinates[0], coordinates[1]].mvtPenalty, givenPath, allegiance, atkDist);
                        givenPath.path.RemoveAt(givenPath.path.Count - 1);
                        if (wasHazardous)
                        {
                            givenPath.hazardCount--;
                        }
                    }
                }
                //'Enemy' unit
                else
                {
                    //Steps at 0; attack distance not.
                    setToAtkHelper(givenMap, coordinates, givenPath, allegiance, atkDist);
                }
            }
            else
            {
                //Steps at 0; attack distance not.
                setToAtkHelper(givenMap, coordinates, givenPath, allegiance, atkDist);
            }
        }
        else if (atkDist > 0)
        {
            //Steps at 0; attack distance not.
            setToAtkHelper(givenMap, coordinates, givenPath, allegiance, atkDist);
        }
    }
}
