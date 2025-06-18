using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class AuthManager : MonoBehaviour
{
    // Register Fields
    [SerializeField] private TMP_InputField regUsernameInput;
    [SerializeField] private TMP_InputField regEmailInput;
    [SerializeField] private TMP_InputField regPasswordInput;
    
    // Login Fields
    [SerializeField] private TMP_InputField loginUsernameInput;
    [SerializeField] private TMP_InputField loginPasswordInput;
    
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject SignupPanel;
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
            SignupPanel.SetActive(false);
            loginPanel.SetActive(false);
        }
    }

    public void OnRegisterButtonClicked()
    {
        StartCoroutine(RegisterAndLogin());
    }

    public void OnLoginButtonClicked()
    {
        StartCoroutine(Login());
    }

    IEnumerator RegisterAndLogin()
    {
        // Register menggunakan input field register
        WWWForm form = new WWWForm();
        form.AddField("username", regUsernameInput.text);
        form.AddField("email", regEmailInput.text);
        form.AddField("password", regPasswordInput.text);

        using (UnityWebRequest request = UnityWebRequest.Post(RegisterUrl, form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Register berhasil! Mencoba login...");
                Debug.Log("Register Response: " + request.downloadHandler.text);
                
                yield return StartCoroutine(Login(regUsernameInput.text, regPasswordInput.text));
            }
            else
            {
                Debug.Log("Error: " + request.error);
            }
        }
    }

    // Overload untuk login setelah register
    IEnumerator Login(string username, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        using (UnityWebRequest request = UnityWebRequest.Post(LoginUrl, form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                ProcessLoginResponse(request.downloadHandler.text);
            }
            else
            {
                Debug.Log("Login gagal: " + request.error);
            }
        }
    }

    IEnumerator Login()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", loginUsernameInput.text);
        form.AddField("password", loginPasswordInput.text);

        using (UnityWebRequest request = UnityWebRequest.Post(LoginUrl, form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                ProcessLoginResponse(request.downloadHandler.text);
            }
            else
            {
                Debug.Log("Login gagal: " + request.error);
            }
        }
    }

    private void ProcessLoginResponse(string jsonResponse)
    {
        Debug.Log("Login Response: " + jsonResponse);
        try
        {
            var response = JsonUtility.FromJson<LoginResponse>(jsonResponse);
            
            if (response != null && response.success)
            {
                PlayerPrefs.SetString("userId", response.payload.id.ToString());
                Debug.Log("Login success! UserId: " + response.payload.id);
                
                PlayerPrefs.Save();
                loginPanel.SetActive(false);
                SignupPanel.SetActive(false);
                UpdateButtonInteractability();

                if (sceneController != null)
                {
                    sceneController.OnLoginSuccess();
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("JSON Parse Error: " + e.Message);
        }
    }
    
    private void UpdateButtonInteractability()
    {
        bool isLoggedIn = PlayerPrefs.HasKey("authToken");
        startButton.interactable = isLoggedIn;
        achievementButton.interactable = isLoggedIn;
    }

    [System.Serializable]
    public class LoginResponse
    {
        public bool success;
        public string message;
        public User payload;
    }

    [System.Serializable]
    public class User
    {
        public int id;
        public string username;
        public string email;
        public string password;
        public string createdAt;
    }
}