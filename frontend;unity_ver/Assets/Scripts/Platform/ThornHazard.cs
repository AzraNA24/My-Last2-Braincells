using UnityEngine;
using System.Collections;

public class ThornHazard : MonoBehaviour
{
    [Header("Settings")]
    public float blinkDuration = 1f;
    public int blinkCount = 3;
    public Color blinkColor = Color.red;
    
    [Header("References")]
    public Transform possessSpawnPoint;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        PossessedController possessed = other.GetComponent<PossessedController>();
        if (possessed != null)
        {
            StartCoroutine(BlinkEffect(possessed));

            ResetAllSystems();
            possessed.ResetToInitialPosition();
            
            // Return possess to spawn point
            PossessController possess = FindObjectOfType<PossessController>();
            if (possess != null && possessSpawnPoint != null)
            {
                possess.transform.position = possessSpawnPoint.position;
                possess.StopPossessing();
                possess.BindToNewPossessed(null);
            }
        }
    }

    IEnumerator BlinkEffect(PossessedController possessed)
    {
        SpriteRenderer sprite = possessed.GetComponent<SpriteRenderer>();
        Color originalColor = sprite.color;
        
        for (int i = 0; i < blinkCount; i++)
        {
            sprite.color = blinkColor;
            yield return new WaitForSeconds(blinkDuration / (blinkCount * 2));
            sprite.color = originalColor;
            yield return new WaitForSeconds(blinkDuration / (blinkCount * 2));
        }
    }

    void ResetAllSystems()
    {
        GameTimer gameTimer = FindObjectOfType<GameTimer>();
        if (gameTimer != null)
        {
            gameTimer.ResetTimer();
        }
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.ResetCollectibles();
        };
    }
}