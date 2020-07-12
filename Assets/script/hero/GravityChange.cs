using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityChange : MonoBehaviour
{
    public int gravityChange;
    public int exitChange;
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            rotate(gravityChange);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            rotate(exitChange);
        }
    }

    private void rotate(int GravityChange)
    {
        if (GravityChange == 0)
        {
            //left
            player.SendMessage("changeGravityMode", "left");
        }
        else if (GravityChange == 1)
        {
            //rihgt
            player.SendMessage("changeGravityMode", "right");
        }
        else if (GravityChange == 2)
        {
            //up
            player.SendMessage("changeGravityMode", "up");
        }
        else
        {
            //down
            player.SendMessage("changeGravityMode", "down");
        }
    }
}
