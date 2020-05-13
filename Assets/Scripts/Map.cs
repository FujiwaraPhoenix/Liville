using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Map : MonoBehaviour
{
    public Tile[,] grid;
    public int xBound, yBound;
    public int[,] tileTypes;
    public bool loaded = false;
    public bool mapExists = false;
    public Unit[] enemyVariants;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Controller.c.currMap == null)
        {
            Controller.c.currMap = this;
        }
        if (Controller.c.gameMode == 4 && !loaded && Controller.c.missionSelected)
        {
            if (!mapExists)
            {
                generateMap();
                mapExists = true;
            }
            else
            {
                wipeGrid();
                generateMap();
            }
            TextFileParser.tfp.readMissionData(Controller.c.chosenMission);
            playersInPosition();
            TextFileParser.tfp.loadResources();
            spawnEnemies();
            //As loadMap overrides heldData, it gets moved to the tail end of the function.
            string[] tempA = TextFileParser.tfp.itemList[0].Split(new string[] { "," }, System.StringSplitOptions.None);
            loadMap(int.Parse(tempA[0]));
            loaded = true;
        }
    }

    public void loadMap(int mapNo)
    {
        tileTypes = TextFileParser.tfp.readMap(mapNo);
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                grid[(9 - j), (9 - i)].tileType = tileTypes[i, (9 - j)];
            }
        }
    }
    
    public void playersInPosition()
    {
        //Checks the controller's playerUnits array's size.
        int playerCount = TextFileParser.tfp.playerCap();
        int[,] coordList = TextFileParser.tfp.availablePlayerCoords(playerCount);
        for (int i = 0; i < Controller.c.playerUnits.Length; i++)
        {
            if (Controller.c.playerUnits.Length <= coordList.GetLength(0))
            {
                Controller.c.playerUnits[i].position[0] = coordList[i, 0];
                Controller.c.playerUnits[i].position[1] = coordList[i, 1];
                Controller.c.playerUnits[i].lastPosition[1] = coordList[i, 0];
                Controller.c.playerUnits[i].lastPosition[1] = coordList[i, 1];
                Controller.c.playerUnits[i].transform.position = new Vector3(coordList[i, 0], coordList[i, 1], -1);
                Controller.c.playerUnits[i].showStatus();
            }
        }
        Controller.c.mp.currX = coordList[0, 0];
        Controller.c.mp.currY = coordList[0, 1];
        Controller.c.mp.transform.position = new Vector3(Controller.c.mp.currX, Controller.c.mp.currY + 0.5f, -3);

    }

    public void spawnEnemies()
    {
        //Clear the enemyList
        foreach (Unit u in Controller.c.enemyUnits)
        {
            if (u != null)
            {
                Destroy(u.gameObject);
            }
        }
        //Make a new one.
        Controller.c.enemyUnits = new List<Unit>();
        //And add every enemy, in order.
        for (int i = 4; i < TextFileParser.tfp.itemList.Length; i++)
        {
            int[] unitValues = TextFileParser.tfp.numbersToUnitValues(TextFileParser.tfp.itemList[i]);
            //Make a new enemy of whatever unitValues[0] refers to.
            Unit newEnemy = Instantiate(enemyVariants[unitValues[0]], new Vector3(0, 0, 0), Quaternion.identity);
            //First, location.
            newEnemy.transform.position = new Vector3(unitValues[1], unitValues[2], -1);
            newEnemy.position[0] = unitValues[1];
            newEnemy.position[1] = unitValues[2];
            //Next, stats.
            //Values 3 to 8.
            newEnemy.setUnitStats(unitValues[3], unitValues[4], unitValues[5], unitValues[6], unitValues[7], unitValues[8]);
            //Now, weapon stats. 9 to 21.
            newEnemy.setUnitWeaponStats(unitValues[9], unitValues[10], unitValues[11], unitValues[12], unitValues[13], unitValues[14], unitValues[15], unitValues[16], unitValues[17], unitValues[18], unitValues[19], unitValues[20], unitValues[21]);
            newEnemy.showStatus();
            Controller.c.enemyUnits.Add(newEnemy);
        }
    }

    public void wipeGrid()
    {
        for (int i = 0; i < xBound; i++)
        {
            for (int j = 0; j < yBound; j++)
            {
                Destroy(grid[i, j].gameObject);
            }
        }
    }

    public void generateMap()
    {
        grid = new Tile[xBound, yBound];
        for (int i = 0; i < xBound; i++)
        {
            for (int j = 0; j < yBound; j++)
            {
                grid[i, j] = Instantiate(Controller.c.tilePrefab, new Vector3(i, j, 0f), Quaternion.identity);
                grid[i, j].transform.parent = this.transform;
            }
        }
        Controller.c.tileMap = new int[xBound, yBound];
        Controller.c.unitMap = new int[xBound, yBound];
    }

}
