using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
public class SaveData
{
    public int id;
    public User user;
    public int slot;
    public string createdAt;
}

[System.Serializable]
public class ProgressData
{
    public int id;
    public Character character;
    public SaveFile saveFile;
    public Level level;
    public int collectEarned;
    public string bestTime;
    public int attempts;
    public bool completed;
}

[System.Serializable]
public class Character
{
    public int id;
    public string name;
    public string imagePath;
}

[System.Serializable]
public class SaveFile
{
    public int id;
    public User user;
    public int slot;
    public string createdAt;
}

[System.Serializable]
public class Level
{
    public int id;
    public int level;
    public string title;
}

[System.Serializable]
public class ApiResponse
{
    public bool success;
    public string message;
    public List<SaveData> payload;
}

[System.Serializable]
public class ProgressApiResponse
{
    public bool success;
    public string message;
    public List<ProgressData> payload;
}

public class SaveSlotManager : MonoBehaviour
{
    public PlayableDirector introCutscene;
    public GameObject Camera;
    public GameObject[] slotButtons;
    public TMP_Text[] slotTexts;
    
    private string baseUrl = "https://my-last2-braincells-backend-production-c9ac.up.railway.app/api";
    private Dictionary<int, ProgressData> slotProgressData = new Dictionary<int, ProgressData>();

    void Start()
    {
        StartCoroutine(FetchSaveSlots());
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

    IEnumerator FetchSaveSlots()
    {
        string url = $"{baseUrl}/saves/user/{CurrentUserId}";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                ApiResponse response = JsonUtility.FromJson<ApiResponse>(webRequest.downloadHandler.text);
                
                // Clear previous data
                slotProgressData.Clear();
                
                // Fetch progress for each save slot
                foreach (SaveData save in response.payload)
                {
                    if (save.user.id == CurrentUserId)
                    {
                        yield return StartCoroutine(FetchSlotProgress(save.slot));
                    }
                }
                
                // Update UI for all slots
                UpdateSlotUI();
            }
            else
            {
                Debug.LogError($"Error fetching save slots: {webRequest.error}");
            }
        }
    }

    IEnumerator FetchSlotProgress(int slotNumber)
    {
        string url = $"{baseUrl}/progress/save/{CurrentUserId}";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                ProgressApiResponse response = JsonUtility.FromJson<ProgressApiResponse>(webRequest.downloadHandler.text);
                
                // Find the progress data for this specific slot
                foreach (ProgressData progress in response.payload)
                {
                    if (progress.saveFile.slot == slotNumber)
                    {
                        slotProgressData[slotNumber] = progress;
                        break;
                    }
                }
            }
            else
            {
                Debug.LogError($"Error fetching progress for slot {slotNumber}: {webRequest.error}");
            }
        }
    }

    void UpdateSlotUI()
    {
        for (int i = 0; i < slotButtons.Length; i++)
        {
            int slotNumber = i + 1; // Biar mulai dari satu
            
            if (slotProgressData.ContainsKey(slotNumber))
            {
                ProgressData progress = slotProgressData[slotNumber];
                slotTexts[i].text = $"{progress.character.name}\nLevel {progress.level.level}: {progress.level.title}";
            }
            else
            {
                slotTexts[i].text = "Empty Slot";
            }
        }
    }

    IEnumerator LoadCharacterImage(string imagePath, Image targetImage)
    {
        string fullUrl = baseUrl + imagePath;
        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(fullUrl))
        {
            yield return webRequest.SendWebRequest();
            
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);
                targetImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            }
            else
            {
                Debug.LogError($"Error loading character image: {webRequest.error}");
            }
        }
    }

    public void OnSlotClicked(int slotNumber)
    {
        if (CurrentUserId == -1)
        {
            Debug.LogError("User not logged in!");
            return;
        }

        PlayerPrefs.SetInt("slotNumber", slotNumber);
        if (slotProgressData.ContainsKey(slotNumber))
        {
            LoadCustomizeScene();
        }
        else
        {
            StartCoroutine(CreateNewSave(slotNumber));
        }
    }

    IEnumerator CreateNewSave(int slotNumber)
    {
        string url = $"{baseUrl}/saves/create?userId={UnityWebRequest.EscapeURL(CurrentUserId.ToString())}&slot={UnityWebRequest.EscapeURL(slotNumber.ToString())}";
        
        using (UnityWebRequest webRequest = UnityWebRequest.PostWwwForm(url, ""))
        {
            webRequest.method = "POST";
            yield return webRequest.SendWebRequest();

            // Handle network errors first
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Network error creating save: {webRequest.error}\nResponse: {webRequest.downloadHandler.text}");
                yield break;
            }

            // Then parse the response
            SaveCreationResponse response = null;
            try
            {
                response = JsonUtility.FromJson<SaveCreationResponse>(webRequest.downloadHandler.text);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error parsing response: {e.Message}\nResponse: {webRequest.downloadHandler.text}");
                yield break;
            }

            // Validate response
            if (response == null || !response.success || response.payload == null)
            {
                Debug.LogError($"API returned failure: {webRequest.downloadHandler.text}");
                yield break;
            }

            Debug.Log($"Save created in slot {slotNumber} with ID: {response.payload.id}");

            PlayerPrefs.SetInt("saveId", response.payload.id);
            PlayerPrefs.SetInt("slotNumber", slotNumber);
            
            // Move yield statements outside the try-catch
            yield return StartCoroutine(FetchSlotProgress(slotNumber));
            UpdateSlotUI();

            if (introCutscene != null)
            {
                Camera.SetActive(true);
                introCutscene.Play();
                yield return new WaitForSeconds((float)introCutscene.duration);
            }
            
            LoadCustomizeScene();
        }
    }

    void LoadCustomizeScene()
    {
        SceneManager.LoadScene("PlayerCostumize");
    }

    public void ClearUserData()
    {
        PlayerPrefs.DeleteKey("userId");
        PlayerPrefs.DeleteKey("authToken");
    }
}

[System.Serializable]
public class SaveCreationResponse
{
    public bool success;
    public string message;
    public SaveData payload;
}