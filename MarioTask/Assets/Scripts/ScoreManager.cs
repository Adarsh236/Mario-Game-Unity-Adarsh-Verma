using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public int startTimeSeconds = 400;

    private float currentTime = 0;
    private int score = 0;
    private int coins = 0;

    private int goombaKillSpreeCounter = 0;
    private float goombaLastKillTimer = 0;

    // Extra Variable for UI
    public Text marioText;
    public Text coinText;
    public Text worldText;
    public Text timeText;

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
        
        if (currentTime > 1 && currentTime < 1.5f) FindObjectOfType<PlayerController>().SetReset(true); //RESTART

        // setting text and decide format
        marioText.text = GetScore().ToString("0000000");
        coinText.text = "x" + GetCoins().ToString("00");
        worldText.text = "1-1";
        timeText.text = (System.Math.Round(GetCurrentTime(), 0)).ToString();
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