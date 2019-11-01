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
    public Image menuPointer;
    public GameObject menuItems;
    public Text currInvShown;

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
            switch (Controller.c.mp.currentInvChoice)
            {
                case 0:
                    menuPointer.transform.localPosition = new Vector3(-41.5f, 32, 0);
                    break;
                case 1:
                    menuPointer.transform.localPosition = new Vector3(-41.5f, 16.5f, 0);
                    break;
                case 2:
                    menuPointer.transform.localPosition = new Vector3(-41.5f, .5f, 0);
                    break;
                case 3:
                    menuPointer.transform.localPosition = new Vector3(-41.5f, -15, 0);
                    break;
                case 4:
                    menuPointer.transform.localPosition = new Vector3(-41.5f, -31.5f, 0);
                    break;

            }
        }
    }
}
