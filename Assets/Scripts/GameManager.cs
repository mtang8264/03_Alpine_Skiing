using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool points;
    public static GameManager instance;
    public bool mayBegin;
    public int score, streak, misses;
    public TextMeshPro scoreText, streakText, timerText, missText, countdown;
    public Color countdownRed, countdownYellow, countdownGreen;
    public float timer;

    public static float bestTime;
    public static int bestMisses;

    private float startTime;

    // Start is called before the first frame update
    void Start()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;

        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(streak > 3)
        {
            MusicManager.instance.PlayLayer(0);
        }
        else
        {
            MusicManager.instance.LeaveLayer(0);
        }
        if (streak > 6)
        {
            MusicManager.instance.PlayLayer(1);
        }
        else
        {
            MusicManager.instance.LeaveLayer(1);
        }
        if (streak > 9)
        {
            MusicManager.instance.PlayLayer(2);
        }
        else
        {
            MusicManager.instance.LeaveLayer(2);
        }

        if (mayBegin)
        {
            timer += Time.deltaTime;
        }
        else
        {
            if(Time.time - startTime - 1f < 1f)
            {
                countdown.text = "3";
                countdown.color = countdownRed;
            }
            else if(Time.time - startTime - 1f < 2f)
            {
                countdown.text = "2";
                countdown.color = countdownYellow;
            }
            else if(Time.time - startTime - 1f < 3f)
            {
                countdown.text = "1";
                countdown.color = countdownGreen;
            }
            else
            {
                countdown.enabled = false;
                mayBegin = true;
            }
        }

        if (points)
        {
            string scoreTemp = "<mspace=2em>SCORE:";
            string streakTemp = "<mspace=2em>STREAK: ";

            if (streak < 10)
            {
                streakTemp += " ";
            }
            streakTemp += "" + streak;
            if (score < 10)
            {
                scoreTemp += "   ";
            }
            else if (score < 100)
            {
                scoreTemp += "  ";
            }
            else if (score < 1000)
            {
                scoreTemp += " ";
            }
            scoreTemp += "" + score;

            scoreText.text = scoreTemp;
            streakText.text = streakTemp;
        }
        else
        {
            string goingToPrint = "Time: ";
            var tempTimer = timer * 10;
            tempTimer = (int)tempTimer;
            tempTimer /= 10;
            goingToPrint += "" + tempTimer;
            timerText.text = goingToPrint;

            if(misses > 0)
            {
                missText.enabled = true;
                missText.text = "MISS: " + misses;
            }
        }
    }

    public void Score()
    {
        score++;
        streak++;
    }
    public void Miss()
    {
        misses++;
        streak = 0;
    }
    public void Finish()
    {
        Debug.Log("Finish");
        if(bestTime < Mathf.Epsilon && bestMisses == 0)
        {
            bestTime = timer;
            bestMisses = misses;
        }
        else if(misses == 0 && timer < bestTime)
        {
            bestTime = timer;
            bestMisses = 0;
        }
        else if(misses < bestMisses)
        {
            bestTime = timer;
            bestMisses = misses;
        }

        SceneManager.LoadScene(0);
    }
}
