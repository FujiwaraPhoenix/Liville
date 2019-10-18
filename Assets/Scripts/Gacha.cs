using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gacha : MonoBehaviour
{
    //In order: AR, SG, HG, RF, SWD
    public bool[] activePools = new bool[5];
    public bool[] rareChanceUpActive = new bool[5];

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void determinePools(int matA, int matB, int matC, int matD)
    {
        checkAR(matA, matB, matC, matD);
        checkHG(matA, matB, matC, matD);
        checkSG(matA, matB, matC, matD);
        checkRF(matA, matB, matC, matD);
        checkSWD(matA, matB, matC, matD);
    }

    void checkAR(int matA, int matB, int matC, int matD)
    {
        //Cull condition: no mat is > 75.
        if (matA > 75 || matB > 75 || matC > 75 || matD > 75)
        {
            activePools[0] = false;
        }
        else if (matA >= 20 && matB >= 20 && matC >= 20 && matD >= 20)
        {
            activePools[0] = true;
            if (matA >= 50 && matB >= 50 && matC >= 50 && matD >= 50)
            {
                rareChanceUpActive[0] = true;
            }
        }
        else
        {
            activePools[0] = false;
        }
    }

    void checkSG(int matA, int matB, int matC, int matD)
    {
        //Cull condition: sum < 100.
        if (matA + matB + matC + matD < 100)
        {
            activePools[1] = false;
        }
        else if (matA >= 35 && matB >= 20 && matC >= 15 && matD >= 30)
        {
            activePools[1] = true;
            if (matA >= 75 && matB >= 42 && matC >= 25 && matD >= 60)
            {
                rareChanceUpActive[1] = true;
            }
        }
        else
        {
            activePools[1] = false;
        }
    }

    void checkHG(int matA, int matB, int matC, int matD)
    {
        //Cull condition: sum > 100.
        if (matA + matB + matC + matD > 100)
        {
            activePools[2] = false;
        }
        else if (matA >= 5 && matB >= 5 && matC >= 5 && matD >= 5)
        {
            activePools[2] = true;
            if (matA >= 25 && matB >= 25 && matC >= 25 && matD >= 25)
            {
                rareChanceUpActive[2] = true;
            }
        }
        else
        {
            activePools[2] = false;
        }
    }

    void checkRF(int matA, int matB, int matC, int matD)
    {
        //Cull condition: abs(mat3-mat4) > 50
        if (Mathf.Abs(matC - matD) > 50)
        {
            activePools[3] = false;
        }
        else if (matA >= 25 && matB >= 10 && matC >= 40 && matD >= 15)
        {
            activePools[3] = true;
            if (matA >= 50 && matB >= 30 && matC >= 80 && matD >= 35)
            {
                rareChanceUpActive[3] = true;
            }
        }
        else
        {
            activePools[3] = false;
        }
    }

    void checkSWD(int matA, int matB, int matC, int matD)
    {
        //Cull condition: Mat3 > 20 || Mat4 > 20
        if (matC > 20 || matD > 20)
        {
            activePools[4] = false;
        }
        else if (matA >= 30 && matB >= 10)
        {
            activePools[4] = true;
            if (matA >= 50 && matB >= 20)
            {
                rareChanceUpActive[4] = true;
            }
        }
        else
        {
            activePools[4] = false;
        }
    }
}
