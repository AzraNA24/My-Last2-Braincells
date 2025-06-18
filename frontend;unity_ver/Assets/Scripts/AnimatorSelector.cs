using UnityEngine;

public class AnimatorSelector : MonoBehaviour
{
    [Header("Animator Options")]
    public Animator[] animators;
    
    private void Start()
    {
        Debug.Log("[AnimatorSelector] Starting character selection process");
        
        // Get selected character ID
        int selectedId = PlayerPrefs.GetInt("SelectedCharacter", 11);
        Debug.Log($"[AnimatorSelector] Selected character ID from PlayerPrefs: {selectedId} (default is 11)");
        
        // Validate animators array
        if (animators == null || animators.Length == 0)
        {
            Debug.LogError("[AnimatorSelector] No animators assigned in the inspector!");
            return;
        }
        else
        {
            Debug.Log($"[AnimatorSelector] Found {animators.Length} animators in array");
        }

        // Get PossessedController component
        var controller = GetComponent<PossessedController>();
        if (controller == null)
        {
            Debug.LogError("[AnimatorSelector] No PossessedController component found on this GameObject!");
            return;
        }

        bool foundActiveAnimator = false;
        
        for (int i = 0; i < animators.Length; i++)
        {
            if (animators[i] == null)
            {
                Debug.LogWarning($"[AnimatorSelector] Animator at index {i} is null!");
                continue;
            }

            bool isActive = (selectedId == 11 + i);
            Debug.Log($"[AnimatorSelector] Animator {i} (ID {11 + i}): {(isActive ? "ACTIVATING" : "deactivating")}");
            
            animators[i].gameObject.SetActive(isActive);

            if (isActive)
            {
                controller.animator = animators[i];
                foundActiveAnimator = true;
                Debug.Log($"[AnimatorSelector] Assigned animator {i} to PossessedController");
            }
        }

        if (!foundActiveAnimator)
        {
            Debug.LogWarning("[AnimatorSelector] No matching animator found for selected character ID!");
            Debug.LogWarning($"[AnimatorSelector] Valid IDs are from 11 to {11 + animators.Length - 1}");
        }
        else
        {
            Debug.Log("[AnimatorSelector] Character selection completed successfully");
        }
    }
}