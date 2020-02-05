using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public bool timeToDie = false;
    ScoreManager sm;

    private void Awake()
    {
        sm = FindObjectOfType<ScoreManager>();
    }

    private void Start()
    {
        sm.Coin();
    }

    void Update()
    {
        if (timeToDie)
            Destroy(this.gameObject);
    }
}
