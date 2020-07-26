using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jetEnemyRed : MonoBehaviour
{
    private Vector2 firstPosition;
    private bool reset = false;
    private SpriteRenderer myRender;
    private float counter;
    private float currentTime;
    private float targetTime = 2f;
    public float speed;

    public GameObject tama;
    private Vector3 targetPosition;
    private Vector3 gunPosition;

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
        gunPosition = transform.Find("gun").transform.position;
        targetPosition = transform.Find("target").transform.position;
        if (counter > 0)
        {
            counter -= Time.deltaTime;
        }
        currentTime += Time.deltaTime;
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

    public void fire(){
        if (targetTime < currentTime && !reset)
        {
            currentTime = 0;
            GameObject t = Instantiate(tama) as GameObject;
            t.transform.parent = transform;
            t.transform.position = gunPosition;
            Vector2 vec = targetPosition - gunPosition;
            t.GetComponent<Rigidbody2D>().velocity = vec * speed;
        }
    }
}
