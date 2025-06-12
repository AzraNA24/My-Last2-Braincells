using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button achievementButton;
    [SerializeField] private string logSceneName = "LogScene";
    [SerializeField] private string achievementSceneName = "AchievementScene";
    [SerializeField] private GameObject loginPanel;

    private AuthManager authManager;

    private void Start()
    {
        authManager = FindObjectOfType<AuthManager>();
        
        // Setup button listeners
        startButton.onClick.AddListener(OnStartButtonClicked);
        achievementButton.onClick.AddListener(OnAchievementButtonClicked);
        
        // Update button state based on login status
        UpdateButtonInteractability();
    }

    private void UpdateButtonInteractability()
    {
        bool isLoggedIn = PlayerPrefs.HasKey("authToken");
        startButton.interactable = isLoggedIn;
        achievementButton.interactable = isLoggedIn;
        
        // Optional: Visual feedback for disabled buttons
        if (!isLoggedIn)
        {
            startButton.GetComponent<Image>().color = Color.gray;
            achievementButton.GetComponent<Image>().color = Color.gray;
        }
        else
        {
            startButton.GetComponent<Image>().color = Color.white;
            achievementButton.GetComponent<Image>().color = Color.white;
        }
    }

    private void OnStartButtonClicked()
    {
        if (PlayerPrefs.HasKey("authToken"))
        {
            SceneManager.LoadScene(logSceneName);
        }
        else
        {
            ShowLoginPanel();
        }
    }

    private void OnAchievementButtonClicked()
    {
        if (PlayerPrefs.HasKey("authToken"))
        {
            SceneManager.LoadScene(achievementSceneName);
        }
        else
        {
            ShowLoginPanel();
        }
    } 

    private void ShowLoginPanel()
    {
        if (loginPanel != null)
        {
            loginPanel.SetActive(true);
        }
    }

    // Call this when login is successful
    public void OnLoginSuccess()
    {
        UpdateButtonInteractability();
    }
}