using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fire : MonoBehaviour
{
    private Transform enemy;
    // Start is called before the first frame update
    void Start()
    {
        enemy = transform.parent.gameObject.transform;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            enemy.SendMessage("fire");
        }
    }
}
