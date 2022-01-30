using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    public int gameLength = 60;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;

    /*public Image panel;
    public Color panelStartColor;
    public Color panelEndColor;
    public float panelColorChangeDuration = 1.0f;*/

    public GameObject startButton;

    [HideInInspector]
    public bool gameIsOn = false;

    //private float stepDuration;

    private float timer;

    private int score;

    /*private void Start()
    {
        stepDuration = Time.deltaTime / panelColorChangeDuration * 2;
    }*/

    private void Update()
    {
        if (gameIsOn)
        {
            timer -= Time.deltaTime;
            timerText.text = ((int)timer).ToString();

            if (timer < 0)
            {
                EndGame();
            }
        }
    }

    public void StartGame()
    {
        timer = gameLength;
        timerText.text = ((int)timer).ToString();
        score = 0;
        scoreText.text = score.ToString();
        gameIsOn = true;
        startButton.SetActive(false);
    }

    public void IncrementScore()
    {
        score++;
        scoreText.text = score.ToString();
        //AnimatePanel();
    }

    private void EndGame()
    {
        gameIsOn = false;
        timerText.text = 0.ToString();
        startButton.SetActive(true);
    }

    /*IEnumerator AnimatePanel()
    {
        for (float i = 0; i < 1; i += stepDuration)
        {
            panel.color = Color.Lerp(panelStartColor, panelEndColor, i);
            yield return new WaitForSeconds(0.1f);
        }
        for (float i = 1; i > 0; i -= stepDuration)
        {
            panel.color = Color.Lerp(panelStartColor, panelEndColor, i);
            yield return new WaitForSeconds(0.1f);
        }
    }*/
}
