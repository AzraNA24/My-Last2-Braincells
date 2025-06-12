using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Events; // Added for UnityEvent

public class TimelineEndEvent : MonoBehaviour 
{
    public PlayableDirector director;
    public string nextSceneName; // Added to specify scene to load
    public UnityEvent onTimelineEnd; // Event kustom

    void OnEnable() 
    {
        director.stopped += OnTimelineEnd; // Daftarkan event
    }

    void OnDisable() 
    {
        director.stopped -= OnTimelineEnd;
    }

    private void OnTimelineEnd(PlayableDirector director) 
    {
        // First invoke any custom events
        onTimelineEnd.Invoke();
        
        // Then load the scene
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}