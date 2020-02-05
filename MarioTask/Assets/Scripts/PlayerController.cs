using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")] public float maxHorizontalSpeed = 12f;
    public float maxVerticalSpeed = 16f;
    public float movementForce = 50f;
    public float jumpVelocity = 15f;
    public float jumpTime = 0.5f;

    [Header("Rigidbody")] public float mass = 0.75f;
    public float linearDrag = 1.5f;
    public float gravityScale = 5f;

    [Header("Ground Check")] public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundMask;

    [Header("Animators")] public RuntimeAnimatorController bigMarioAnimatorController;
    public RuntimeAnimatorController smallMarioAnimatorController;

    [Header("Death Mechanics")] public float invulnerabilityTime = 2f;
    public float deathHeight = -10f;

    [Header("Sounds")] public AudioClip smallJumpSound;
    public AudioClip bigJumpSound;
    public AudioClip firework;

    private AudioSource audioSource;

    private bool isFacingRight = true;
    private bool isTouchingGround = true;

    [HideInInspector] public bool isJumping = false;

    [HideInInspector] public bool poweredUp = false;

    [HideInInspector] public bool isDead = false;

    [HideInInspector] public bool isInvulnerable = false;

    private float movementInput = 0f;
    private float jumpTimeCounter = 0f;
    private float invulnerabilityTimer = 0f;

    private Rigidbody2D playerRigidbody2D;

    private CapsuleCollider2D playerCapsuleCollider2D;

    private Animator playerAnimator;

    private SpriteRenderer playerSpriteRenderer;

    private PlayerController instance = null;

    private bool takeAwayControll = false; //taking away control so Mario would not stick to the side

    // Extra Variables
    private bool gettingPower = false;
    private bool isReset = false;
    private bool IsKeyBoardEnable = true;
    private bool tunTheFirework = false;

    private ParticleSystem fireworkParticleSystem;
    
    GameObject fireworkGameObject;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        fireworkGameObject = GameObject.Find("Firework");
    }

    void Start()
    {
        playerRigidbody2D = GetComponent<Rigidbody2D>();
        playerRigidbody2D.mass = mass;
        playerRigidbody2D.drag = linearDrag;
        playerRigidbody2D.gravityScale = gravityScale;
        playerCapsuleCollider2D = GetComponent<CapsuleCollider2D>();
        playerAnimator = GetComponent<Animator>();
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        fireworkGameObject.SetActive(false); // turn off fire work when game start
    }


    void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.BoxCast(this.transform.position, new Vector2(0.4f, 0.1f), 0f, Vector2.down,
            groundCheckRadius, groundMask); //using this for a bigger and more accurate ground check
        isTouchingGround = (hit.collider != null) ? true : false;

        movementInput = Input.GetAxis("Horizontal");

        CheckIfStuck(); //Checks if Mario is trying to walk into the wall and get stuck

        if (GameObject.FindWithTag("Player").transform.position.x < 116f)// level finished
        {
            if (!isDead)
            {
                if ((playerRigidbody2D.velocity.x > 0 && !isFacingRight) ||
                    (playerRigidbody2D.velocity.x < 0 && isFacingRight))
                {
                    playerAnimator.SetBool("turning", true);
                }
                else
                {
                    playerAnimator.SetBool("turning", false);
                }

                float movementForceMultiplier =
                    Mathf.Max(maxHorizontalSpeed - Mathf.Abs(playerRigidbody2D.velocity.x), 1);

                playerRigidbody2D.AddForce(new Vector2(movementInput * movementForce * movementForceMultiplier, 0));

                playerRigidbody2D.velocity =
                    new Vector2(Mathf.Clamp(playerRigidbody2D.velocity.x, -maxHorizontalSpeed, maxHorizontalSpeed),
                        Mathf.Clamp(playerRigidbody2D.velocity.y, -maxVerticalSpeed, maxVerticalSpeed));

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (isTouchingGround)
                    {
                        //Play Jump sound
                        if (!poweredUp)
                            audioSource.PlayOneShot(smallJumpSound);
                        else
                            audioSource.PlayOneShot(bigJumpSound);

                        isJumping = true;
                        playerRigidbody2D.velocity = new Vector2(playerRigidbody2D.velocity.x, jumpVelocity);
                        jumpTimeCounter = jumpTime;
                    }
                }

                if (jumpTimeCounter > 0 && isJumping)
                    if (Input.GetKey(KeyCode.Space))
                    {
                        jumpTimeCounter -= Time.deltaTime;
                        {
                            playerRigidbody2D.velocity = new Vector2(playerRigidbody2D.velocity.x, jumpVelocity);
                        }
                    }

                if (Input.GetKeyUp(KeyCode.Space))
                {
                    isJumping = false;
                    jumpTimeCounter = 0;
                }

                playerAnimator.SetFloat("movementSpeed", Mathf.Abs(playerRigidbody2D.velocity.x));
                playerAnimator.SetBool("touchingGround", isTouchingGround);
            }

            GameObject.FindWithTag("InvisiblePlayer").GetComponent<SpriteRenderer>().enabled = false;
            tunTheFirework = true;
        }
        else
        {
            GameObject.FindWithTag("Player").GetComponent<SpriteRenderer>().enabled = false;// make player invisible and start animation
            if (tunTheFirework == true)// start firework
            {
                GameObject.FindWithTag("InvisiblePlayer").GetComponent<SpriteRenderer>().enabled = true;// make invisible animation player visible
                audioSource.PlayOneShot(firework);// start firework audio 
                fireworkGameObject.SetActive(true);//  make firework activate
                tunTheFirework = false;
            }
        }

        CheckFlipSpriteOfPlayer();// make it cleaner
        GameObject.FindWithTag("Player").GetComponent<CapsuleCollider2D>().isTrigger = false;// player wont die
    } // FixedUpdate


    private void Update()
    {
        if (!isInvulnerable)
        {
            invulnerabilityTimer -= Time.deltaTime;

            float newAlpha = 1f;

            if (playerSpriteRenderer.color.a > 0.51f)
            {
                newAlpha = 0.5f;
            }

            if (invulnerabilityTimer < 0)
            {
                isInvulnerable = false;
                newAlpha = 1f;
            }
            
            playerSpriteRenderer.color = new Color(playerSpriteRenderer.color.r, playerSpriteRenderer.color.g,
                playerSpriteRenderer.color.b, newAlpha);

            if (playerRigidbody2D.position.y < deathHeight)
            {
                SceneManager.LoadScene(1);
            }
        }
        
        // used in other class also  but private modifier didnt allow it
        PowerUp();
        Die();
        CheckIfStuck();
        
        // no need of modifier it is already private
        void OnCollisionExit2D(Collision2D collision)
        {
            takeAwayControll = false; //give back control when it's no longer colliding with anything
        }
    } // Update

    public void PowerUp()
    {
        if (gettingPower)// check if player hit the mushroom
        {
            playerAnimator.runtimeAnimatorController = bigMarioAnimatorController as RuntimeAnimatorController;
            playerCapsuleCollider2D.offset = new Vector2(0, 0.5f);
            playerCapsuleCollider2D.size = new Vector2(0.9f, 2);
            poweredUp = true;
            Invoke("PoweredDown", 5); // wait for 5 sec then power down
        }
    }

    public void PoweredDown()// convert to smaller mario
    {
        SetPower(false);
        playerAnimator.runtimeAnimatorController = smallMarioAnimatorController as RuntimeAnimatorController;
        playerCapsuleCollider2D.offset = new Vector2(0, 0f);
        playerCapsuleCollider2D.size = new Vector2(0.74f, 1);
        poweredUp = false;// work like small mario
    }

    public void Die()
    {
        if (poweredUp && !isDead && !isInvulnerable)
        {
            playerAnimator.runtimeAnimatorController = smallMarioAnimatorController as RuntimeAnimatorController;
            playerCapsuleCollider2D.offset = new Vector2(0, 0f);
            playerCapsuleCollider2D.size = new Vector2(1, 1);
            poweredUp = false;
            isInvulnerable = true;
            invulnerabilityTimer = invulnerabilityTime;
        }

        if (GameObject.FindWithTag("Player").transform.position.y < -4f ||
            GameObject.FindWithTag("Player").GetComponent<CapsuleCollider2D>().isTrigger == true
        ) // if height below -4 mean fall down or get hit by Goomba then Mario dead
        {
            playerRigidbody2D.velocity = new Vector2(0, jumpVelocity);
            playerAnimator.SetBool("dead", true);
            playerCapsuleCollider2D.enabled = false;
            isDead = true;
            isReset = true;// restart the game
        }

        if (isReset)
        {
            Invoke("ResetGame", 0.5f);// wait before restart
        }
    }

    public void ResetGame() // Reset the game
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameObject.FindWithTag("Player").GetComponent<CapsuleCollider2D>().isTrigger = false;// dont let player die
        isReset = false;
    }

    void FlipSprite()
    {
        isFacingRight = !isFacingRight;
        Vector3 tempScale = transform.localScale;
        tempScale.x *= -1;
        transform.localScale = tempScale;
    }

    private void CheckIfStuck()
    {
        //Taking away users control when player is not touching the ground and not moving to any direction
        if (!isTouchingGround && playerRigidbody2D.velocity == Vector2.zero)
            takeAwayControll = true;

        if (takeAwayControll)
            movementInput = 0;

        //if starts touching ground - give control back
        if (isTouchingGround)
            takeAwayControll = false;
    } // CheckIfStuck

    // My extra methods
    public bool GetPower()
    {
        return gettingPower;
    }

    public void SetPower(bool gettingPower)
    {
        this.gettingPower = gettingPower;
    }

    public bool GetReset()
    {
        return isReset;
    }

    public void SetReset(bool isReset)
    {
        this.isReset = isReset;
    }

    void CheckFlipSpriteOfPlayer()
    {
        if (movementInput > 0 && !isFacingRight)
        {
            FlipSprite();
        }
        else if (movementInput < 0 && isFacingRight)
        {
            FlipSprite();
        }

        CheckIfStuck();
    }
} // class