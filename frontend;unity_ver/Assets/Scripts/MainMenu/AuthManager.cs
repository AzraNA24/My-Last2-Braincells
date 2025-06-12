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
    [SerializeField] private Text feedbackText;

    // URL Backend
    private const string RegisterUrl = "https://my-last2-braincells-backend-production-c9ac.up.railway.app/api/users/register";
    private const string LoginUrl = "https://my-last2-braincells-backend-production-c9ac.up.railway.app/api/users/login";

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
                Debug.Log("Login berhasil!");
                // Simpan token atau sesi di PlayerPrefs
                PlayerPrefs.SetString("authToken", request.downloadHandler.text);
                loginPanel.SetActive(false);
            }
            else
            {
                Debug.Log("Login gagal: " + request.error);
            }
        }
    }
}