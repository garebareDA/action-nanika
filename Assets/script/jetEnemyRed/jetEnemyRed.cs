using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jetEnemyRed : MonoBehaviour
{
    private Vector2 firstPosition;
    private bool reset = false;
    private SpriteRenderer myRender;
    private float counter;

    // Start is called before the first frame update
    void Start()
    {
        myRender = gameObject.GetComponent<SpriteRenderer>();
        firstPosition = transform.position;
        counter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (counter > 0)
        {
            counter -= Time.deltaTime;
        }

        if (!reset)
        {

        }
        else
        {
            transform.position = firstPosition;
        }

    }

    void OnBecameVisible()
    {
        if (counter <= 0)
        {
            myRender.color = new Color(myRender.color.r, myRender.color.g, myRender.color.b, 100);
            gameObject.GetComponent<Collider2D>().enabled = true;
            reset = false;
        }
    }

    private void OnBecameInvisible()
    {
        transform.position = firstPosition;
        myRender.color = new Color(myRender.color.r, myRender.color.g, myRender.color.b, 0);
        gameObject.GetComponent<Collider2D>().enabled = false;
        reset = true;
        counter = 0.1f;
    }
}
