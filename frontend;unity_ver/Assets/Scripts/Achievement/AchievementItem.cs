using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementItem : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;

    public void Setup(Achievement achievement, bool isUnlocked, Color unlockedColor, Color lockedColor)
    {
        titleText.text = achievement.name;
        descriptionText.text = achievement.description;
        
        // Set color based on unlocked status
        titleText.color = isUnlocked ? unlockedColor : lockedColor;
        descriptionText.color = isUnlocked ? unlockedColor : lockedColor;
    }
}