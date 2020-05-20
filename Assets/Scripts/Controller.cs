using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Controller : MonoBehaviour
{
    public static Controller c;

    //This is all for the sake of managing battles. Controller's useful for other stuff too, though.
    public Tile tilePrefab;
    //For reference: roster is ALL PLAYERS IN THE GAME. Units is active* players.
    public Unit[] playerRoster;
    public Unit[] playerUnits;
    public List<Unit> enemyUnits = new List<Unit>();
    public Map currMap;
    public int[,] tileMap, unitMap;
    public MapPointer mp;
    public bool playerTurn = true;
    public bool saidWL = false;
    public GameObject grid;
    public int currentMovingEnemy = 0;
    public bool allEnemiesMoved = false;
    public Image foreground;
    public Sprite[] helpList = new Sprite[2];
    public int currentPicture = 0;

    //Some stuff to display Player/Enemy Phase
    public int timer;
    public bool delaying = false;

    //0 is overworld menu; 1 is map select; 2 is party select; 3 is gacha; 4 is battle screen.
    public int gameMode = 0;

    //For menuing.
    public int lastMenu, currentHover;

    //For map loading
    public int chosenMission;

    //Object groups.
    public GameObject battleObjs, battleUI, overworldUI;
    public GameObject defaultMenu, loadoutUI, gachaUI, mapSelectUI;

    public bool missionSelected = false;

    //Gacha mat rewards
    public int materialAGain, materialBGain, materialCGain, materialDGain;

    //Stuffing these here because it's a giant pain to have to stash these in every unit.
    public Sprite[] damageNumbers;
    public Sprite[] healNumbers;

    //Might as well.
    public Sprite[] t1mods, t2mods, t3mods, demeritMods;
    public Sprite blankMod;

    //Background
    public Background bg;

    //Minor thing to keep consistency.
    public int dmgDelayTimer;

    //Audio stuff
    //0 - BGM
    //Everything else can be allocated as needed
    public AudioSource[] soundPlayers = new AudioSource[10];
    public AudioSource template;
    //BGM list.
    //0 is title, 1 is menus, 2 is battle
    public AudioClip[] bgm = new AudioClip[3];
    //For buttons, menus, and W/L. Also death. And phase transitions.
    public AudioClip[] sfx = new AudioClip[8];

    

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
        for (int i = 0; i < soundPlayers.Length; i++)
        {
            soundPlayers[i] = Instantiate(template, bg.transform.position, Quaternion.identity);
            soundPlayers[i].transform.parent = transform;
            soundPlayers[i].volume = .5f;
        }
        soundPlayers[0].clip = bgm[0];
        soundPlayers[0].loop = true;
        soundPlayers[0].Play();
    }

    // Update is called once per frame
    void Update()
    {
        //Run EP.
        if (gameMode == 4)
        {
            if (delaying)
            {
                if (timer > 0)
                {
                    BattleMenuUI.bmui.phaseChange.gameObject.SetActive(true);
                    float remainingTransparency = (timer / 120f);
                    BattleMenuUI.bmui.phaseChange.color = new Color(1f, 1f, 1f, remainingTransparency);
                }
                else
                {
                    BattleMenuUI.bmui.phaseChange.gameObject.SetActive(false);
                    delaying = !delaying;
                    playerTurn = !playerTurn;
                    mp.flipSpriteActive(playerTurn);
                }
                timer--;
            }
            else
            {
                if (!playerTurn)
                {
                    runEnemyTurn();
                }
                if (currMap.loaded)
                {
                    checkWLState();
                }
                BattleMenuUI.bmui.phaseChange.gameObject.SetActive(false);
            }
        }
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
                foreach (Unit u in enemyUnits)
                {
                    if (u != null && !u.isDead)
                    {
                        u.hasMoved = false;
                        u.stunned = false;
                        if (u.displayActive)
                        {
                            u.hideMovement();
                            u.displayActive = !u.displayActive;
                        }
                        u.tickDownStatus();
                        u.showStatus();
                    }
                }
                if (enemyUnits.Count != 0)
                {
                    delaying = true;
                    allEnemiesMoved = false;
                    timer = 120;
                    BattleMenuUI.bmui.phaseChange.sprite = BattleMenuUI.bmui.ePhase;
                    BattleMenuUI.bmui.phaseChange.color = new Color(1f, 1f, 1f, 1f);
                    Controller.c.playSound(Controller.c.sfx[7]);
                    Debug.Log("Player Turn: Over. Enemy Phase Begins.");
                }
            }
        }
        else
        {
            /*bool allUnitsMoved = true;
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
            }*/
            if (allEnemiesMoved)
            {
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
                                u.showHealing(1);
                                Debug.Log(u.name + " healed for 1 HP!");
                            }
                        }
                        u.tickDownStatus();
                        u.showStatus();
                    }
                }
                if (enemyUnits.Count != 0)
                {
                    delaying = true;
                    timer = 120;
                    BattleMenuUI.bmui.phaseChange.sprite = BattleMenuUI.bmui.pPhase;
                    BattleMenuUI.bmui.phaseChange.color = new Color(1f, 1f, 1f, 1f);
                    Controller.c.playSound(Controller.c.sfx[6]);
                    BattleMenuUI.bmui.currentEnemy = null;
                    BattleMenuUI.bmui.foundEnemy = false;
                    BattleMenuUI.bmui.updateEnemyDisplay();

                    Debug.Log("Enemy Turn: Over. Player Phase Begins.");
                }
            }
        }
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
            //Debug.Log("Victory!");
            BattleMenuUI.bmui.winLoss.sprite = BattleMenuUI.bmui.victory;
            BattleMenuUI.bmui.winLoss.gameObject.SetActive(true);
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
            soundPlayers[0].clip = sfx[8];
            soundPlayers[0].loop = false;
            soundPlayers[0].Play();
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
            BattleMenuUI.bmui.winLoss.sprite = BattleMenuUI.bmui.defeat;
            BattleMenuUI.bmui.winLoss.gameObject.SetActive(true);
            //Debug.Log("Defeat!");
            soundPlayers[0].clip = sfx[9];
            soundPlayers[0].loop = false;
            soundPlayers[0].Play();
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
        if (!allEnemiesMoved)
        {
            if (dmgDelayTimer <= 0)
            {
                if (currentMovingEnemy < enemyUnits.Count)
                {
                    Unit temp = enemyUnits[currentMovingEnemy];
                    if (!(temp.procPath) && !(temp.hasMoved))
                    {
                        temp.huntPlayers();
                    }
                    else if (temp.hasMoved)
                    {
                        currentMovingEnemy++;
                    }
                }
                else
                {
                    allEnemiesMoved = true;
                    currentMovingEnemy = 0;
                    checkTurn();
                }
            }
            dmgDelayTimer--;
        }
        else
        {
            checkTurn();
        }
    }

    public void switchGameState()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            switch (gameMode)
            {
                case -1:
                case -2:
                case -3:
                    if (currentPicture < 2)
                    {
                        foreground.sprite = helpList[currentPicture];
                        currentPicture++;
                    }
                    gameMode++;
                    if (gameMode == 0)
                    {
                        foreground.gameObject.SetActive(false);
                        soundPlayers[0].clip = bgm[1];
                        soundPlayers[0].Play();
                    }
                    break;
                case 0:
                    //Here, we can cycle to either the battle setup, loadout, or gacha. Let's work accordingly.
                    defaultMenu.gameObject.SetActive(false);
                    switch (currentHover)
                    {
                        //Here, 0 is battle; 1 is loadout; 2 is gacha.
                        case 0:
                            mapSelectUI.gameObject.SetActive(true);
                            break;
                        case 1:
                            loadoutUI.gameObject.SetActive(true);
                            LoadoutUI.lUI.currentLoadoutMenu = 0;
                            LoadoutUI.lUI.loadoutLimitSetup();
                            LoadoutUI.lUI.currentX = 0;
                            LoadoutUI.lUI.currentY = 0;
                            LoadoutUI.lUI.updateBaseLoadoutSpr();
                            gameMode = 2;
                            break;
                        case 2:
                            gachaUI.gameObject.SetActive(true);
                            GachaUI.gaUI.currentModdedValue = 0;
                            GachaUI.gaUI.matAVal = 0;
                            GachaUI.gaUI.matBVal = 0;
                            GachaUI.gaUI.matCVal = 0;
                            GachaUI.gaUI.matDVal = 0;
                            GachaUI.gaUI.gachaMachine.basicGunData = null;
                            GachaUI.gaUI.itemOutput.text = "";
                            break;
                    }
                    playSound(sfx[1]);
                    gameMode = currentHover + 1;
                    lastMenu = 0;
                    break;
                case 1:
                    //We should be in the battle select screen.
                    //Determine which map we're selecting with currentHover, then proceed to loadout.
                    //Insert map select function here.
                    chosenMission = MapSelect.ms.availableMissions[MapSelect.ms.currentChoice].missionNo;
                    mapSelectUI.gameObject.SetActive(false);
                    loadoutUI.gameObject.SetActive(true);
                    LoadoutUI.lUI.currentLoadoutMenu = 0;
                    LoadoutUI.lUI.currentX = 0;
                    LoadoutUI.lUI.currentY = 0;
                    missionSelected = true;
                    LoadoutUI.lUI.loadoutLimitSetup();
                    LoadoutUI.lUI.updateBaseLoadoutSpr();
                    lastMenu = 1;
                    gameMode = 2;
                    playSound(sfx[1]);
                    break;
                case 2:
                    //Loadout. If the last menu was 1 (goToBattle) and the loadout UI's on 0 AND it's on the button that only appears when you came from the battle select menu,
                    //proceed to 4.
                    if (missionSelected)
                    {
                        if (LoadoutUI.lUI.currentY == -1 && (LoadoutUI.lUI.activePlayerCount() <= chosenMission + 2) && (LoadoutUI.lUI.activePlayerCount() > 0))
                        {
                            playerUnits = LoadoutUI.lUI.currentPList();
                            loadoutUI.gameObject.SetActive(false);
                            battleObjs.gameObject.SetActive(true);
                            battleUI.gameObject.SetActive(true);
                            Controller.c.bg.changeBG(chosenMission + 1);
                            playerTurn = false;
                            timer = 120;
                            delaying = true;
                            BattleMenuUI.bmui.phaseChange.sprite = BattleMenuUI.bmui.pPhase;
                            BattleMenuUI.bmui.phaseChange.color = new Color(1f, 1f, 1f, 1f);
                            playSound(sfx[3]);
                            soundPlayers[0].clip = bgm[2];
                            soundPlayers[0].Play();
                            gameMode = 4;
                        }
                    }
                    break;
                case 3:
                    //Gacha. It does nothing. Unless we implement moving to loadout from gacha, of course, but for now?
                    break;
                case 4:
                    //We only cycle out of battle if we retreat, lose, or win, as dictated by saidWL.
                    if (saidWL)
                    {
                        //return to 0. Turn off all battle objects, turn on all base menu objects.
                        battleObjs.gameObject.SetActive(false);
                        battleUI.gameObject.SetActive(false);
                        overworldUI.gameObject.SetActive(true);
                        defaultMenu.gameObject.SetActive(true);
                        gameMode = 0;
                        foreach (Unit u in playerUnits)
                        {
                            u.resetChar();
                        }
                        saidWL = false;
                        BattleMenuUI.bmui.foundPlayer = false;
                        BattleMenuUI.bmui.currentPlayer = null;
                        BattleMenuUI.bmui.foundEnemy = false;
                        BattleMenuUI.bmui.currentEnemy = null;
                        BattleMenuUI.bmui.winLoss.gameObject.SetActive(false);
                        missionSelected = false;
                        currMap.loaded = false;
                        if (enemyUnits.Count != 0)
                        {
                            foreach (Unit u in enemyUnits)
                            {
                                Destroy(u.gameObject);
                            }
                        }
                        bg.changeBG(0);
                        soundPlayers[0].loop = true;
                        soundPlayers[0].clip = bgm[1];
                        soundPlayers[0].Play();
                    }
                    break;
            }
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            switch (gameMode)
            {
                case 0:
                    //Nothing happens. Unless we prompt to go back to title, which... Yeah no. Not right now.
                    break;

                case 1:
                    //We should be in the battle select screen.
                    //The only way to get here is from 0, so we simply turn off the battle stuff and turn the main menu back on.
                    //Insert stuff turning things on/off.
                    defaultMenu.gameObject.SetActive(true);
                    mapSelectUI.gameObject.SetActive(false);
                    lastMenu = 0;
                    MainMenu.mm.highlighted = 0;
                    playSound(sfx[2]);
                    gameMode = 0;
                    break;
                case 2:
                    //Loadout.
                    if (LoadoutUI.lUI.currentLoadoutMenu == 0)
                    {
                        loadoutUI.gameObject.SetActive(false);
                        switch (lastMenu)
                        {
                            case 0:
                                //We came here from the main menu, so go back to the main menu.
                                defaultMenu.gameObject.SetActive(true);
                                gameMode = 0;
                                break;
                            case 1:
                                //We came here from the battle select screen, so go back there.
                                mapSelectUI.gameObject.SetActive(true);
                                missionSelected = false;
                                gameMode = 1;
                                break;
                        }
                        playSound(sfx[2]);
                        lastMenu = 0;
                    }
                    break;
                case 3:
                    //Gacha. Going back here means the same as going back from battle select, so.
                    //Insert stuff turning things on/off.
                    if (!(GachaUI.gaUI.modifyingValue))
                    {
                        defaultMenu.gameObject.SetActive(true);
                        gachaUI.gameObject.SetActive(false);
                        lastMenu = 0;
                        gameMode = 0;
                    }
                    playSound(sfx[2]);
                    break;
                case 4:
                    //In battle. Doesn't work.
                    break;
            }
        }
    }

    public string determineModName(int modTier, int modID)
    {
        switch (modTier)
        {
            //Negative mods here.
            case -1:
                switch (modID)
                {
                    case 1:
                        return ("Feeble");
                    case 2:
                        return ("Flatfoot");
                    case 3:
                        return ("Paperclad");
                    case 4:
                        return ("Unaware");
                    case 5:
                        return ("Unlucky");
                    case 6:
                        return ("Uncalibrated");
                    case 7:
                        return ("Ammo Capacity-");
                    case 8:
                        return ("Ammo Capacity--");
                    default:
                        return ("--");
                }
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
                    case 9:
                        return ("Electric");
                    case 10:
                        return ("Burn");
                    case 11:
                        return ("Ice");
                    case 12:
                        return ("Mark");
                    case 13:
                        return ("Poison");
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
                        return ("Last Laugh");
                    case 3:
                        return ("Backloaded");
                    case 4:
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

    public Sprite determineModIcon(int modTier, int modID)
    {
        switch (modTier)
        {
            //From here, Demerits
            case -1:
                switch (modID)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                        return demeritMods[modID];
                    default:
                        return blankMod;
                }
            //From here, T1
            case 1:
                switch (modID)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                        return t1mods[modID];
                    default:
                        return blankMod;
                }
            //From here, T2
            case 2:
                switch (modID)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                        return t2mods[modID];
                    default:
                        return blankMod;
                }
            //From here, T3
            case 3:
                switch (modID)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                        return t3mods[modID];
                    default:
                        return blankMod;
                }
            //From here, T4
            case 4:
                switch (modID)
                {
                    default:
                        return blankMod;
                }
            //Default case.
            default:
                return blankMod;
        }
    }

    public void playSound(AudioClip ac)
    {
        bool foundOpen = false;
        for (int i = 1; i < soundPlayers.Length; i++)
        {
            if (!foundOpen)
            {
                if (!(soundPlayers[i].isPlaying))
                {
                    soundPlayers[i].clip = ac;
                    soundPlayers[i].Play();
                    foundOpen = true;
                }
            }
        }
    }
    
}

