using UnityEngine;
using UnityEngine.UI;

public class ColorToggle : MonoBehaviour
{
    [Header("Color Settings")]
    public Color color1 = Color.red;
    public Color color2 = Color.blue;
    private bool isColor1 = true;

    [Header("Display Settings")]
    public bool useCameraBackground = true;
    public Image uiOverlay;
    [Range(0, 1)] public float overlayOpacity = 0.5f;

    private void Start()
    {
        ApplyCurrentColor();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleColor();
        }
    }

    public void ToggleColor()
    {
        isColor1 = !isColor1;
        ApplyCurrentColor();
    }

    private void ApplyCurrentColor()
    {
        Color targetColor = isColor1 ? color1 : color2;

        if (useCameraBackground)
        {
            Camera.main.backgroundColor = targetColor;
        }
        else if (uiOverlay != null)
        {
            targetColor.a = overlayOpacity; 
            uiOverlay.color = targetColor;
        }
    }
}