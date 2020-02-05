using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionBlock : MonoBehaviour
{
    public int timesToBeHit = 1;
    public GameObject prefabToAppear;
    public bool isSecret;

    private Animator anim;

    public bool isHidden = true;

    private void Awake()
    {
        anim = GetComponentInParent<Animator>();
        if (isSecret) //if it's a secret Question block
            anim.SetBool("IsSecret", true);

        if (GameObject.FindWithTag("InvisibleBox")) //if it's a invisible Question block
            GameObject.FindWithTag("InvisibleBox").GetComponent<SpriteRenderer>().enabled = false;
    }

    private void Update() // for checking prayer movement
    {
        if (FindObjectOfType<PlayerController>().transform.position.y < -1) // find slope is increasing or decreasing 
        {
            if (FindObjectOfType<PlayerController>().transform.position.y > -1.2)// jumping from ground
            {
                GameObject.FindWithTag("InvisibleBox").transform.localScale = new Vector3(1, 1, 1);// hidden box scale & make it visible
            }
        }

        if (GameObject.FindWithTag("InvisibleBox").GetComponent<BoxCollider2D>().isTrigger)// after hitting no need to hide invisible box
        {
            // hide it
            if (FindObjectOfType<PlayerController>().transform.position.y > 0.5)
            {
                GameObject.FindWithTag("InvisibleBox").transform.localScale = new Vector3(0, 0, 0);// hidden box scale & make it invisible
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (timesToBeHit > 0)
        {
            if (collision.gameObject.tag == "Player" && IsPlayerBelow(collision.gameObject))
            {
                collision.gameObject.GetComponent<PlayerController>().isJumping = false; //Mario can't jump higher
                Instantiate(prefabToAppear, transform.parent.transform.position,
                    Quaternion.identity); //instantiate other obj
                timesToBeHit--;
                anim.SetTrigger("GotHit"); //hit animation

                if (collision.otherCollider.tag == ("InvisibleBox"))// if player touch invisible box
                {
                    GameObject.FindWithTag("InvisibleBox").GetComponent<SpriteRenderer>().enabled = true;// make it visible
                    GameObject.FindWithTag("InvisibleBox").GetComponent<BoxCollider2D>().isTrigger = false;// dont need to hide anymore
                }
            }
        }

        if (timesToBeHit == 0)
        {
            anim.SetBool("EmptyBlock", true); //change sprite in animator
        }
    }

    private bool IsPlayerBelow(GameObject go)
    {
        if ((go.transform.position.y + 1.4f < this.transform.position.y)) //if Mario is powered-up
            return true;
        if ((go.transform.position.y + 0.4f < this.transform.position.y) &&
            !go.transform.GetComponent<PlayerController>().poweredUp)
            return true;
        return false;
    }
}// class