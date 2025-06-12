using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class AchievementManager : MonoBehaviour
{
    [Header("API Configuration")]
    [SerializeField] private string allAchievementsUrl = "https://my-last2-braincells-backend-production-c9ac.up.railway.app/api/achievements/";
    [SerializeField] private float apiTimeout = 10f;
    [SerializeField] private int maxRetries = 3;

    [Header("UI Elements")]
    [SerializeField] private GameObject achievementPrefab;
    [SerializeField] private Transform achievementContainer;
    [SerializeField] private Sprite lockedSprite;

    private List<Achievement> allAchievements = new List<Achievement>();
    private int currentRetryCount = 0;

    private void Start()
    {
        StartCoroutine(LoadAchievementsWithRetry());
    }

    private IEnumerator LoadAchievementsWithRetry()
    {
        while (currentRetryCount < maxRetries)
        {
            yield return StartCoroutine(LoadAllAchievements());
            
            if (allAchievements.Count > 0)
            {
                // Success, break the retry loop
                break;
            }
            
            currentRetryCount++;
            Debug.LogWarning($"Retry attempt {currentRetryCount}/{maxRetries}");
            yield return new WaitForSeconds(2f); // Wait before retry
        }
    }

    private IEnumerator LoadAllAchievements()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(allAchievementsUrl))
        {
            request.timeout = (int)apiTimeout;
            
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                Debug.Log("Achievements API Response: " + jsonResponse);
                
                try
                {
                    ApiResponse<Achievement> response = JsonUtility.FromJson<ApiResponse<Achievement>>(jsonResponse);
                    
                    if (response != null && response.success && response.payload != null)
                    {
                        allAchievements = new List<Achievement>(response.payload);
                        Debug.Log($"Successfully loaded {allAchievements.Count} achievements");
                        DisplayAchievements();
                    }
                    else
                    {
                        Debug.LogError("Invalid API response format");
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError("JSON parse error: " + e.Message);
                }
            }
            else
            {
                HandleNetworkError(request, "achievements");
            }
        }
    }

    private void HandleNetworkError(UnityWebRequest request, string context)
    {
        string errorMessage = $"Error fetching {context}: ";
        
        switch (request.result)
        {
            case UnityWebRequest.Result.ConnectionError:
                errorMessage += "Connection error";
                break;
            case UnityWebRequest.Result.ProtocolError:
                errorMessage += $"HTTP {request.responseCode}";
                break;
            case UnityWebRequest.Result.DataProcessingError:
                errorMessage += "Data processing error";
                break;
            default:
                errorMessage += "Unknown error";
                break;
        }
        
        errorMessage += $"\n{request.error}";
        Debug.LogError(errorMessage);
    }

    private void DisplayAchievements()
    {
        // Clear existing achievements
        foreach (Transform child in achievementContainer)
        {
            Destroy(child.gameObject);
        }

        // Create achievement items
        foreach (var achievement in allAchievements)
        {
            GameObject achievementItem = Instantiate(achievementPrefab, achievementContainer);
            AchievementItem itemScript = achievementItem.GetComponent<AchievementItem>();
            
            // Load sprite from Resources
            
            // For this simplified version, we'll just show all achievements as unlocked
            // or you can add some other logic to determine if they should be shown as locked
            bool showAsUnlocked = true; // Change this if you have different display logic
            
            itemScript.Setup(               
                achievement.name,
                achievement.description,
                showAsUnlocked ? "Unlocked" : "Locked",
                showAsUnlocked
            );
        }
    }
}

[System.Serializable]
public class ApiResponse<T>
{
    public bool success;
    public string message;
    public T[] payload;
}