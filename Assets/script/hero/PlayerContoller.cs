using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerContoller : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed;
    public float jumpForce;
    private float moveInput;

    private bool isGounded;
    public Transform feetPos;
    public float checkRadius;
    public LayerMask whatIsGournd;

    private float jumpTimeCounter;
    public float jumpTime;
    private bool isJumpinig;

    public float gravity;

    public string gravityMode;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (gravityMode == "up" || gravityMode=="down")
        {
            rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
        }else if (gravityMode == "right")
        {
            rb.velocity = new Vector2(rb.velocity.x, moveInput * speed);
        }else if(gravityMode == "left")
        {
            rb.velocity = new Vector2(rb.velocity.x, moveInput * -speed);
        }
  
        Vector3 gravityVector = gravietyDirection(gravityMode);
        rb.AddForce(gravityVector);
    }

    void Update()
    {
        isGounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGournd);
        if(isGounded == true && Input.GetKeyDown(KeyCode.Space))
        {
            isJumpinig = true;
            jumpTimeCounter = jumpTime;
            jump(gravityMode);
        }

        if (Input.GetKey(KeyCode.Space) && isJumpinig == true)
        {
            if (jumpTimeCounter > 0)
            {
                jump(gravityMode);
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumpinig = false;
            }

        }

        if(Input.GetKeyUp(KeyCode.Space))
        {
            isJumpinig = false;
        }
    }

    private Vector3 gravietyDirection(string mode)
    {
        switch(mode)
        {
            case "up":
                Vector3 gravityVectorUp = new Vector3(0, -gravity, 0);
                transform.eulerAngles = new Vector3(0, 0, 180f);
                return gravityVectorUp;

            case "left":
                Vector3 gravityVectorLeft = new Vector3 (gravity, 0, 0);
                transform.eulerAngles = new Vector3(0, 0, -90f);
                return gravityVectorLeft;

            case "right":
                Vector3 gravityVectorRight = new Vector3(-gravity, 0, 0);
                transform.eulerAngles = new Vector3(0, 0, 90f);
                return gravityVectorRight;

            default:
                Vector3 gravityVectorDown = new Vector3(0, gravity, 0);
                transform.eulerAngles = Vector3.zero;
                return gravityVectorDown;
        }
    }

    private void jump(string mode)
    {
        switch (mode)
        {
            case "up":
                rb.velocity = Vector2.down * jumpForce;
                break;

            case "left":
                rb.velocity = Vector2.right * jumpForce;
                break;

            case "right":
                rb.velocity = Vector2.left * jumpForce;
                break;

            default:
                rb.velocity = Vector2.up * jumpForce;
                break;
        }
    }

    public void changeGravityMode(string mode)
    {
        if(mode == "up" || mode == "down" || mode == "left" || mode =="right")
        {
            gravityMode = mode;
        }else
        {
            Debug.LogError("gravity change error");
        }
    }
}
