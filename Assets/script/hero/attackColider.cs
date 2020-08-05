using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class attackColider : MonoBehaviour
{
    private GameObject player;
    private Transform distance;

    private GameObject destroy;
    private GameObject target;

    private AudioSource targetSound;

    public GameObject anim;
    private float target_distance = 1000;
    private bool playSound = true;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        target = player.transform.Find("target").gameObject;
        targetSound = transform.GetComponent<AudioSource>();
    }

    public void attackDestoroy()
    {
        Instantiate(anim, destroy.transform.position, destroy.transform.rotation);
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

            if (playSound)
            {
                playSound = false;
                targetSound.Play();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "enemy")
        {
            target_distance = 1000;
            target.SetActive(false);
            playSound = true;
            player.SendMessage("isAttacks", false);
        }
    }
}
