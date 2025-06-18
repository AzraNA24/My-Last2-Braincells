using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeButton : MonoBehaviour
{
    public string targetSceneName;

    public void ChangeScene()
    {
        SceneManager.LoadScene(targetSceneName);
    }
}