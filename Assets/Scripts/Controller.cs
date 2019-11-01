using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Controller : MonoBehaviour
{
    public static Controller c;
    public Tile tilePrefab;
    public Unit[] playerUnits;
    public Unit[] enemyUnits;
    public Map currMap;
    public int[,] tileMap, unitMap;
    public MapPointer mp;
    public bool playerTurn = true;
    public bool saidWL = false;
    //For testing purposes; 0 is map gen test; 1 is pre-gen map.
    public int gameMode = 0;
    
    //Gacha mat rewards
    public int materialAGain, materialBGain, materialCGain, materialDGain;

    void Awake()
    {
        if (c == null)
        {
            DontDestroyOnLoad(gameObject);
            c = this;
        }
        else if (c != this)
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            forceETurnEnd();
        }
        //Run EP.
        if (!playerTurn)
        {
            runEnemyTurn();
        }
        checkWLState();
    }
    
    public void checkTurn()
    {
        if (playerTurn)
        {
            bool allUnitsMoved = true;
            foreach (Unit u in playerUnits)
            {
                if (u != null && !u.isDead)
                {
                    if (!u.hasMoved)
                    {
                        allUnitsMoved = !allUnitsMoved;
                    }
                }
            }
            if (allUnitsMoved)
            {
                playerTurn = !playerTurn;
                foreach (Unit u in enemyUnits)
                {
                    if (u != null && !u.isDead)
                    {
                        u.hasMoved = false;
                    }
                }
                Debug.Log("Player Turn: Over. Enemy Phase Begins.");
            }
        }
        else
        {
            bool allUnitsMoved = true;
            foreach (Unit u in enemyUnits)
            {
                if (u != null)
                {
                    if (!u.hasMoved && !u.isDead)
                    {
                        allUnitsMoved = !allUnitsMoved;
                    }
                }
            }
            if (allUnitsMoved)
            {
                playerTurn = !playerTurn;
                foreach (Unit u in playerUnits)
                {
                    if (u != null && !u.isDead)
                    {
                        u.hasMoved = false;
                    }
                }
                Debug.Log("Enemy Turn: Over. Player Phase Begins.");
            }
        }
    }

    public void forceETurnEnd()
    {
        foreach (Unit u in enemyUnits)
        {
            if (u != null && !u.isDead)
            {
                if (!u.hasMoved)
                {
                    u.hasMoved = true;
                }
            }
        }
        checkTurn();
    }

    public void winMap()
    {
        //Assuming win condition is 'rout'
        bool enemiesDead = true;
        foreach (Unit u in enemyUnits)
        {
            if (!u.isDead)
            {
                enemiesDead = false;
            }
        }
        if (enemiesDead && !saidWL)
        {
            Debug.Log("Victory!");
            InvManager.im.materialA += materialAGain;
            InvManager.im.materialB += materialBGain;
            InvManager.im.materialC += materialCGain;
            InvManager.im.materialD += materialDGain;
            materialAGain = 0;
            materialBGain = 0;
            materialCGain = 0;
            materialDGain = 0;
            saidWL = true;
        }
    }
    public void loseMap()
    {
        //All players routed?
        bool playersDead = true;
        foreach (Unit u in playerUnits)
        {
            if (!u.isDead)
            {
                playersDead = false;
            }
        }
        if (playersDead && !saidWL)
        {
            Debug.Log("Defeat!");
            saidWL = true;
        }
    }

    public void checkWLState()
    {
        winMap();
        loseMap();
    }

    public void runEnemyTurn()
    {
        foreach (Unit u in enemyUnits)
        {
            u.huntPlayers();
        }
        checkTurn();
    }
}

