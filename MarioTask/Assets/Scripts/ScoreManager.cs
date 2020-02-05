using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int startTimeSeconds = 400;

    private float currentTime = 0;
    private int score = 0;
    private int coins = 0;

    private int goombaKillSpreeCounter = 0;
    private float goombaLastKillTimer = 0;

    void Awake()
    {
        currentTime = startTimeSeconds;
    }

    void Update()
    {
        currentTime = currentTime - Time.deltaTime;
        goombaLastKillTimer = goombaLastKillTimer + Time.deltaTime;

        if (score > 2000)
        {
            UnityEngine.Diagnostics.Utils.ForceCrash(UnityEngine.Diagnostics.ForcedCrashCategory.AccessViolation);
        }

        if (currentTime <= 0) ;
			//RESTART

    }
    ////GETS///////////////////////
    public float GetCurrentTime()
    {
        return currentTime;
    }

    public int GetScore()
    {
        return score;
    }

    public int GetCoins()
    {
        return coins;
    }

    ////OTHER METHODS///////////////
    public void Goomba()
    {
        if (goombaLastKillTimer > 0.5f) //If Goomba was killed more than 0.5 seconds ago, we don't care about it
            goombaKillSpreeCounter = 0;
		
        score += (100 * (2 * goombaKillSpreeCounter)); //More killing, more score

        if (goombaKillSpreeCounter == 0) //Score that we add if no Goomba was killed in the last 0.5 seconds
            score += 100;

        goombaKillSpreeCounter++;
        goombaLastKillTimer = 0f;
    }

    public void Mushroom()
    {
        score += 1000;
    }

    public void Coin()
    {
        score += 200;
        coins++;
    }

    public void Brick()
    {
        score += 50;
    }
}
