using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerContoller : MonoBehaviour
{
    private GameObject GameManager;

    private GameObject CanvasPause;
    private Button backButton;

    private bool pause;
    private bool miss;

    private Rigidbody2D rb;
    [SerializeField] private ContactFilter2D filter2d;
    private Animator animator;

    public GameObject helthObject;
    private Animator helthAnimator;
    private Transform helths;
    private int helth;

    public GameObject gravityBar;

    public Text timeText;
    private float time;
    private int minite;

    private AudioSource dashSound;
    private AudioSource jumpSound;
    private AudioSource playerDamageSound;
    private AudioSource walkSound;
    private AudioSource dashEffectSound;

    private bool dashSoundIsVolume = false;
    private bool playDashsound = true;
    public float fadeSecond;
    private float fadeDeltaTime = 0;

    private bool walkSoundIsVolume = false;
    public float fadeWalkSecond;
    private float fadeWalkDeltaTime = 0;

    public float speed;
    private float moveInput;
    private Vector2 movementNomal = new Vector2(0, 0);

    private bool isGounded;

    public float jumpForce;
    private float jumpTimeCounter;
    public float jumpTime;
    private bool isJumpinig;
    public float isStop;
    private bool jumpStop;

    public float gravity;
    public string gravityMode;

    private Transform gravityDown;
    private Transform gravityRightUp;
    private Transform gravityRightDown;
    private Transform gravityJump;

    private bool dash = false;
    private bool isDash = false;
    public GameObject dashEffect;
    float speedUp = 1f;

    private GameObject attackColider;
    private float attackCounter;
    public float attackTime;

    private GameObject target;
    Vector3 warp;
    private Vector2 warpVector;
    private Vector2 warpVectorTmp;
    public bool isAttack;

    private bool stop;
    private float damageCountor = 0;
    private bool isStopMoveDamage = false;

    private GameObject particleSmork;
    public GameObject boom;
    public GameObject warpEffect;
    private GameObject dashEffectOver;
    private Transform particle;

    // Start is called before the first frame update
    void Start()
    {
        GameManager = GameObject.Find("Game Manager");
        rb = GetComponent<Rigidbody2D>();
        helths = helthObject.transform;
        helthAnimator = helths.parent.GetComponent<Animator>();
        animator = GetComponent<Animator>();
        Transform gravityTrandform = transform.Find("Gravity").gameObject.transform; 
        gravityDown = gravityTrandform.Find("gravityDown").gameObject.transform;
        gravityRightUp = gravityTrandform.Find("gravityRightUp").gameObject.transform;
        gravityRightDown = gravityTrandform.Find("gravityRightDown").gameObject.transform;
        gravityJump = gravityTrandform.Find("isGrounded").gameObject.transform;
        attackColider = transform.Find("attackColider").gameObject;
        target = transform.Find("target").gameObject;
        particleSmork = transform.Find("Particle System").gameObject;
        dashEffect = transform.Find("dashEffect").gameObject;
        dashEffectOver = transform.Find("dashEffectOver").gameObject;
        particle = Camera.main.transform.Find("Concentration");
        AudioSource[] audios = transform.GetComponents<AudioSource>();
        CanvasPause = GameObject.Find("CanvasPause");
        backButton = CanvasPause.transform.Find("backButton").GetComponent<Button>();
        CanvasPause.SetActive(false);
        dashSound = audios[0];
        jumpSound = audios[1];
        playerDamageSound = audios[2];
        walkSound = audios[3];
        dashEffectSound = audios[4];


        walkSound.Play();
        walkSound.volume = 0;

        dashSound.Play();
        dashSound.volume = 0;

        helth = 5;

        time = 0;
        minite = 0;

        pause = false;
        miss = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rayGravity();
        moveInput = Input.GetAxisRaw("Horizontal");

        if(attackCounter > 0)
        {
            attackColider.SetActive(false);
            isDash = false;
            target.SetActive(false);
            speedUp = 0.3f;
            return;
        }

        if (isStopMoveDamage || stop || jumpStop)
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
            fadeOut();
            fadeOutWalk();
            dashEffect.SetActive(false);
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
        if (Input.GetKeyDown(KeyCode.Escape) && !pause && !miss)
        {
            activePause();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape) && pause)
        {
            unPause();
        }

        if (pause)
        {
            return;
        }

        time += Time.deltaTime;
        if(time >= 60f)
        {
            minite++;
            time -= 60f;
        }
        timeText.text = "Time " + minite.ToString("00") + ":" + string.Format("{00:00.00}", time);

        rayGravity();
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

        if (Input.GetKeyDown(KeyCode.LeftShift) && !stop)
        {
            isDash = true;
        }

        if (Input.GetKey(KeyCode.LeftShift) && moveInput != 0 && isDash)
        {
            speedUp = 2f;
            animator.SetBool("dash", true);
            dash = true;
            dashEffect.SetActive(true);
            dashEffectOver.SetActive(true);
            particle.gameObject.SetActive(true);
            if (playDashsound && attackCounter <= 0)
            {
                playDashsound = false;
                dashEffectSound.Play();
            }
        }
        else
        {
            speedUp = 1f;
            animator.SetBool("dash", false);
            dash = false;
            dashEffect.SetActive(false);
            dashEffectOver.SetActive(false);
            particle.gameObject.SetActive(false);
            fadeOut();
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            playDashsound = true;
        }

        isGounded = downCheck();
        if (isGounded)
        {
            attackColider.SetActive(false);
            target.SetActive(false);
            animator.SetBool("up", false);
            animator.SetBool("down", false);
            animator.SetBool("right", false);
            particleSmork.SetActive(true);
            rb.mass = 0.5f;

            if(moveInput != 0 && !dash)
            {
                fadeInWalk();
            }
            else
            {
                fadeOutWalk();
            }

            if (dash && moveInput != 0)
            {
                fadeIn();
            }
            else
            {
                fadeOut();
            }

            if (isStopMoveDamage)
            {
                animator.SetBool("damage", false);
                helthAnimator.SetBool("damage", false);
                isStopMoveDamage = false;
            }
        }
        else
        {
            fadeOut();
            fadeOutWalk();
            rb.mass = 0.5f;
            attackColider.SetActive(true);
            animator.SetBool("down", true);
            particleSmork.SetActive(false);
            
            if (Input.GetKeyDown(KeyCode.Space) && attackCounter <= 0 && warpVector != warpVectorTmp && isAttack && !isStopMoveDamage)
            {
                Time.timeScale = 0.8f;
                warpVectorTmp = warpVector;
                attackColider.gameObject.SendMessage("attackDestoroy");
                attackCounter = attackTime;
                rb.position = warpVector + new Vector2(0, 1);
                Instantiate(warpEffect, warpVector, transform.rotation);
            }
        }

        if(attackCounter > 0)
        {
            jump(gravityMode, 20);
            dashEffect.SetActive(false);
            attackCounter -= Time.deltaTime;
            if(attackCounter <= 0)
            {
                Time.timeScale = 1f;
            }
            return;
        }

        if(isGounded && Input.GetKeyDown(KeyCode.Space) && !jumpStop)
        {
            jumpSound.Play();
            rb.mass = 2;
            animator.SetBool("up", true);
            isJumpinig = true;
            jumpTimeCounter = jumpTime;
            jump(gravityMode, jumpForce);
        }

        if (Input.GetKey(KeyCode.Space) && isJumpinig == true)
        {
            fadeOut();
            fadeOutWalk();
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
            jumpSound.Stop();
            rb.mass = 2;
            animator.SetBool("up", false);
            if (!isGounded)
            {
                animator.SetBool("down", true);
            }
            isJumpinig = false;
        }
    }

    private Vector3 gravietyDirection(string mode)
    {
        float step = 500 * Time.deltaTime;
        switch (mode)
        {
            case "up":
                Vector3 gravityVectorUp = new Vector3(0, -gravity, 0);
                gravityBar.transform.rotation = Quaternion.RotateTowards(gravityBar.transform.rotation, Quaternion.Euler(0, 0, 0), step);
                return gravityVectorUp;

            case "left":
                Vector3 gravityVectorLeft = new Vector3 (gravity, 0, 0);
                gravityBar.transform.rotation = Quaternion.RotateTowards(gravityBar.transform.rotation, Quaternion.Euler(0, 0, 90f), step);
                return gravityVectorLeft;

            case "right":
                Vector3 gravityVectorRight = new Vector3(-gravity, 0, 0);
                gravityBar.transform.rotation = Quaternion.RotateTowards(gravityBar.transform.rotation, Quaternion.Euler(0, 0, -90f), step);
                return gravityVectorRight;

            default:
                Vector3 gravityVectorDown = new Vector3(0, gravity, 0);
                gravityBar.transform.rotation = Quaternion.RotateTowards(gravityBar.transform.rotation, Quaternion.Euler(0, 0, 180f), step);
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
        int h = Physics2D.RaycastNonAlloc(ray.origin, ray.direction, hits, 1f);
        Debug.DrawRay(ray.origin, ray.direction, Color.red);
        if (h > 1)
        {
            int index = 1;
            if (hits[index].collider.tag == "ground" && isGounded)
            {
                movementNomal = new Vector2(hits[index].normal.x, hits[index].normal.y);
                Quaternion q = Quaternion.FromToRotation(
                        transform.up,
                        hits[index].normal);
                if (movementNomal.x < 1 && movementNomal.x > -1)
                {
                   Quaternion rote = transform.rotation * q;
                    transform.rotation = rote;
                }
            }
        }else
        {
            gravitySwitch(gravityMode);
        }
    }

    private bool downCheck()
    {
        Ray ray = new Ray(gravityJump.position, -gravityJump.up);
        RaycastHit2D hitDown = Physics2D.Raycast(ray.origin, ray.direction, isStop);
        if (hitDown.collider != null)
        {
            return hitDown.collider.tag == "ground";
        }
        return false;
    }

    public void changeGravityMode(string mode)
    {
        string bforeode = gravityMode;
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
                if (bforeode == "down")
                {
                    transform.position += new Vector3(0, 1, 0);
                }
                break;

            case "down":
                transform.eulerAngles = Vector3.zero;
                if(bforeode == "up")
                {
                    transform.position -= new Vector3(0, 1, 0);
                }
                break;

            case "left":
                transform.eulerAngles = new Vector3(0, 0, -90f);
                break;

            case "right":
                transform.eulerAngles = new Vector3(0, 0, 90f);
                break;
        }
    }

    private void gravitySwitch(string mode)
    {
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
            rb.velocity = new Vector2(rb.velocity.x - movementNomal.x, speed);
        }
        else if (gravityMode == "left")
        {
            rb.velocity = new Vector2(rb.velocity.x - movementNomal.x, -speed);
        }
    }

    void fadeIn()
    {
        if (!dashSoundIsVolume && Input.GetKey(KeyCode.LeftShift))
        {
            fadeDeltaTime += Time.deltaTime;
            if(fadeDeltaTime >= fadeSecond)
            {
                fadeDeltaTime = fadeSecond;
                dashSoundIsVolume = true;
            }

            dashSound.volume = (fadeDeltaTime / fadeSecond) - 0.5f;
        }
        else
        {
            dashSoundIsVolume = true;
        }
    }

    void fadeOut()
    {
        if (dashSoundIsVolume)
        {
            fadeDeltaTime += Time.deltaTime;
            if (fadeDeltaTime >= fadeSecond)
            {
                fadeDeltaTime = fadeSecond;
                dashSoundIsVolume = false;
            }

            dashSound.volume = 1.0f - fadeDeltaTime / fadeSecond;
        }
    }

    void fadeInWalk()
    {
        if (!walkSoundIsVolume)
        {
            fadeWalkDeltaTime += Time.deltaTime;
            if (fadeWalkDeltaTime >= fadeWalkSecond)
            {
                fadeWalkDeltaTime = fadeWalkSecond;
                walkSoundIsVolume = true;
            }

            walkSound.volume = (fadeWalkDeltaTime / fadeWalkSecond) - 0.5f;
        }
        else
        {
            walkSoundIsVolume = true;
        }
    }

    void fadeOutWalk()
    {
        if (walkSoundIsVolume)
        {
            fadeWalkDeltaTime += Time.deltaTime;
            if (fadeWalkDeltaTime >= fadeWalkSecond)
            {
                fadeWalkDeltaTime = fadeWalkSecond;
                walkSoundIsVolume = false;
            }

            walkSound.volume = 1.0f - fadeWalkDeltaTime / fadeWalkSecond;
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
        if (transform.localScale.x > 0 && stops)
        {
            if(moveInput < 0)
            {
                stop = false;
                return;
            }    
        }
        else if (transform.localScale.x < 0 && stops)
        {
            if (moveInput > 0)
            {
                stop = false;
                return;
            }
        }

        stop = stops;
    }

    public void isJump(bool jump)
    {
        jumpStop = jump;
    }

    private void helthCount()
    {
        if(helth == 0)
        {
            return;
        }
        helths.GetChild(helth + 1).gameObject.SetActive(false);
        helths.GetChild(helth).gameObject.SetActive(true);
    }

    public void unPause()
    {
        if (pause)
        {
            EventSystem.current.SetSelectedGameObject(null);
            pause = false;
            CanvasPause.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    private void activePause()
    {
        pause = true;
        CanvasPause.SetActive(true);
        EventSystem.current.SetSelectedGameObject(backButton.gameObject, null);
        Time.timeScale = 0f;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "enemy" || collision.gameObject.tag == "trap")
        {
            if (dash && collision.gameObject.tag == "enemy")
            {
                Instantiate(boom, collision.transform.position, collision.transform.rotation);
                Destroy(collision.gameObject);
            }
            else
            {
                animator.SetBool("damage", true);
                helthAnimator.SetBool("damage", true);
                isStopMoveDamage = true;
                rb.mass = 3;
                damageCountor = 0.4f;
                playerDamageSound.Play();
                helth--;
                if (helth == 0)
                {
                    GameManager.SendMessage("miss");
                }
                else
                {
                    helthCount();
                }
            }
        }else if(collision.gameObject.tag == "miss")
        {
            miss = true;
            GameManager.SendMessage("miss");
        }
    }
}