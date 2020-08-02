using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stop : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "ground")
        {
            transform.parent.gameObject.SendMessage("isMoveStop", true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "ground")
        {
            transform.parent.gameObject.SendMessage("isMoveStop", false);
        }
    }
}
