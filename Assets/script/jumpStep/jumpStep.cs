﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jumpStep : MonoBehaviour
{
    private Vector3 target;
    public float distance;
    public float jumpCounter;
    private float counter;
    private Rigidbody2D player;
    private Animator jumpAnimator;
    // Start is called before the first frame update
    void Start()
    {
        target = transform.Find("jumpTarget").transform.position;
        jumpAnimator = gameObject.GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(counter >= 0)
        {
          counter = counter - Time.deltaTime;
        }

        if(counter <= 0)
        {
            player.SendMessage("isMoveStop", false);
            jumpAnimator.SetBool("jump", false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            Vector3 shotVector = target - transform.position;
            player.SendMessage("isMoveStop", true);
            player.transform.position = transform.position + new Vector3(0, 1);
            player.mass = 2;
            player.velocity = shotVector * distance;
            counter = jumpCounter;
            jumpAnimator.SetBool("jump", true);
        }
    }
}
