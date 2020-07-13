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

    private float angleTmp;
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
 
        Vector3 gravityVector = gravietyDirection(gravityMode);
        rb.AddForce(gravityVector);

        ray();
    }

    private Vector3 gravietyDirection(string mode)
    {
        switch(mode)
        {
            case "up":
                Vector3 gravityVectorUp = new Vector3(0, -gravity, 0);
                return gravityVectorUp;

            case "left":
                Vector3 gravityVectorLeft = new Vector3 (gravity, 0, 0);
                return gravityVectorLeft;

            case "right":
                Vector3 gravityVectorRight = new Vector3(-gravity, 0, 0);
                return gravityVectorRight;

            default:
                Vector3 gravityVectorDown = new Vector3(0, gravity, 0);
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

    private Ray getGravityRay()
    {
        Ray ray = new Ray(transform.position, -transform.up);
        if (gravityMode == "down")
        {
            ray = new Ray(transform.position + new Vector3(0, -0.5f, 0), -transform.up);
        }
        else if (gravityMode == "up")
        {
            ray = new Ray(transform.position + new Vector3(0, 0.5f, 0), -transform.up);
        }
        else if (gravityMode == "left")
        {
            ray = new Ray(transform.position + new Vector3(-0.5f, 0, 0), -transform.up);
        }
        else if (gravityMode == "right")
        {
            ray = new Ray(transform.position + new Vector3(0.5f, 0, 0), -transform.up);
        }
        return ray;
    }

    private void ray()
    {
        Ray ray = getGravityRay();
        
        RaycastHit2D[] hits = new RaycastHit2D[2];
        int h = Physics2D.RaycastNonAlloc(ray.origin, ray.direction, hits);
        Debug.DrawRay(ray.origin, ray.direction, Color.red);
        if (h > 1)
        {
            if (hits[1].collider.tag == "ground")
            {
   
                Quaternion q = Quaternion.FromToRotation(
                        transform.up,
                        hits[1].normal);
                transform.rotation *= q;
            }

        }
    }

    public void changeGravityMode(string mode)
    {
        ray();
        if (gravityMode != mode)
        {
            isJumpinig = false;
            if (mode == "up" || mode == "down" || mode == "left" || mode =="right")
            {
                gravityMode = mode;
            }
            else
            {
                Debug.LogError("gravity change error");
            }
        }
        else
        {
            return;
        }

        switch (mode)
        {
            case "up":
                transform.eulerAngles = new Vector3(0, 0, 180f);
                break;

            case "down":
                transform.eulerAngles = Vector3.zero;
                break;

            case "left":
                transform.eulerAngles = new Vector3(0, 0, -90f);
                break;

            case "right":
                transform.eulerAngles = new Vector3(0, 0, 90f);
                break;
        }
    }
}
