using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Map : MonoBehaviour
{
    public Tile[,] grid;
    public int xBound, yBound;

    // Start is called before the first frame update
    void Start()
    {
        if (Controller.c.currMap == null)
        {
            Controller.c.currMap = this;
        }
        if (Controller.c.gameMode == 1)
        {
            grid = new Tile[xBound, yBound];
            for (int i = 0; i < xBound; i++)
            {
                for (int j = 0; j < yBound; j++)
                {
                    grid[i, j] = Instantiate(Controller.c.tilePrefab, new Vector3(i, j, 0f), Quaternion.identity);
                }
            }
            Controller.c.tileMap = new int[xBound, yBound];
            Controller.c.unitMap = new int[xBound, yBound];
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Initialize a grid; test
        if (Controller.c.gameMode == 0)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                grid = new Tile[10, 10];
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        grid[i, j] = Instantiate(Controller.c.tilePrefab, new Vector3(i, j, 0f), Quaternion.identity);
                    }
                }
                Controller.c.tileMap = new int[10, 10];
                Controller.c.unitMap = new int[10, 10];
                xBound = 10;
                yBound = 10;
            }
        }
    }
}
