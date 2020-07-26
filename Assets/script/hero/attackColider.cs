using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class attackColider : MonoBehaviour
{
    GameObject player;
    Transform distance;

    GameObject destroy;
    GameObject target;
    float target_distance = 1000;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        target = player.transform.Find("target").gameObject;
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
            float dis = Vector3.Distance(player.transform.position, collision.transform.position);
            if (target_distance > dis)
            {
                target_distance = dis;
                distance = collision.transform;
                destroy = collision.gameObject;
            }
            player.SendMessage("isAttacks", true);
            player.SendMessage("attack", distance.position);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        target_distance = 1000;
        target.SetActive(false);
        player.SendMessage("isAttacks", false);
    }
}
