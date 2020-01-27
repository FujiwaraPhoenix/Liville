using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Controller : MonoBehaviour
{
    public static Controller c;
    public Tile tilePrefab;
    public Unit[] playerRoster;
    public Unit[] playerUnits;
    public List<Unit> enemyUnits = new List<Unit>();
    public Map currMap;
    public int[,] tileMap, unitMap;
    public MapPointer mp;
    public bool playerTurn = true;
    public bool saidWL = false;
    public GameObject grid;
    //0 is overworld menu; 1 is map select; 2 is party select; 3 is gacha; 4 is battle screen.
    public int gameMode = 0;

    //Object groups.
    public GameObject battleObjs, battleUI, overworldUI;
    public GameObject defaultMenu, loadoutUI, gachaUI;

    public bool missionSelected = false;

    //For testing purposes.
    public bool switchGameMode = false;

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
        unitMap = new int[10, 10];
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        //Run EP.
        if (gameMode == 4)
        {
            if (!playerTurn)
            {
                runEnemyTurn();
            }
            checkWLState();
        }
        //Debugging.
        switchGameState();
    }
    
    public void checkTurn()
    {
        if (playerTurn)
        {
            bool allUnitsMoved = true;
            foreach (Unit u in playerUnits)
            {
                if (u.stunned)
                {
                    u.hasMoved = true;
                }
                if (u != null && !u.isDead)
                {
                    if (!u.hasMoved)
                    {
                        allUnitsMoved = false;
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
                        u.stunned = false;
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
                if (u.stunned)
                {
                    u.hasMoved = true;
                }
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
                        u.stunned = false;
                        //Regeneration procs at the start of turn.
                        if (u.checkMod(2, 4))
                        {
                            if (u.hp < u.maxhp)
                            {
                                u.hp++;
                                Debug.Log(u.name + " healed for 1 HP!");
                            }
                        }
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
            foreach (Unit u in playerUnits)
            {
                if (u.checkMod(1, 7))
                {
                    materialAGain = (int)(materialAGain * 1.2f);
                    materialBGain = (int)(materialBGain * 1.2f);
                    materialCGain = (int)(materialCGain * 1.2f);
                    materialDGain = (int)(materialDGain * 1.2f);
                }
            }
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

    public void switchGameState()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            switchGameMode = !switchGameMode;
            Debug.Log("Game mode switch flipped!");
        }
        if (switchGameMode)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                gameMode = 0;
                switchGameMode = false;
                Debug.Log("Game mode 0!");
                foreach (Unit u in playerRoster)
                {
                    u.GetComponent<SpriteRenderer>().enabled = true;
                }
                BattleMenuUI.bmui.gameObject.SetActive(true);
                mp.gameObject.SetActive(true);
                grid.SetActive(true);
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                gameMode = 1;
                switchGameMode = false;
                Debug.Log("Game mode 1!");
                foreach (Unit u in playerRoster)
                {
                    u.GetComponent<SpriteRenderer>().enabled = false;
                }
                BattleMenuUI.bmui.gameObject.SetActive(false);
                grid.SetActive(false);
                mp.gameObject.SetActive(false);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                gameMode = 2;
                switchGameMode = false;
                Debug.Log("Game mode 2!");
                foreach (Unit u in playerRoster)
                {
                    u.GetComponent<SpriteRenderer>().enabled = false;
                }
                BattleMenuUI.bmui.gameObject.SetActive(false);
                mp.gameObject.SetActive(false);
                grid.SetActive(false);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                gameMode = 3;
                switchGameMode = false;
                Debug.Log("Game mode 3!");
                foreach (Unit u in playerRoster)
                {
                    u.GetComponent<SpriteRenderer>().enabled = false;
                }
                BattleMenuUI.bmui.gameObject.SetActive(false);
                mp.gameObject.SetActive(false);
                grid.SetActive(false);
            }
        }
    }

    public string determineModName(int modTier, int modID)
    {
        switch (modTier)
        {
            //From here, T1
            case 1:
                switch (modID)
                {
                    case 1:
                        return ("Fleetfoot");
                    case 2:
                        return ("Ironclad");
                    case 3:
                        return ("Aware");
                    case 4:
                        return ("Lucky");
                    case 5:
                        return ("Resistant");
                    case 6:
                        return ("Determined");
                    case 7:
                        return ("Scavenger");
                    case 8:
                        return ("Ammo Capacity+");
                    default:
                        return ("--");
                }
            //From here, T2
            case 2:
                switch (modID)
                {
                    case 1:
                        return ("Brutal");
                    case 2:
                        return ("Scope");
                    case 3:
                        return ("Frontloaded");
                    case 4:
                        return ("Regeneration");
                    case 5:
                        return ("Stun");
                    case 6:
                        return ("Ammo Capacity++");
                    case 7:
                        return ("Ammo Capacity-");
                    default:
                        return ("--");
                }
            //From here, T3
            case 3:
                switch (modID)
                {
                    case 1:
                        return ("Recycle");
                    case 2:
                        return ("Kamikaze");
                    case 3:
                        return ("Backloaded");
                    case 4:
                        return ("Gentle");
                    case 5:
                        return ("Flatfoot");
                    case 6:
                        return ("Paperclad");
                    case 7:
                        return ("Unaware");
                    case 8:
                        return ("Unlucky");
                    case 9:
                        return ("Uncalibrated");
                    case 10:
                        return ("Ammo Capacity--");
                    case 11:
                        return ("Ethereal");
                    default:
                        return ("--");
                }
            //From here, T4
            case 4:
                switch (modID)
                {
                    default:
                        return ("--");
                }
            //Default case.
            default:
                return ("--");
        }
    }
}

