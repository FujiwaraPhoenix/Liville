﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    public List<int> path = new List<int>();
    public int hazardCount = 0;
    public bool set = false;
    public int whoseSide = 0;
    public bool setAtk = false;
    public bool tempImmune = false;
    public bool currentTile = false;

    public void fillPath(int g)
    {
       for (int i = 0; i < 50; i++)
        {
            path.Add(g);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if ((Controller.c.playerTurn && whoseSide == 2) || (!(Controller.c.playerTurn) && whoseSide == 1) || (Controller.c.gameMode != 4) || !set || !tempImmune || whoseSide == 0)
        {
            suicide();
        }
    }

    public string toString()
    {
        string s = "[";
        foreach (int i in path)
        {
            s += i + ", ";
        }
        s += "]";
        return s;
    }
    public Path copyPath()
    {
        Path newPath = Instantiate(this, transform.position, Quaternion.identity);
        newPath.whoseSide = whoseSide;
        newPath.path = new List<int>(path);
        newPath.hazardCount = hazardCount;
        newPath.set = set;
        newPath.setAtk = setAtk;
        return newPath;
    }

    public void suicide()
    {
        Destroy(this.gameObject);
    }
}
