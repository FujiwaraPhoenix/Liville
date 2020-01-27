using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingRoom : MonoBehaviour
{
    public float delayTimer;
    public int frameCounter;
    public int incrementValue;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && incrementValue < 100)
        {
            incrementValue++;
            delayTimer = 0;
        }
        if (Input.GetKey(KeyCode.UpArrow) && incrementValue < 100)
        {
            if (delayTimer < 1f)
            {
                delayTimer += Time.deltaTime;
            }
            else
            {
                if (frameCounter < 10)
                {
                    frameCounter++;
                }
                else
                {
                    frameCounter = 0;
                    incrementValue++;
                }
            }
        }
    }
}
