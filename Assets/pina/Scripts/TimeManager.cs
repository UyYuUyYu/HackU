using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    [SerializeField]
    private float timeLimit = 300f;

    [SerializeField]
    private Text timerText;

    private float currentTime;

    private ScoreManager scoreManager;

    private void Start()
    {
        scoreManager=this.GetComponent<ScoreManager>();
        currentTime = timeLimit;
        UpdateTimerText();
        Debug.Log("Time Left: " + FormatTime(currentTime));
    }

    private void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerText();

            if (currentTime <= 0)
            {
                currentTime = 0;
                TimeOut();
            }
        }
    }

    private void TimeOut()
    {
        SendScorePUN.myScore=scoreManager.GetScore();
        Debug.Log("Time's up!");

    }

    private void UpdateTimerText()
    {
        if (timerText != null)
        {
            timerText.text = FormatTime(currentTime);
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return string.Format("Time: {0:0}:{1:00}", minutes, seconds);
    }
}

