using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static MainMenu mm;

    //Buttons.
    public Image battle, loadout, gacha;
    //Sprites for the buttons
    public Sprite bDef, bHighlight, lDef, lHighlight, gDef, gHighlight;

    //0 = battle, 1 = loadout, 2 = gacha
    public int highlighted = 0;

    void Awake()
    {
        if (mm == null)
        {
            DontDestroyOnLoad(gameObject);
            mm = this;
        }
        else if (mm != this)
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        updateButtons();
    }

    // Update is called once per frame
    void Update()
    {
        if (Controller.c.gameMode == 0)
        {
            navigate();
        }
    }

    public void navigate()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            switch (highlighted)
            {
                case 1:
                    highlighted = 2;
                    break;
                case 2:
                    highlighted = 1;
                    break;
            }
            updateButtons();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            switch (highlighted)
            {
                case 0:
                    highlighted = 1;
                    break;
                case 1:
                case 2:
                    highlighted = 0;
                    break;
            }
            updateButtons();
        }
        Controller.c.currentHover = highlighted;
    }

    public void updateButtons()
    {
        switch (highlighted)
        {
            case 0:
                battle.sprite = bHighlight;
                loadout.sprite = lDef;
                gacha.sprite = gDef;
                break;
            case 1:
                battle.sprite = bDef;
                loadout.sprite = lHighlight;
                gacha.sprite = gDef;
                break;
            case 2:
                battle.sprite = bDef;
                loadout.sprite = lDef;
                gacha.sprite = gHighlight;
                break;
        }
        if (Controller.c.gameMode == 0)
        {
            Controller.c.playSound(Controller.c.sfx[0], .25f);
        }
    }
}
