using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvManager : MonoBehaviour
{
    public static InvManager im;
    public List<Item> convoy = new List<Item>();
    public List<Item> armory = new List<Item>();

    //For battle usage; show inventory details.
    public GameObject menuItems;
    public Text currInvShown;
    public Image[] invIcons = new Image[5];
    public Sprite[] potionSprites = new Sprite[2];

    //Gacha mats
    public int materialA, materialB, materialC, materialD;

    void Awake()
    {
        if (im == null)
        {
            DontDestroyOnLoad(gameObject);
            im = this;
        }
        else if (im != this)
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
        updateMenuPosition();
    }

    public void addItemToConvoy(Item newItem)
    {
        convoy.Add(newItem);
    }

    public void addGunToArmory(Item newGun)
    {
        armory.Add(newGun);
    }

    public void updateMenuPosition()
    {
        if (!Controller.c.mp.selectingItem)
        {
            menuItems.gameObject.SetActive(false);
        }
        else
        {
            menuItems.gameObject.SetActive(true);
            for (int i = 0; i < 5; i++)
            {
                if (i < BattleMenuUI.bmui.currentPlayer.inventory.Count)
                {
                    invIcons[i].gameObject.SetActive(true);
                    if (Controller.c.mp.currentInvChoice == i)
                    {
                        invIcons[i].sprite = potionSprites[1];
                    }
                    else
                    {
                        invIcons[i].sprite = potionSprites[0];
                    }
                }
                else
                {
                    invIcons[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
