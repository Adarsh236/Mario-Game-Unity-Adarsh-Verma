using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    public GameObject brickBreakParticles;
    public AudioClip brickBounce;
    public AudioClip brickBreak;

    private AudioSource source;
    private Animator anim;
    ScoreManager sm;

    public void DestroyBricks()
    {
        Vector3 pos = transform.position;
        source.PlayOneShot(brickBreak);
        GetComponentInParent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
        Destroy(this.gameObject,0.6f);
        Instantiate(brickBreakParticles, pos, Quaternion.Euler(-90,0,0));
    }

    void Awake()
    {
        sm = FindObjectOfType<ScoreManager>();
        source = GetComponent<AudioSource>();
        anim = GetComponentInParent<Animator>();
        anim.SetBool("EmptyBlock", true);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && IsPlayerBelow(collision.gameObject))
        {
            collision.gameObject.GetComponent<PlayerController>().isJumping = false;

            if (collision.transform.GetComponent<PlayerController>().poweredUp)
            {
                DestroyBricks();
                sm.Brick();
            }
            else
            {
                anim.SetTrigger("GotHit");
                source.PlayOneShot(brickBounce);
            }
        }
    }

    private bool IsPlayerBelow(GameObject go)
    {
        if ((go.transform.position.y + 1.4f < this.transform.position.y))
            return true;
        if ((go.transform.position.y + 0.4f < this.transform.position.y) && !go.transform.GetComponent<PlayerController>().poweredUp)
            return true;
        return false;
    }
}
