using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaybackControls : MonoBehaviour
{
    public GameManager gameManager;
    public Button exit;
    public Button pause;
    public Button next;
    public Button prev;
    public Text speedScale;

    float lastTimeScale;
    float scaledTimeFactor;
    const int maxTimeScaleFactor = 4;
    const int stepTimeScaleFactor = 2;

    void Start()
    {
        lastTimeScale = Time.timeScale;
        scaledTimeFactor = 1;

        if (gameManager == null)
        {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        }
    }

    void Update()
    {
        if (speedScale)
        {
            string speedScaleText = (Time.timeScale == 1 || Time.timeScale == 0) ? "" : Time.timeScale.ToString() + "x";

            speedScale.text = speedScaleText;
        }
    }

    public void OnExit()
    {
        ResetTimeScale();
        if (gameManager) gameManager.ReSetup();
    }

    public void OnPause()
    {
        Text pauseText = pause.GetComponentInChildren<Text>();
        
        if (Time.timeScale <= Mathf.Epsilon)
        {
            Time.timeScale = lastTimeScale;
            lastTimeScale = 0;

            if (pauseText)
            {
                pauseText.text = "Pause";
            }
        }
        else
        {
            // To pause
            lastTimeScale = Time.timeScale;
            Time.timeScale = 0;

            if (pauseText)
            {
                pauseText.text = "Resume";
            }
        }
    }

    public void OnTimeScale(bool speedUp)
    {
        if (Time.timeScale <= Mathf.Epsilon)
        {
            Time.timeScale = 1;
        }

        // scaledTimeFactor += speedUp ? stepTimeScaleFactor : -stepTimeScaleFactor;
        // scaledTimeFactor = Mathf.Clamp(scaledTimeFactor, -maxTimeScaleFactor, maxTimeScaleFactor);

        //Time.timeScale *= speedUp ? stepTimeScaleFactor : 1f / stepTimeScaleFactor;


        scaledTimeFactor *= speedUp ? stepTimeScaleFactor : 1f / stepTimeScaleFactor;
        scaledTimeFactor = Mathf.Clamp(scaledTimeFactor, 1f / maxTimeScaleFactor, maxTimeScaleFactor);
        
        Time.timeScale = scaledTimeFactor;
    }

    public void OnNext()
    {
        OnTimeScale(true);

        Text pauseText = pause.GetComponentInChildren<Text>();
        if (pauseText)
        {
            pauseText.text = "Pause";
        }
    }

    public void OnPrev()
    {
        OnTimeScale(false);

        Text pauseText = pause.GetComponentInChildren<Text>();
        if (pauseText)
        {
            pauseText.text = "Pause";
        }
    }

    public void ToggleButtons(bool enable)
    {
        exit.interactable = enable;
        pause.interactable = enable;
        prev.interactable = enable;
        next.interactable = enable;
    }

    public void ResetTimeScale()
    {
        Time.timeScale = 1;
        scaledTimeFactor = 1;

        Text pauseText = pause.GetComponentInChildren<Text>();
        if (pauseText)
        {
            pauseText.text = "Pause";
        }
    }
}
