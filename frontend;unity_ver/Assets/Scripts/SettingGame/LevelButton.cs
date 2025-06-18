using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButton : MonoBehaviour
{
    public TMP_Text levelNumberText;
    public TMP_Text levelTitleText;
    public Image buttonImage;

    public void Setup(int levelNumber, string title, Color color, bool interactable)
    {
        levelNumberText.text = $"{levelNumber}";
        levelTitleText.text = title;
        buttonImage.color = color;
        GetComponent<Button>().interactable = interactable;
    }
}