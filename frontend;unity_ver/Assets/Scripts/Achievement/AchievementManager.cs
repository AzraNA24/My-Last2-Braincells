using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class AchievementManager : MonoBehaviour
{
    [SerializeField] private Transform achievementContainer;
    [SerializeField] private GameObject achievementPrefab;
    [SerializeField] private Color unlockedColor = Color.white;
    [SerializeField] private Color lockedColor = Color.gray;

    private string userId;

    private void Start()
    {
        userId = PlayerPrefs.GetString("userId");
        Debug.Log("Attempting to load achievements for user: " + userId);
        if (!string.IsNullOrEmpty(userId))
        {
            StartCoroutine(LoadAchievements());
        }
        else
        {
            Debug.LogError("User ID not found. Please login first.");
        }
    }

    IEnumerator LoadAchievements()
    {
        string allAchievementsUrl = "https://my-last2-braincells-backend-production-c9ac.up.railway.app/api/achievements/";
        string userAchievementsUrl = $"https://my-last2-braincells-backend-production-c9ac.up.railway.app/api/achievements/user/{userId}";

        // Get all available achievements
        Debug.Log("Loading all achievements from: " + allAchievementsUrl);
        using (UnityWebRequest allAchievementsRequest = UnityWebRequest.Get(allAchievementsUrl))
        {
            yield return allAchievementsRequest.SendWebRequest();
            Debug.Log("Response Text: " + allAchievementsRequest.downloadHandler.text);

            if (allAchievementsRequest.result == UnityWebRequest.Result.Success)
            {
                AchievementResponse achievementResponse = JsonUtility.FromJson<AchievementResponse>(allAchievementsRequest.downloadHandler.text);

                // Get user's unlocked achievements
                using (UnityWebRequest userAchievementsRequest = UnityWebRequest.Get(userAchievementsUrl))
                {
                    yield return userAchievementsRequest.SendWebRequest();
                    Debug.Log("Response Text: " + userAchievementsRequest.downloadHandler.text);

                    if (userAchievementsRequest.result == UnityWebRequest.Result.Success)
                    {
                        UserAchievementResponse userAchievementResponse = JsonUtility.FromJson<UserAchievementResponse>(userAchievementsRequest.downloadHandler.text);
                        DisplayAchievements(achievementResponse.payload, userAchievementResponse.payload);
                    }
                    else
                    {
                        Debug.LogError("Failed to load achievements: " + allAchievementsRequest.error);
                        Debug.LogError("Full error response: " + allAchievementsRequest.downloadHandler.text);
                    }
                }
            }
            else
            {
                Debug.LogError("Failed to load achievements: " + allAchievementsRequest.error);
            }
        }
    }

    void DisplayAchievements(Achievement[] allAchievements, UserAchievement[] userAchievements)
    {
        // Null checks
        if (achievementContainer == null)
        {
            Debug.LogError("Achievement Container not assigned in inspector!");
            return;
        }

        if (achievementPrefab == null)
        {
            Debug.LogError("Achievement Prefab not assigned in inspector!");
            return;
        }

        if (allAchievements == null)
        {
            Debug.LogError("Received null achievements array");
            return;
        }

        // Clear existing items
        foreach (Transform child in achievementContainer)
        {
            Destroy(child.gameObject);
        }

        List<int> unlockedIds = new List<int>();
        if (userAchievements != null)
        {
            foreach (var ua in userAchievements)
            {
                if (ua?.achievement != null)
                    unlockedIds.Add(ua.achievement.id);
            }
        }

        achievementContainer.gameObject.SetActive(true);
        // Instantiate achievements
        foreach (var achievement in allAchievements)
        {
            if (achievement == null) continue;
            
            try 
            {
                GameObject obj = Instantiate(achievementPrefab, achievementContainer);
                Debug.Log($"Instantiated achievement: {achievement.name}", obj);
                AchievementItem item = obj.GetComponent<AchievementItem>();
                LayoutRebuilder.ForceRebuildLayoutImmediate(achievementContainer.GetComponent<RectTransform>());
                
                if (item != null)
                {
                    bool isUnlocked = unlockedIds.Contains(achievement.id);
                    item.Setup(achievement, isUnlocked, unlockedColor, lockedColor);
                }
                else
                {
                    Debug.LogError("Prefab missing AchievementItem component");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error instantiating achievement: {e.Message}");
            }
        }
    }
}