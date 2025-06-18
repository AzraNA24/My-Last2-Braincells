using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public TMP_Text timerText;
    private float currentTime;
    private bool isRunning;

    void Start()
    {
        StartTimer();
    }
    void Update()
    {
        if (isRunning)
        {
            currentTime += Time.deltaTime;
            timerText.text = FormatTime(currentTime);
        }
    }

    public void StartTimer()
    {
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public float GetCurrentTime()
    {
        return currentTime;
    }

    public void ResetTimer()
    {
        currentTime = 0f;
        timerText.text = "00:00";
    }

    public string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60);
        int remainingSeconds = Mathf.FloorToInt(seconds % 60);
        return $"{minutes:00}:{remainingSeconds:00}";
    }
}