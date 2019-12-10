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
    public Unit enemyBase;

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
            //This bit is for testing purposes.
            TextFileParser.tfp.readMissionData(0);
            playersInPosition();
            TextFileParser.tfp.loadResources();
            //As loadMap overrides heldData, it gets moved to the tail end of the function.
            loadMap(int.Parse(TextFileParser.tfp.itemList[0]));
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
        int playerCount = Controller.c.playerUnits.Length;
        int[,] coordList = TextFileParser.tfp.availablePlayerCoords(playerCount);
        for (int i = 0; i < Controller.c.playerUnits.Length; i++)
        {
            Controller.c.playerUnits[i].position[0] = coordList[i, 0];
            Controller.c.playerUnits[i].position[1] = coordList[i, 1];
            Controller.c.playerUnits[i].lastPosition[1] = coordList[i, 0];
            Controller.c.playerUnits[i].lastPosition[1] = coordList[i, 1];
            Controller.c.playerUnits[i].transform.position = new Vector3(coordList[i, 0], coordList[i, 1], -1);
        }
    }
}
