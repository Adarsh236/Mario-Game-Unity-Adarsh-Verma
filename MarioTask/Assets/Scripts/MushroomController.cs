using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomController : MonoBehaviour
{
    public float moveSpeed = 8f;
    private ScoreManager scoreManager;

    public bool isGoingRight = true;
    private Rigidbody2D rig;

    void Awake()
    {
        rig = GetComponentInParent<Rigidbody2D>();
    }

    void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
    }

    void FixedUpdate()
    {
        if (isGoingRight)
            rig.velocity = Vector2.Lerp(rig.velocity, new Vector2(moveSpeed, Mathf.Clamp(rig.velocity.y, -10, 16)), 0.5f); //go right
        else
            rig.velocity = Vector2.Lerp(rig.velocity, new Vector2(-moveSpeed, Mathf.Clamp(rig.velocity.y, -10, 16)), 0.5f); //go left
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().PowerUp();
            scoreManager.Mushroom();
            collision.transform.GetComponent<Rigidbody2D>().velocity -= new Vector2(rig.velocity.x, 0); //When we hit Mario, he gets mushrooms velocity. So workaround is to take it away
            Destroy(this.gameObject);
        }
        if (collision.transform.tag == "Goomba") //if we touch Goomba, change direction
        {
            rig.velocity = new Vector2(0, rig.velocity.y);
        }

        if (rig.velocity.x == 0f) //if it's no longer going left/right - change direction
            isGoingRight = !isGoingRight;
    }

    private void OnCollisionStay2D(Collision2D collision) //Sometimes there are situations when collision with Tiles is constant
    {
        if (collision.gameObject.tag == "Tiles" && rig.velocity.x == 0f) //if it's no longer going left/right - change direction
            isGoingRight = !isGoingRight;
    }
}
