using UnityEngine;

public class LoginButtonController : MonoBehaviour
{
    [SerializeField] private GameObject loginPanel;
    
    void Start()
    {
        if (loginPanel != null)
            loginPanel.SetActive(false);
    }

    public void ToggleLoginPanel()
    {
        if (loginPanel != null)
        {
            bool isActive = loginPanel.activeSelf;
            loginPanel.SetActive(!isActive);
        }
    }
}