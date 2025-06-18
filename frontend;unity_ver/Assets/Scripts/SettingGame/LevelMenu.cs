using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;

public class LevelMenu : MonoBehaviour
{
    [System.Serializable]
    public class LevelResponse
    {
        public bool success;
        public string message;
        public List<LevelData> payload;
    }

    [System.Serializable]
    public class LevelData
    {
        public int id;
        public int level;
        public string title;
    }

    [System.Serializable]
    public class ProgressResponse
    {
        public bool success;
        public string message;
        public List<ProgressData> payload;
    }

    [System.Serializable]
    public class ProgressData
    {
        public int id;
        public Character character;
        public SaveFile saveFile;
        public LevelData level;
        public int collectEarned;
        public string bestTime;
        public int attempts;
        public bool completed;
    }

    [System.Serializable]
    public class SaveFile
    {
        public int id;
        public User user;
        public int slot;
        public string createdAt;
    }


    public GameObject levelButtonPrefab;
    public Transform levelContainer;
    public float horizontalSpacing = 600f;
    public float verticalSpacing = 200f;
    public float rowOffset = 100f;
    public Color completedColor = new Color(1f, 0.5f, 0f);
    public Color availableColor = Color.white;
    public Color lockedColor = Color.gray;

    private string baseUrl = "https://my-last2-braincells-backend-production-c9ac.up.railway.app/api";
    private List<LevelData> allLevels = new List<LevelData>();
    private Dictionary<int, ProgressData> userProgress = new Dictionary<int, ProgressData>();
    private String selectedCharacterId;
    private int CurrentSlotId;

    void Start()
    {
        selectedCharacterId = PlayerPrefs.GetString("SelectedCharacter");
        CurrentSlotId = PlayerPrefs.GetInt("slotNumber", -1);
        

        StartCoroutine(LoadLevelData());
    }

    private int CurrentUserId
    {
        get
        {
            if (PlayerPrefs.HasKey("userId"))
            {
                return int.Parse(PlayerPrefs.GetString("userId"));
            }
            Debug.LogError("User ID not found! Make sure user is logged in.");
            return -1;
        }
    }
    IEnumerator LoadLevelData()
    {
        yield return StartCoroutine(FetchAllLevels());
        yield return StartCoroutine(FetchUserProgress());
        
        CreateLevelButtons();
    }

    IEnumerator FetchAllLevels()
    {
        string url = $"{baseUrl}/levels/";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                LevelResponse response = JsonUtility.FromJson<LevelResponse>(webRequest.downloadHandler.text);
                allLevels = response.payload;
                allLevels.Sort((a, b) => a.level.CompareTo(b.level));
            }
            else
            {
                Debug.LogError($"Error fetching levels: {webRequest.error}");
            }
        }
    }

    IEnumerator FetchUserProgress()
    {
        string url = $"{baseUrl}/progress/user/{CurrentUserId}/slot/{CurrentSlotId}";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                ProgressResponse response = JsonUtility.FromJson<ProgressResponse>(webRequest.downloadHandler.text);
                foreach (var progress in response.payload)
                {
                    userProgress[progress.level.level] = progress;
                }
            }
            else
            {
                Debug.LogError($"Error fetching progress: {webRequest.error}");
            }
        }
    }

    void CreateLevelButtons()
    {
        foreach (Transform child in levelContainer)
        {
            Destroy(child.gameObject);
        }

        int highestCompletedLevel = 0;
        foreach (var progress in userProgress.Values)
        {
            if (progress.completed && progress.level.level > highestCompletedLevel)
            {
                highestCompletedLevel = progress.level.level;
            }
        }

        // Zigzag pattern
        for (int i = 0; i < allLevels.Count; i++)
        {
            int levelNumber = allLevels[i].level;
            bool isCompleted = userProgress.ContainsKey(levelNumber) && userProgress[levelNumber].completed;
            bool isAvailable = levelNumber <= highestCompletedLevel + 1;

            // Calculate position for horizontal zigzag
            int column = (levelNumber - 1) / 2;  // Two levels per column
            float yPos = ((levelNumber - 1) % 2) * verticalSpacing;
            
            // Alternate direction every column
            if (column % 2 != 0) 
            {
                yPos = verticalSpacing - yPos;
            }
            
            float xPos = column * horizontalSpacing;

            GameObject buttonObj = Instantiate(levelButtonPrefab, levelContainer);
            buttonObj.transform.localPosition = new Vector2(xPos, -yPos); // Note: negative yPos if using top-down

            LevelButton levelButton = buttonObj.GetComponent<LevelButton>();
            if (levelButton != null)
            {
                Color buttonColor = isCompleted ? completedColor : 
                    (isAvailable ? availableColor : lockedColor);
                
                levelButton.Setup(
                    levelNumber: allLevels[i].level,
                    title: allLevels[i].title,
                    color: buttonColor,
                    interactable: isAvailable || isCompleted
                );
            }

            int currentLevel = levelNumber;
            buttonObj.GetComponent<Button>().onClick.AddListener(() => OnLevelSelected(currentLevel));
        }
    }

    void OnLevelSelected(int levelNumber)
    {
        Debug.Log($"Level {levelNumber} selected");
        PlayerPrefs.SetInt("SelectedLevel", levelNumber);
        
        PlayerPrefs.SetInt("currentNumber", levelNumber);
        SceneManager.LoadScene($"Level{levelNumber}");
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}