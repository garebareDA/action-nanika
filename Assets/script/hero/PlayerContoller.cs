using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerContoller : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed;
    public float jumpForce;
    private float moveInput;
    private Vector2 movementNomal = new Vector2(0, 0);

    private bool isGounded;
    public Transform feetPos;
    public float checkRadius;
    public LayerMask whatIsGournd;

    private float jumpTimeCounter;
    public float jumpTime;
    private bool isJumpinig;

    public float gravity;
    public string gravityMode;

    public float isStop;

    private Transform gravityDown;
    private Transform gravityLeftUp;
    private Transform gravityLeftDown;
    private Transform gravityRightUp;
    private Transform gravityRightDown;

    private Animator animator;


    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        Transform gravityTrandform = transform.Find("Gravity").gameObject.transform; 
        gravityDown = gravityTrandform.Find("gravityDown").gameObject.transform;
        gravityLeftUp = gravityTrandform.Find("gravityLeftUp").gameObject.transform;
        gravityLeftDown = gravityTrandform.Find("gravityLeftDown").gameObject.transform;
        gravityRightUp = gravityTrandform.Find("gravityRightUp").gameObject.transform;
        gravityRightDown = gravityTrandform.Find("gravityRightDown").gameObject.transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {   bool checkeRay = checkedRay(moveInput);
        if (!checkeRay)
        {
            return;
        }
        rayGravity();
        moveInput = Input.GetAxisRaw("Horizontal");

        bool shiftInput = Input.GetKey(KeyCode.LeftShift);

        float speedUp = 1;
        if (shiftInput && moveInput != 0)
        {
            speedUp = 1.5f;
            animator.SetBool("dash", true);
        }else if (!shiftInput || moveInput == 0)
        {
            animator.SetBool("dash", false);
        }

        if (!isGounded && !shiftInput)
        {
            speedUp = 0.4f;
        }

        if (moveInput < 0)
        {
            animator.SetBool("walk", true);
            if (gravityMode == "up")
            {
                transform.localScale = new Vector3(5, 5, 0);
            }
            else
            {
                transform.localScale = new Vector3(-5, 5, 0);
            }
        }
        else if (moveInput > 0)
        {
            animator.SetBool("walk", true);
            if(gravityMode == "up")
            {
                transform.localScale = new Vector3(-5, 5, 0);
            }
            else
            {
                transform.localScale = new Vector3(5, 5, 0);
            }
        }
        else
        {
            animator.SetBool("walk", false);
        }

        RaycastHit2D hit = Physics2D.Raycast(gravityDown.position, -Vector2.up, 1f, 8);
        if (hit.collider != null && Mathf.Abs(hit.normal.x) > 0.1f)
        {
            if (!isGounded)
            {
                movementNomal = hit.normal;
            }
            else
            {
                movementNomal = Vector2.zero;
            }
        }

        if (gravityMode == "up")
        {
            rb.velocity = new Vector2(moveInput * -speed * speedUp - movementNomal.x, rb.velocity.y - movementNomal.y);
        } else if (gravityMode == "down")
        {
            rb.velocity = new Vector2(moveInput * speed * speedUp - movementNomal.x, rb.velocity.y - movementNomal.y);
        } else if (gravityMode == "right")
        {
            rb.velocity = new Vector2(rb.velocity.x - movementNomal.x, moveInput * speed * speedUp);
        } else if (gravityMode == "left")
        {
            rb.velocity = new Vector2(rb.velocity.x - movementNomal.x, moveInput * -speed * speedUp);
        }
    }

    void Update()
    {
        isGounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGournd);

        if (isGounded)
        {
            animator.SetBool("up", false);
            animator.SetBool("down", false);
            animator.SetBool("right", false);
        }

        if(isGounded && Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetBool("up", true);
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
                animator.SetBool("up", false);
                isDashJump();
                isJumpinig = false;
            }
        }

        if(Input.GetKeyUp(KeyCode.Space))
        {
            animator.SetBool("up", false);
            isDashJump();
            isJumpinig = false;
        }
 
        Vector3 gravityVector = gravietyDirection(gravityMode);
        rb.AddForce(gravityVector);
        rayGravity();
    }

    private void isDashJump()
    {
        if(Input.GetKey(KeyCode.LeftShift))
        {
            animator.SetBool("down", true);
        }
        else
        {
            animator.SetBool("right", true);
        }
    }

    private bool checkedRay(float moveInput)
    {
        Ray rayLeftUp = new Ray(gravityLeftUp.position, -gravityLeftUp.up);
        Debug.DrawRay(rayLeftUp.origin, rayLeftUp.direction, Color.red);
        RaycastHit2D hitLeftUp = Physics2D.Raycast(rayLeftUp.origin, rayLeftUp.direction, isStop);

        Ray rayLeftDown = new Ray(gravityLeftDown.position, -gravityLeftUp.up);
        Debug.DrawRay(rayLeftDown.origin, rayLeftDown.direction, Color.red);
        RaycastHit2D hitLeftDown = Physics2D.Raycast(rayLeftDown.origin, rayLeftDown.direction, isStop);
        if (hitLeftUp.collider != null)
        {
            if (hitLeftUp.collider.tag == "ground")
            {
                return moveInput < 0;
            }
        }

        if(hitLeftDown.collider != null)
        {
            if(hitLeftDown.collider.tag == "ground")
            {
                return moveInput < 0;
            }
        }

        Ray rayRightUp = new Ray(gravityRightUp.position, -gravityRightUp.up);
        Debug.DrawRay(rayRightUp.origin, rayRightUp.direction, Color.red);
        RaycastHit2D hitRightUp = Physics2D.Raycast(rayRightUp.origin, rayRightUp.direction, isStop);

        Ray rayRightDown = new Ray(gravityRightDown.position, -gravityRightDown.up);
        Debug.DrawRay(rayRightDown.origin, rayRightDown.direction, Color.red);
        RaycastHit2D hitRightDown = Physics2D.Raycast(rayRightDown.origin, rayRightDown.direction, isStop);
        if (hitRightUp.collider != null)
        {
            if (hitRightUp.collider.tag == "ground")
            {
                return moveInput > 0;
            }
        }

        if(hitRightDown.collider != null)
        {
            if(hitRightDown.collider.tag == "ground")
            {
                return moveInput > 0;
            }
        }

        return true;
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

    private void rayGravity()
    {
        Ray ray = new Ray(gravityDown.position, -gravityDown.up);
        RaycastHit2D[] hits = new RaycastHit2D[2];
        int h = Physics2D.RaycastNonAlloc(ray.origin, ray.direction, hits);
        Debug.DrawRay(ray.origin, ray.direction, Color.red);
        if (h > 1)
        {
            if (hits[1].collider.tag == "ground")
            {
                movementNomal = new Vector2(hits[1].normal.x, hits[1].normal.y);
                Quaternion q = Quaternion.FromToRotation(
                        transform.up,
                        hits[1].normal);
                transform.rotation *= q;
            }
        }
    }

    public void changeGravityMode(string mode)
    {
        rayGravity();
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
