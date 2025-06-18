using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SaveCustomizationButton : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(SaveCustomization);
    }

    private void SaveCustomization()
    {
        var manager = FindObjectOfType<CustomizationManager>();
        if (manager != null)
        {
            manager.SaveCustomizations();
        }
        else
        {
            Debug.LogError("CustomizationManager not found in scene!");
        }
    }
}