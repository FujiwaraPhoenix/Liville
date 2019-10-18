using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvManager : MonoBehaviour
{
    public static InvManager im;
    public List<Item> convoy = new List<Item>();
    public List<Item> armory = new List<Item>();

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
        
    }

    public void addItemToConvoy(Item newItem)
    {
        convoy.Add(newItem);
    }

    public void addGunToArmory()
    {

    }

    
}
