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
    public void drawPath(Unit selectedUnit, int[] startLocation, int steps, Path currentPath, int unitAllegiance)
    {
        if (steps == 0)
        {
            //Check attack range for outlier targets.
        }
        else
        {
            //Check up
            //Out of bounds?
            if (!(startLocation[1] + 1 >= Controller.c.currMap.yBound))
            {
                if (Controller.c.currMap.grid[startLocation[0], startLocation[1] + 1].isPassable)
                {
                    //A unit doesn't exist on this tile.
                    if (Controller.c.unitMap[startLocation[0], startLocation[1] + 1] == 0)
                    {
                        
                        //If I can move here:
                        if (steps - Controller.c.currMap.grid[startLocation[0], startLocation[1] + 1].mvtPenalty >= 0)
                        {
                            currentPath.path.Add(0);
                            if (Controller.c.currMap.grid[startLocation[0], startLocation[1] + 1].isHazardous)
                            {
                                currentPath.hazardCount++;
                            }
                            //Check the path map
                            if (selectedUnit.pathMap[startLocation[0], startLocation[1] + 1] == null)
                            {
                                selectedUnit.pathMap[startLocation[0], startLocation[1] + 1] = currentPath.copyPath();
                            }
                            else
                            {
                                //Is the old path less hazardous?
                                if (selectedUnit.pathMap[startLocation[0], startLocation[1] + 1].hazardCount > currentPath.hazardCount)
                                {
                                    selectedUnit.pathMap[startLocation[0], startLocation[1] + 1] = currentPath.copyPath();
                                }
                                //Is it longer?
                                else if (selectedUnit.pathMap[startLocation[0], startLocation[1] + 1].hazardCount == currentPath.hazardCount)
                                {
                                    if (selectedUnit.pathMap[startLocation[0], startLocation[1] + 1].path.Count > currentPath.path.Count)
                                    {
                                        selectedUnit.pathMap[startLocation[0], startLocation[1] + 1] = currentPath.copyPath();
                                    }
                                }
                            }
                            int[] newStart = new int[2];
                            newStart[0] = startLocation[0];
                            newStart[1] = startLocation[1] + 1;

                            drawPath(selectedUnit, newStart, steps - Controller.c.currMap.grid[newStart[0], newStart[1]].mvtPenalty, currentPath, unitAllegiance);
                            currentPath.path.RemoveAt(currentPath.path.Count-1);
                        }
                        //If I can't:
                        else
                        {
                            //Nothing.
                        }
                    }
                    //'Ally' unit
                    if (Controller.c.unitMap[startLocation[0], startLocation[1] + 1] == unitAllegiance)
                    {
                        //If I can move here:
                        if (steps - Controller.c.currMap.grid[startLocation[0], startLocation[1] + 1].mvtPenalty >= 1)
                        {
                            currentPath.path.Add(0);
                            if (Controller.c.currMap.grid[startLocation[0], startLocation[1] + 1].isHazardous)
                            {
                                currentPath.hazardCount++;
                            }
                            int[] newStart = new int[2];
                            newStart[0] = startLocation[0];
                            newStart[1] = startLocation[1] + 1;
                            drawPath(selectedUnit, newStart, steps - Controller.c.currMap.grid[newStart[0], newStart[1]].mvtPenalty, currentPath, unitAllegiance);
                            currentPath.path.RemoveAt(currentPath.path.Count - 1);
                        }
                    }
                    //'Enemy' unit
                    else
                    {
                        //Nope
                    }
                }
            }

            //Check right
            //Out of bounds?
            if (!(startLocation[0] + 1 >= Controller.c.currMap.xBound))
            {
                if (Controller.c.currMap.grid[startLocation[0] + 1, startLocation[1]].isPassable)
                {
                    //A unit doesn't exist on this tile.
                    if (Controller.c.unitMap[startLocation[0] + 1, startLocation[1]] == 0)
                    {
                        //If I can move here:
                        if (steps - Controller.c.currMap.grid[startLocation[0] + 1, startLocation[1]].mvtPenalty >= 0)
                        {
                            currentPath.path.Add(1);
                            if (Controller.c.currMap.grid[startLocation[0] + 1, startLocation[1]].isHazardous)
                            {
                                currentPath.hazardCount++;
                            }
                            //Check the path map
                            if (selectedUnit.pathMap[startLocation[0] + 1, startLocation[1]] == null)
                            {
                                selectedUnit.pathMap[startLocation[0] + 1, startLocation[1]] = currentPath.copyPath();
                            }
                            else
                            {
                                //Is the old path less hazardous?
                                if (selectedUnit.pathMap[startLocation[0] + 1, startLocation[1]].hazardCount > currentPath.hazardCount)
                                {
                                    selectedUnit.pathMap[startLocation[0] + 1, startLocation[1]] = currentPath.copyPath();
                                }
                                //Is it longer?
                                else if (selectedUnit.pathMap[startLocation[0] + 1, startLocation[1]].hazardCount == currentPath.hazardCount)
                                {
                                    if (selectedUnit.pathMap[startLocation[0] + 1, startLocation[1]].path.Count > currentPath.path.Count)
                                    {
                                        selectedUnit.pathMap[startLocation[0] + 1, startLocation[1]] = currentPath.copyPath();
                                    }
                                }
                            }
                            int[] newStart = new int[2];
                            newStart[0] = startLocation[0] + 1;
                            newStart[1] = startLocation[1];
                            drawPath(selectedUnit, newStart, steps - Controller.c.currMap.grid[newStart[0], newStart[1]].mvtPenalty, currentPath, unitAllegiance);
                            currentPath.path.RemoveAt(currentPath.path.Count - 1);
                        }
                        //If I can't:
                        else
                        {
                            //Nothing.
                        }
                    }
                    //'Ally' unit
                    if (Controller.c.unitMap[startLocation[0] + 1, startLocation[1]] == unitAllegiance)
                    {
                        //If I can move here:
                        if (steps - Controller.c.currMap.grid[startLocation[0] + 1, startLocation[1]].mvtPenalty >= 0)
                        {
                            currentPath.path.Add(0);
                            if (Controller.c.currMap.grid[startLocation[0] + 1, startLocation[1]].isHazardous)
                            {
                                currentPath.hazardCount++;
                            }
                            int[] newStart = new int[2];
                            newStart[0] = startLocation[0] + 1;
                            newStart[1] = startLocation[1];
                            drawPath(selectedUnit, newStart, steps - Controller.c.currMap.grid[newStart[0], newStart[1]].mvtPenalty, currentPath, unitAllegiance);
                            currentPath.path.RemoveAt(currentPath.path.Count - 1);
                        }
                    }
                    //'Enemy' unit
                    else
                    {
                        //Nope
                    }
                }
            }

            //Check down
            //Out of bounds?
            if (!(startLocation[1] - 1 < 0))
            {
                if (Controller.c.currMap.grid[startLocation[0], startLocation[1] - 1].isPassable)
                {
                    //A unit doesn't exist on this tile.
                    if (Controller.c.unitMap[startLocation[0], startLocation[1] - 1] == 0)
                    {
                        //If I can move here:
                        if (steps - Controller.c.currMap.grid[startLocation[0], startLocation[1] - 1].mvtPenalty >= 0)
                        {
                            currentPath.path.Add(0);
                            if (Controller.c.currMap.grid[startLocation[0], startLocation[1] - 1].isHazardous)
                            {
                                currentPath.hazardCount++;
                            }
                            //Check the path map
                            if (selectedUnit.pathMap[startLocation[0], startLocation[1] - 1] == null)
                            {
                                selectedUnit.pathMap[startLocation[0], startLocation[1] - 1] = currentPath.copyPath();
                            }
                            else
                            {
                                //Is the old path less hazardous?
                                if (selectedUnit.pathMap[startLocation[0], startLocation[1] - 1].hazardCount > currentPath.hazardCount)
                                {
                                    selectedUnit.pathMap[startLocation[0], startLocation[1] - 1] = currentPath.copyPath();
                                }
                                //Is it longer?
                                else if (selectedUnit.pathMap[startLocation[0], startLocation[1] - 1].hazardCount == currentPath.hazardCount)
                                {
                                    if (selectedUnit.pathMap[startLocation[0], startLocation[1] - 1].path.Count > currentPath.path.Count)
                                    {
                                        selectedUnit.pathMap[startLocation[0], startLocation[1] - 1] = currentPath.copyPath();
                                    }
                                }
                            }
                            int[] newStart = new int[2];
                            newStart[0] = startLocation[0];
                            newStart[1] = startLocation[1] - 1;
                            drawPath(selectedUnit, newStart, steps - Controller.c.currMap.grid[newStart[0], newStart[1]].mvtPenalty, currentPath, unitAllegiance);
                            currentPath.path.RemoveAt(currentPath.path.Count - 1);
                        }
                        //If I can't:
                        else
                        {
                            //Nothing.
                        }
                    }
                    //'Ally' unit
                    if (Controller.c.unitMap[startLocation[0], startLocation[1] - 1] == unitAllegiance)
                    {
                        //If I can move here:
                        if (steps - Controller.c.currMap.grid[startLocation[0], startLocation[1] - 1].mvtPenalty >= 0)
                        {
                            currentPath.path.Add(0);
                            if (Controller.c.currMap.grid[startLocation[0], startLocation[1] - 1].isHazardous)
                            {
                                currentPath.hazardCount++;
                            }
                            int[] newStart = new int[2];
                            newStart[0] = startLocation[0];
                            newStart[1] = startLocation[1] - 1;
                            drawPath(selectedUnit, newStart, steps - Controller.c.currMap.grid[newStart[0], newStart[1]].mvtPenalty, currentPath, unitAllegiance);
                            currentPath.path.RemoveAt(currentPath.path.Count - 1);
                        }
                    }
                    //'Enemy' unit
                    else
                    {
                        //Nope
                    }
                }
            }

            //Check left
            //Out of bounds?
            if (!(startLocation[0] - 1 < 0))
            {
                if (Controller.c.currMap.grid[startLocation[0] - 1, startLocation[1]].isPassable)
                {
                    //A unit doesn't exist on this tile.
                    if (Controller.c.unitMap[startLocation[0] - 1, startLocation[1]] == 0)
                    {
                        //If I can move here:
                        if (steps - Controller.c.currMap.grid[startLocation[0] - 1, startLocation[1]].mvtPenalty >= 0)
                        {
                            currentPath.path.Add(1);
                            if (Controller.c.currMap.grid[startLocation[0] - 1, startLocation[1]].isHazardous)
                            {
                                currentPath.hazardCount++;
                            }
                            //Check the path map
                            if (selectedUnit.pathMap[startLocation[0] - 1, startLocation[1]] == null)
                            {
                                selectedUnit.pathMap[startLocation[0] - 1, startLocation[1]] = currentPath.copyPath();
                            }
                            else
                            {
                                //Is the old path less hazardous?
                                if (selectedUnit.pathMap[startLocation[0] - 1, startLocation[1]].hazardCount > currentPath.hazardCount)
                                {
                                    selectedUnit.pathMap[startLocation[0] - 1, startLocation[1]] = currentPath.copyPath();
                                }
                                //Is it longer?
                                else if (selectedUnit.pathMap[startLocation[0] - 1, startLocation[1]].hazardCount == currentPath.hazardCount)
                                {
                                    if (selectedUnit.pathMap[startLocation[0] - 1, startLocation[1]].path.Count > currentPath.path.Count)
                                    {
                                        selectedUnit.pathMap[startLocation[0] - 1, startLocation[1]] = currentPath.copyPath();
                                    }
                                }
                            }
                            int[] newStart = new int[2];
                            newStart[0] = startLocation[0] - 1;
                            newStart[1] = startLocation[1];
                            drawPath(selectedUnit, newStart, steps - Controller.c.currMap.grid[newStart[0], newStart[1]].mvtPenalty, currentPath, unitAllegiance);
                            currentPath.path.RemoveAt(currentPath.path.Count - 1);
                        }
                        //If I can't:
                        else
                        {
                            //Nothing.
                        }
                    }
                    //'Ally' unit
                    if (Controller.c.unitMap[startLocation[0] - 1, startLocation[1]] == unitAllegiance)
                    {
                        //If I can move here:
                        if (steps - Controller.c.currMap.grid[startLocation[0] - 1, startLocation[1]].mvtPenalty >= 0)
                        {
                            currentPath.path.Add(0);
                            if (Controller.c.currMap.grid[startLocation[0] - 1, startLocation[1]].isHazardous)
                            {
                                currentPath.hazardCount++;
                            }
                            int[] newStart = new int[2];
                            newStart[0] = startLocation[0] - 1;
                            newStart[1] = startLocation[1];
                            drawPath(selectedUnit, newStart, steps - Controller.c.currMap.grid[newStart[0], newStart[1]].mvtPenalty, currentPath, unitAllegiance);
                            currentPath.path.RemoveAt(currentPath.path.Count - 1);
                        }
                    }
                    //'Enemy' unit
                    else
                    {
                        //Nope
                    }
                }
            }
        }
    }
}
