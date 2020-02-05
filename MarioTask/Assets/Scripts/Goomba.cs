using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goomba : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float activationDistance = 15f;

    private bool isGoingRight = false;
    private ScoreManager scoreMan;
    private Rigidbody2D rig;
    private PlayerController player;
    private CapsuleCollider2D col;
    private Animator anim;
    private bool inRange = false;
    private Rigidbody2D colRB;

    void Awake()
    {
        rig = GetComponentInParent<Rigidbody2D>();
    }

    void Start()
    {
        scoreMan = FindObjectOfType<ScoreManager>();
        player = FindObjectOfType<PlayerController>();
        col = this.GetComponent<CapsuleCollider2D>();
        anim = this.GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (Mathf.Abs(transform.position.x - player.transform.position.x) < activationDistance)
            inRange = true;

        if (inRange)
        {
            if (isGoingRight)
                rig.velocity = Vector2.Lerp(rig.velocity, new Vector2(moveSpeed, Mathf.Clamp(rig.velocity.y, -8, 15)),
                    0.5f); //go right
            else
                rig.velocity = Vector2.Lerp(rig.velocity, new Vector2(-moveSpeed, Mathf.Clamp(rig.velocity.y, -8, 15)),
                    0.5f); //go left
        }
    }

    public void Stomp(Rigidbody2D playerRB)
    {
        playerRB.velocity += new Vector2(0, 15f);
        scoreMan.Goomba();
        anim.SetBool("GoombaSquashed", true);
    }

    public void Death(float hitDirection)
    {
        rig.gravityScale = 8;
        anim.enabled = false;
        this.transform.eulerAngles = new Vector3(180, 0, 0);
        col.enabled = false;
        Destroy(gameObject, 1f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            Rigidbody2D colRb = collision.transform.GetComponent<Rigidbody2D>();
            if (colRb.velocity.y < 0)
            {
                Stomp(colRb);
            }
            else
            {
                GameObject.FindWithTag("Player").GetComponent<CapsuleCollider2D>().isTrigger = true;// player will die if trigger is true
                player.Die();// got to the playercontroller and execute die method
            }
        }

        if (collision.transform.tag == "Mushroom") //if we touch Mushroom, change direction
        {
            Rigidbody2D colRB = collision.transform.GetComponent<Rigidbody2D>();
            if ((colRB.velocity.x >= 0 && rig.velocity.x < 0) || (colRB.velocity.x <= 0 && rig.velocity.x > 0))
                rig.velocity = new Vector2(0, rig.velocity.y);
            isGoingRight = !isGoingRight;
        }
    }

    //Sometimes there are situations when collision with Tiles or Goomba is constant
    private void OnCollisionStay2D(Collision2D collision)
    {
        if ((collision.gameObject.tag == "Tiles" || collision.gameObject.tag == "Goomba") &&
            Mathf.Round(rig.velocity.x) == 0f) //if it's no longer going left/right - change direction
            isGoingRight = !isGoingRight;
    }

    public void GoombaDestroy()
    {
        Destroy(this.gameObject);
    }
}