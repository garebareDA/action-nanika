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
    public LayerMask whatIsGournd;

    [SerializeField] private ContactFilter2D filter2d;

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
    
    private bool isStopMoveDamage = false;

    private bool dash = false;
    private bool isDash = false;

    private GameObject attackColider;

    private float attackCounter;
    public float attackTime;
    float speedUp = 1f;

    private GameObject target;

    private Vector2 warpVector;
    private Vector2 warpVectorTmp;
    public bool isAttack;

    Vector3 warp;

    private bool stop;

    private float damageCountor = 0;
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

        attackColider = transform.Find("attackColider").gameObject;

        target = transform.Find("target").gameObject;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        bool checkeRay = checkedRay(moveInput);

        if(attackCounter > 0)
        {
            attackColider.SetActive(false);
            isDash = false;
            target.SetActive(false);
            speedUp = 0.3f;
            return;
        }

        if (isStopMoveDamage || checkeRay || stop)
        {   
            return;
        }

        if (moveInput < 0)
        {
            animator.SetBool("walk", true);
            transform.localScale = new Vector3(-5, 5, 0);
        }
        else if (moveInput > 0)
        {
            animator.SetBool("walk", true);
            transform.localScale = new Vector3(5, 5, 0);
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

        if (gravityMode == "down" || gravityMode == "up")
        {
            move(moveInput * speed * speedUp - movementNomal.x);
        }else if (gravityMode == "left" || gravityMode == "right")
        {
            move(moveInput * speed * speedUp - movementNomal.y);
        }
    }

    void Update()
    {
        Vector3 gravityVector = gravietyDirection(gravityMode);
        rb.AddForce(gravityVector);

        if (0 <= damageCountor)
        {
            damageCountor = damageCountor - Time.deltaTime;
        }
        else if(isStopMoveDamage)
        {
            rb.mass = 0.5f;
        }

        if (isStopMoveDamage && damageCountor > 0)
        {
            damage(15);
            return;
        }

        bool checkeRay = checkedRay(moveInput);
        if (Input.GetKeyDown(KeyCode.LeftShift) && !checkeRay)
        {
            isDash = true;
        }

        if (Input.GetKey(KeyCode.LeftShift) && moveInput != 0 && isDash)
        {
            speedUp = 2f;
            animator.SetBool("dash", true);
            dash = true;
        }
        else
        {
            speedUp = 1f;
            animator.SetBool("dash", false);
            dash = false;
        }

        rayGravity();
        isGounded = downCheck();
        if (isGounded)
        {
            attackColider.SetActive(false);
            target.SetActive(false);
            animator.SetBool("up", false);
            animator.SetBool("down", false);
            animator.SetBool("right", false);
            rb.mass = 0.5f;
            if (isStopMoveDamage)
            {
                animator.SetBool("damage", false);
                isStopMoveDamage = false;
            }
        }
        else
        {
            attackColider.SetActive(true);
            animator.SetBool("down", true);
            if (Input.GetKeyDown(KeyCode.Space) && attackCounter <= 0 && warpVector != warpVectorTmp && isAttack)
            {
                warpVectorTmp = warpVector;
                attackColider.gameObject.SendMessage("attackDestoroy");
                attackCounter = attackTime;
                transform.position = warpVector + new Vector2(0, 1);
            }
        }

        if(attackCounter > 0)
        {
            jump(gravityMode, 20);
            attackCounter -= Time.deltaTime;
            return;
        }

        if(isGounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.mass = 2;
            animator.SetBool("up", true);
            isJumpinig = true;
            jumpTimeCounter = jumpTime;
            jump(gravityMode, jumpForce);
        }

        if (Input.GetKey(KeyCode.Space) && isJumpinig == true)
        {
            rb.mass = 2;
            if (jumpTimeCounter > 0)
            {
                jump(gravityMode, jumpForce);
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                animator.SetBool("up", false);
                animator.SetBool("down", true);
                isJumpinig = false;
            }
        }

        if(Input.GetKeyUp(KeyCode.Space))
        {
            rb.mass = 2;
            animator.SetBool("up", false);
            if (!isGounded)
            {
                animator.SetBool("down", true);
            }
            isJumpinig = false;
        }

        rayGravity();
    }

    private bool checkedRay(float moveInput)
    {
        float scale = transform.localScale.x;
        bool checks = false;
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
                
            }
        }

        if(hitLeftDown.collider != null)
        {
            if(hitLeftDown.collider.tag == "ground")
            {
            
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
                if(scale > 0)
                {
                    checks = 0 < moveInput;
                }else if(scale < 0)
                {
                    checks = 0 > moveInput;
                }
            }
        }

        if(hitRightDown.collider != null)
        {
            if(hitRightDown.collider.tag == "ground")
            {
                if (scale == 5)
                {
                    checks = 0 < moveInput;
                }
                else if (scale < 0)
                {
                    checks = 0 > moveInput;
                }
            }
        }

        return checks;
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

    private void jump(string mode, float jumpForce)
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
        if (h > 0)
        {
            int index = 0;
            if (hits[index].collider.tag == "ground")
            {
                movementNomal = new Vector2(hits[index].normal.x, hits[index].normal.y);
                Quaternion q = Quaternion.FromToRotation(
                        transform.up,
                        hits[index].normal);
                transform.rotation *= q;
            }
        }
    }

    private bool downCheck()
    {
        Ray ray = new Ray(gravityDown.position, -gravityDown.up);
        RaycastHit2D hitDown = Physics2D.Raycast(ray.origin, ray.direction, isStop);
        if (hitDown.collider != null)
        {
            return hitDown.collider.tag == "ground";
        }

        return false;
    }

    public void changeGravityMode(string mode)
    {
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

    private void damage(float speed)
    {
        jump(gravityMode, 5);
        notMoveGravity();
    }

    private void notMoveGravity()
    {
        if (gravityMode == "up")
        {
            if (transform.localScale.x == 5)
            {
                rb.velocity = new Vector2(speed, rb.velocity.y - movementNomal.y);
            }
            else if (transform.localScale.x == -5)
            {
                rb.velocity = new Vector2(-speed, rb.velocity.y - movementNomal.y);
            }
        }
        else if (gravityMode == "down")
        {
            if (transform.localScale.x == 5)
            {
                rb.velocity = new Vector2(-speed, rb.velocity.y - movementNomal.y);
            }
            else if (transform.localScale.x == -5)
            {
                rb.velocity = new Vector2(speed, rb.velocity.y - movementNomal.y);
            }
        }
        else if (gravityMode == "right")
        {
            rb.velocity = new Vector2(rb.velocity.x - movementNomal.x, -speed);
        }
        else if (gravityMode == "left")
        {
            rb.velocity = new Vector2(rb.velocity.x - movementNomal.x, speed);
        }
    }

    private void move(float speed)
    {
        if (gravityMode == "up")
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y - movementNomal.y);
        }
        else if (gravityMode == "down")
        {
            rb.velocity = new Vector2(speed, rb.velocity.y - movementNomal.y);
        }
        else if (gravityMode == "right")
        {
            rb.velocity = new Vector2(rb.velocity.x - movementNomal.x, -speed);
        }
        else if (gravityMode == "left")
        {
            rb.velocity = new Vector2(rb.velocity.x - movementNomal.x, speed);
        }
    }

    public void attack(Vector3 warp)
    {
        if (!isGounded)
        {
            target.SetActive(true);
            target.transform.position = warp;
            warpVector = warp;
        }
    }

    public void isAttacks(bool i)
    {
        isAttack = i;
    }

    public void isMoveStop(bool stops)
    {
        stop = stops;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "enemy" || collision.gameObject.tag == "trap")
        {
            if (dash && collision.gameObject.tag == "enemy")
            {
                Destroy(collision.gameObject);
            }
            else
            {
                animator.SetBool("damage", true);
                isStopMoveDamage = true;
                rb.mass = 3;
                damageCountor = 0.4f;
            }
        }
    }
}
