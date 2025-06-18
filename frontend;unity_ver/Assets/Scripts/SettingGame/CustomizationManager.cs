using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.SceneManagement;

public class CustomizationManager : MonoBehaviour
{
    private string baseUrl = "https://my-last2-braincells-backend-production-c9ac.up.railway.app/api";
    private int selectedCharacterId = -1;
    private int[] selectedOptions = new int[5];

    public void SetCharacter(int characterId)
    {
        selectedCharacterId = characterId;
        PlayerPrefs.SetInt("SelectedCharacter", characterId);
    }

    public void SetCustomization(int customTypeId, int optionIndex)
    {
        if (customTypeId > 0 && customTypeId <= selectedOptions.Length)
        {
            selectedOptions[customTypeId - 1] = optionIndex;
            PlayerPrefs.SetInt($"CustomType_{customTypeId}", optionIndex);
        }
    }

    public void SaveCustomizations()
    {
        if (selectedCharacterId == -1)
        {
            Debug.LogError("No character selected!");
            return;
        }
        StartCoroutine(SaveCustomizationsCoroutine());
    }

    IEnumerator SaveCustomizationsCoroutine()
    {
        string userId = PlayerPrefs.GetString("userId");
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("User ID not found!");
            yield break;
        }
        string checkUrl = $"{baseUrl}/customizations/user/{userId}/character/{selectedCharacterId}";
        using (UnityWebRequest checkRequest = UnityWebRequest.Get(checkUrl))
        {
            yield return checkRequest.SendWebRequest();

            if (checkRequest.result == UnityWebRequest.Result.Success)
            {
                bool hasExisting = !string.IsNullOrEmpty(checkRequest.downloadHandler.text) && 
                                checkRequest.downloadHandler.text != "[]" &&
                                checkRequest.downloadHandler.text != "{}";
                
                if (hasExisting)
                {
                    Debug.Log("Existing customizations found, updating...");
                    Debug.Log($"Response: {checkRequest.downloadHandler.text}");
                    yield return StartCoroutine(UpdateCustomizations());
                }
                else
                {
                    Debug.Log("No existing customizations, creating new...");
                    yield return StartCoroutine(CreateCustomizations());
                }
            }
            else
            {
                Debug.LogError($"Error checking customizations: {checkRequest.error}");
                Debug.Log($"Response: {checkRequest.downloadHandler.text}");
            }
        }
    }

    IEnumerator CreateCustomizations()
    {
        string userId = PlayerPrefs.GetString("userId");
        
        for (int i = 0; i < selectedOptions.Length; i++)
        {
            int customTypeId = i + 1;
            int customItemId = (customTypeId - 1) * 5 + selectedOptions[i] + 1;
            
            string url = $"{baseUrl}/customizations/create";
            
            WWWForm form = new WWWForm();
            form.AddField("userId", userId);
            form.AddField("characterId", selectedCharacterId.ToString());
            form.AddField("customTypeId", customTypeId.ToString());
            form.AddField("customItemId", customItemId.ToString());

            using (UnityWebRequest request = UnityWebRequest.Post(url, form))
            {
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Error creating customization (Type {customTypeId}): {request.error}");
                    Debug.Log($"Response: {request.downloadHandler.text}");
                }
                else
                {
                    Debug.Log($"Successfully created customization (Type {customTypeId})");
                    Debug.Log($"Response: {request.downloadHandler.text}");
                }
            }
            
            yield return new WaitForSeconds(0.1f);
        }

        PlayerPrefs.Save();
        SceneManager.LoadScene("LevelMenu");
    }

    IEnumerator UpdateCustomizations()
{
    string userId = PlayerPrefs.GetString("userId");

    // Proses setiap customType
    for (int i = 0; i < selectedOptions.Length; i++)
    {
        int customItemId = i * 5 + selectedOptions[i] + 1;
        
        string url = $"{baseUrl}/customizations/update/{userId}?customItemId={customItemId}";
        
        using (UnityWebRequest request = UnityWebRequest.Put(url, ""))
        {
            yield return request.SendWebRequest();
            
            if (request.result != UnityWebRequest.Result.Success)
                Debug.LogError($"Error updating item {customItemId}: {request.error}");
        }
    }

    PlayerPrefs.Save();
    SceneManager.LoadScene("LevelMenu");
}
}