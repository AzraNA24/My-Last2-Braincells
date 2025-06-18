using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class FinishPoint : MonoBehaviour
{
    [Header("UI References")]
    public GameObject completionPanel;
    public TextMeshProUGUI collectedText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI bestTimeText;
    public TextMeshProUGUI attemptsText;

    private string baseUrl = "https://my-last2-braincells-backend-production-c9ac.up.railway.app/api";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            int saveId = PlayerPrefs.GetInt("saveId", 1);
            string characterId = "2";
            int levelNumber = PlayerPrefs.GetInt("currentNumber", -1);

            if (saveId <= 0 || string.IsNullOrEmpty(characterId) || levelNumber <= 0)
            {
                Debug.LogError("Missing required player data! saveId: " + saveId + 
                            ", characterId: " + characterId + 
                            ", levelNumber: " + levelNumber);
                return;
            }

            GameTimer timer = FindObjectOfType<GameTimer>();
            GameManager gameManager = FindObjectOfType<GameManager>();

            timer.StopTimer();
            float currentTime = timer.GetCurrentTime();
            int collectedItems = gameManager.GetCollectibleItemsCount();
            
            ShowCompletionScreen(currentTime, collectedItems);
            
            StartCoroutine(UpdateProgress(
                saveId, 
                levelNumber, 
                characterId, 
                collectedItems, 
                FormatTime(currentTime),
                true
            ));
        }
    }

    private void ShowCompletionScreen(float currentTime, int collectedItems)
    {
        if (completionPanel == null) return;

        collectedText.text = $"Collected: {collectedItems}";
        timeText.text = $"Time: {FormatTime(currentTime)}";
        bestTimeText.text = $"Best Time: {FormatTime(currentTime)}";
        attemptsText.text = $"Attempts: Loading...";

        completionPanel.SetActive(true);
        
        int saveId = PlayerPrefs.GetInt("saveId");
        int levelNumber = PlayerPrefs.GetInt("currentNumber");
        StartCoroutine(FetchAndUpdateProgress(saveId, levelNumber, currentTime, collectedItems));
    }

    private IEnumerator FetchAndUpdateProgress(int saveId, int levelNumber, float currentTime, int collectedItems)
    {
        string url = $"{baseUrl}/progress/save/{saveId}";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();
            
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                ProgressResponse response = JsonUtility.FromJson<ProgressResponse>(webRequest.downloadHandler.text);
                foreach (var progress in response.payload)
                {
                    if (progress.level.level == levelNumber)
                    {
                        // Update best time if available
                        if (!string.IsNullOrEmpty(progress.bestTime))
                        {
                            bestTimeText.text = $"Best Time: {progress.bestTime}";
                            
                            float apiBestTime = ParseTimeString(progress.bestTime);
                            if (currentTime < apiBestTime)
                            {
                                bestTimeText.text = $"Best Time: {FormatTime(currentTime)} (New Record!)";
                            }
                        }
                        
                        attemptsText.text = $"Attempts: {progress.attempts + 1}";
                        break;
                    }
                }
            }
            else
            {
                Debug.LogError($"Error fetching progress: {webRequest.error}");
                attemptsText.text = $"Attempts: 1";
            }
        }
    }
    
    private IEnumerator UpdateProgress(
        int saveId, 
        int levelNumber, 
        string characterId, 
        int collectedEarned, 
        string time, 
        bool isCompleted
    )
    {
        string url = $"{baseUrl}/progress/update?" +
                $"saveId={UnityWebRequest.EscapeURL(saveId.ToString())}" +
                $"&levelNumber={UnityWebRequest.EscapeURL(levelNumber.ToString())}" +
                $"&characterId={UnityWebRequest.EscapeURL(characterId)}" +
                $"&collectEarned={UnityWebRequest.EscapeURL(collectedEarned.ToString())}" +
                $"&bestTime={UnityWebRequest.EscapeURL(time)}" +
                $"&isCompleted={isCompleted.ToString().ToLower()}";
        
        using (UnityWebRequest webRequest = UnityWebRequest.PostWwwForm(url, ""))
        {
            webRequest.method = "POST";
            webRequest.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            yield return webRequest.SendWebRequest();
            
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error updating progress: {webRequest.error}");
                Debug.LogError($"Response: {webRequest.downloadHandler.text}");
                Debug.LogError($"URL: {url}");
            }
            else
            {
                Debug.Log("Progress updated successfully!");
            }
        }
    }
    
    private string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60);
        int remainingSeconds = Mathf.FloorToInt(seconds % 60);
        return $"{minutes:00}:{remainingSeconds:00}";
    }
    
    private float ParseTimeString(string timeString)
    {
        string[] parts = timeString.Split(':');
        if (parts.Length == 2 && 
            int.TryParse(parts[0], out int minutes) && 
            int.TryParse(parts[1], out int seconds))
        {
            return minutes * 60 + seconds;
        }
        return float.MaxValue;
    }
}

[System.Serializable]
public class ProgressResponse
{
    public bool success;
    public string message;
    public List<ProgressData> payload;
}

[System.Serializable]
public class LevelData
{
    public int level;
}