using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamControl : MonoBehaviour
{
    //Is camera mvt enabled?
    public bool camMvtActive = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Controller.c.gameMode == 0)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                camMvtActive = !camMvtActive;
            }
            if (camMvtActive)
            {
                bool tryUp = Input.GetKey(KeyCode.W);
                bool tryDown = Input.GetKey(KeyCode.S);
                bool tryLeft = Input.GetKey(KeyCode.A);
                bool tryRight = Input.GetKey(KeyCode.D);

                Vector2 mvtDir = Vector2.zero;

                if (tryUp)
                {
                    mvtDir += Vector2.up;
                }
                if (tryDown)
                {
                    mvtDir += Vector2.down;
                }
                if (tryLeft)
                {
                    mvtDir += Vector2.left;
                }
                if (tryRight)
                {
                    mvtDir += Vector2.right;
                }
                this.gameObject.transform.position += new Vector3(mvtDir.x, mvtDir.y, 0f);
            }
        }
        /*if (Controller.c.gameMode == 1)
        {
            if (Controller.c.mp != null)
            {
                transform.position = new Vector3(Controller.c.mp.transform.position.x, Controller.c.mp.transform.position.y, -10);
            }
        }*/
    }
}
