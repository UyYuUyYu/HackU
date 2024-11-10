using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [SerializeField]
    private Text scoreText;

    private int score;

    private void Start()
    {
        UpdateScoreText();
    }

    public int ScoreUp(int points)
    {
        score += points;
        UpdateScoreText();
        Debug.Log("Score increased: " + score);
        return points;
    }

    public int ScoreDown(int points)
    {
        score -= points;
        if (score < 0) score = 0;
        UpdateScoreText();
        Debug.Log("Score decreased: " + score);
        return -points;
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    public int GetScore()
    {
        return score;
    }
}

