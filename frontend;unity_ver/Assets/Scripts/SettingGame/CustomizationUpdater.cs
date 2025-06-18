using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class CustomizationUpdater : MonoBehaviour
{
    public CharacterCustomization characterCustomization;
    public Animator characterAnimator;

    private int currentCustomTypeId;
    private int currentSelectedOption;

    public void UpdateCustomization(int customTypeId, int selectedOption)
    {
        // Calculate customItemId based on the formula: (CustomTypeId-1)*5 + (selectedOption +1)
        int customItemId = (customTypeId - 1) * 5 + (selectedOption + 1);
        string userId = PlayerPrefs.GetString("userId");
        StartCoroutine(SendUpdateRequest(userId, customItemId));

        currentCustomTypeId = customTypeId;
        currentSelectedOption = selectedOption;
    }

    public IEnumerator SendUpdateRequest(string userId, int customItemId)
    {
        string url = $"https://my-last2-braincells-backend-production-c9ac.up.railway.app/api/customizations/update/{userId}?customItemId={customItemId}";
        
        using (UnityWebRequest request = UnityWebRequest.Put(url, ""))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Customization updated successfully");
                Debug.Log("Response: " + request.downloadHandler.text);

                CustomizationResponse response = JsonUtility.FromJson<CustomizationResponse>(request.downloadHandler.text);
                Debug.Log(" " + response);
                ApplyCustomization(currentCustomTypeId, currentSelectedOption);
            }
            else
            {
                Debug.LogError("Error updating customization: " + request.error);
                Debug.LogError("Response: " + request.downloadHandler.text);
            }
        }
    }

    public void ApplyCustomization(int customTypeId, int selectedOption)
    {
        if (characterCustomization == null || characterAnimator == null)
        {
            Debug.LogError("CharacterCustomization or Animator not set!");
            return;
        }

        foreach (var type in characterCustomization.customizationTypes)
        {
            if (type.typeId == customTypeId)
            {
                if (selectedOption >= 0 && selectedOption < type.options.Length)
                {
                    characterAnimator.runtimeAnimatorController = type.options[selectedOption].animatorController;
                    PlayerPrefs.SetInt($"CustomType_{customTypeId}", selectedOption);
                    return;
                }
            }
        }
        
        Debug.LogError($"Customization type {customTypeId} or option {selectedOption} not found!");
    }
}

[System.Serializable]
public class CustomizationResponse
{
    public bool success;
    public string message;
    public CustomizationPayload payload;
}

[System.Serializable]
public class CustomizationPayload
{
    public int id;
    public User user;
    public Character character;
    public CustomType customType;
    public CustomItem customItem;
}

[System.Serializable]
public class CustomType
{
    public int id;
    public string customType;
    public Character character;
}

[System.Serializable]
public class CustomItem
{
    public int id;
    public CustomType customType;
    public string name;
    public string imagePath;
}