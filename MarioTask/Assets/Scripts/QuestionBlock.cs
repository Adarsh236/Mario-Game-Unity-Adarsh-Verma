using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionBlock : MonoBehaviour
{
	public int timesToBeHit = 1;
    public GameObject prefabToAppear;
    public bool isSecret;

    private Animator anim;

    private void Awake()
    {
        anim = GetComponentInParent<Animator>();
        if (isSecret) //if it's a secret Question block
            anim.SetBool("IsSecret", true);

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (timesToBeHit > 0)
        {
            if (collision.gameObject.tag == "Player" && IsPlayerBelow(collision.gameObject))
            {
                collision.gameObject.GetComponent<PlayerController>().isJumping = false; //Mario can't jump higher
                Instantiate(prefabToAppear, transform.parent.transform.position, Quaternion.identity); //instantiate other obj
                timesToBeHit--;
                anim.SetTrigger("GotHit"); //hit animation
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
        if ((go.transform.position.y + 0.4f < this.transform.position.y) && !go.transform.GetComponent<PlayerController>().poweredUp)
            return true;
        return false;
    }
}
