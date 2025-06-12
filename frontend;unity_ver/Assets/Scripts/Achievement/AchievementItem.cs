using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementItem : MonoBehaviour
{
    [SerializeField] private Image achievementImage;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI unlocked_at;
    [SerializeField] private GameObject lockedOverlay;

    public void Setup( string title, string description, string when, bool unlocked)
    {
        titleText.text = title;
        descriptionText.text = description;
        unlocked_at.text = when;
        
        // Set locked state
        lockedOverlay.SetActive(!unlocked);
        
        // Set color
        achievementImage.color = unlocked ? Color.white : Color.gray;
    }
}