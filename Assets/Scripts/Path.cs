using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    public List<int> path = new List<int>();
    public int hazardCount = 0;
    public bool set = false;

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
        newPath.path = new List<int>(path);
        newPath.hazardCount = hazardCount;
        newPath.set = true;
        return newPath;
    }

    public void suicide()
    {
        Destroy(this.gameObject);
    }
}
