using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Changer : MonoBehaviour
{
    public SpriteRenderer bodyParts;
    public List<Sprite> options = new List<Sprite>();
    public int customTypeId;
    

    private int currentOption = 0;
    private CustomizationUpdater updater;

    private void Start()
    {
        // Load saved option
        currentOption = PlayerPrefs.GetInt($"CustomType_{customTypeId}", 0);
        UpdateAppearance();

        // Get the updater component
        updater = FindObjectOfType<CustomizationUpdater>();
        if (updater == null)
        {
            GameObject obj = new GameObject("CustomizationUpdater");
            updater = obj.AddComponent<CustomizationUpdater>();
        }
    }

    public void NextOption()
    {
        currentOption = (currentOption + 1) % options.Count;
        UpdateAppearance();
    }

    public void PreviousOption()
    {
        currentOption = (currentOption - 1 + options.Count) % options.Count;
        UpdateAppearance();
    }

    private void UpdateAppearance()
    {
        if (bodyParts != null && options.Count > 0)
        {
            bodyParts.sprite = options[currentOption];

            // Save the option
            PlayerPrefs.SetInt($"CustomType_{customTypeId}", currentOption);

            // Update the customization on the server
            if (updater != null)
            {
                updater.UpdateCustomization(customTypeId, currentOption);
            }

            var manager = FindObjectOfType<CustomizationManager>();
            if (manager != null)
            {
                manager.SetCustomization(customTypeId, currentOption);
            }
        }
    }
    public void ApplyCustomization()
    {
        if (updater == null)
        {
            updater = FindObjectOfType<CustomizationUpdater>();
            if (updater == null)
            {
                GameObject obj = new GameObject("CustomizationUpdater");
                updater = obj.AddComponent<CustomizationUpdater>();
            }
        }
        
        updater.UpdateCustomization(customTypeId, currentOption);
        
        PlayerPrefs.SetInt($"CustomType_{customTypeId}", currentOption);
        SceneManager.LoadScene("LevelMenu");
    }
}