using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;
using TMPro;
public class AuthManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private GameObject loginPanel;
    // [SerializeField] private Text feedbackText;
    [SerializeField] private Button startButton;
    [SerializeField] private Button achievementButton;

    private SceneController sceneController;

    // URL Backend
    private const string RegisterUrl = "https://my-last2-braincells-backend-production-c9ac.up.railway.app/api/users/register";
    private const string LoginUrl = "https://my-last2-braincells-backend-production-c9ac.up.railway.app/api/users/login";

    private void Start()
    {
        Debug.Log("Stored UserId: " + PlayerPrefs.GetString("userId"));
        Debug.Log("Stored Token: " + PlayerPrefs.GetString("authToken"));

        sceneController = FindObjectOfType<SceneController>();

        UpdateButtonInteractability();
        if (PlayerPrefs.HasKey("authToken"))
        {
            loginPanel.SetActive(false);
        }
    }
    public void OnRegisterButtonClicked()
    {
        StartCoroutine(RegisterAndLogin());
    }

    IEnumerator RegisterAndLogin()
    {
        // 1. Register
        WWWForm form = new WWWForm();
        form.AddField("username", usernameInput.text);
        form.AddField("email", emailInput.text);
        form.AddField("password", passwordInput.text);

        using (UnityWebRequest request = UnityWebRequest.Post(RegisterUrl, form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Register berhasil! Mencoba login...");
                Debug.Log("Register Response: " + request.downloadHandler.text);
                yield return new WaitForSeconds(1);

                // 2. Auto-Login setelah register berhasil
                yield return StartCoroutine(LoginAfterRegister());
            }
            else
            {
                Debug.Log("Error: " + request.error);
            }
        }
    }

    IEnumerator LoginAfterRegister()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", usernameInput.text);
        form.AddField("password", passwordInput.text);

        using (UnityWebRequest request = UnityWebRequest.Post(LoginUrl, form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                Debug.Log("Login Response: " + jsonResponse);
                Debug.Log("Login berhasil!");
                try
                {
                    var response = JsonUtility.FromJson<LoginResponse>(jsonResponse);
                    
                    if (response != null && response.user != null)
                    {
                        PlayerPrefs.SetString("authToken", response.token);
                        PlayerPrefs.SetString("userId", response.user.id.ToString());
                        Debug.Log("Login success! UserId: " + response.user.id);
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError("JSON Parse Error: " + e.Message);
                }

                PlayerPrefs.Save();
                loginPanel.SetActive(false);
                UpdateButtonInteractability();

                if (sceneController != null)
                {
                    sceneController.OnLoginSuccess();
                }
            }
            else
            {
                Debug.Log("Login gagal: " + request.error);
            }
        }
    }

    
    private void UpdateButtonInteractability()
    {
        bool isLoggedIn = PlayerPrefs.HasKey("authToken");
        startButton.interactable = isLoggedIn;
        achievementButton.interactable = isLoggedIn;
    }

    [System.Serializable]
    public class AlternativeLoginResponse
    {
        public bool success;
        public string message;
        public LoginPayload payload;
    }

    [System.Serializable]
    public class LoginPayload
    {
        public string token;
        public User user;
    }

    [System.Serializable]
    public class LoginResponse
    {
        public string token;
        public User user;
    }

    [System.Serializable]
    public class User
    {
        public int id;
        public string username;
        public string email;
    }
}