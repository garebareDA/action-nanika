using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class attackColider : MonoBehaviour
{
    GameObject player;
    Vector3 distance;
    GameObject destroy;
    float target_distance = 1000;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void attackDestoroy()
    {
        Destroy(destroy);
        target_distance = 1000;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "enemy")
        {
            player = transform.parent.gameObject;
            float dis = Vector3.Distance(player.transform.position, collision.transform.position);
            if (target_distance > dis)
            {
                target_distance = dis;
                distance = collision.transform.position;
                destroy = collision.gameObject;
            }

            player.SendMessage("attack", distance);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        target_distance = 1000;
        distance = Vector3.zero;
    }
}
