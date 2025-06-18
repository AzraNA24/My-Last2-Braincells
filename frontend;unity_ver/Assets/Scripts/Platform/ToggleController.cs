using UnityEngine;

public class TogglePlatforms : MonoBehaviour
{
    public GameObject[] appearOnToggle;
    public GameObject[] disappearOnToggle;
    public float cooldown = 0.5f;
    
    private float lastToggle;
    private bool isToggled = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > lastToggle + cooldown)
        {
            isToggled = !isToggled;
            
            foreach (var platform in appearOnToggle)
                if (platform != null) platform.SetActive(isToggled);
                
            foreach (var platform in disappearOnToggle)
                if (platform != null) platform.SetActive(!isToggled);
                
            lastToggle = Time.time;
        }
    }
}