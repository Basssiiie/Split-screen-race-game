using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreEntry : MonoBehaviour 
{
    public Text nameText;
    public Text scoreText;

    public float timeScore;

    public void SetScore(string name, float time)
    {
        timeScore = time;
        
        nameText.text = name;

        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        scoreText.text = (minutes + ":" + seconds.ToString("00"));
    }
}
