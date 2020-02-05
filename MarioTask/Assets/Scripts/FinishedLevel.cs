﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishedLevel : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (GameObject.FindWithTag("Player").transform.position.x > 115)
        {
            anim.SetBool("IsLevelFinished", true);
        }
    }

    void MakePlayerInvisible()
    {
        GameObject.FindWithTag("InvisiblePlayer").GetComponent<SpriteRenderer>().enabled = false;
    }
} // class