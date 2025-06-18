using UnityEngine;
using System.Collections;

public class ChasingEnemy : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform pointA;
    public Transform pointB;
    public float moveSpeed = 2f;
    public float chaseSpeed = 3f;
    public float detectionRadius = 5f;
    public float waypointThreshold = 0.1f;
    
    [Header("Effect Settings")]
    public float blinkDuration = 1f;
    public int blinkCount = 4;
    public Color blinkColor = Color.red;
    public Transform possessSpawnPoint;
    
    private Transform currentTarget;
    private bool isChasing = false;
    private Vector3 initialPosition;
    private SpriteRenderer spriteRenderer;
    
    private void Start()
    {
        initialPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentTarget = pointB;
    }
    
    private void Update()
    {
        if (!isChasing)
        {
            PatrolBetweenPoints();
        }
        // Check for possess in detection radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        bool possessDetected = false;
        GameObject possess = null;
        
        foreach (var hit in hits)
        {
            PossessController possessController = hit.GetComponent<PossessController>();
            if (possessController != null && possessController.IsPossessing())
            {
                possessDetected = true;
                possess = hit.gameObject;
                break;
            }
        }
        
        if (possessDetected)
        {
            isChasing = true;
            currentTarget = possess.transform;
        }
        else
        {
            isChasing = false;
            // Return to patrol between points A and B
            if (Vector3.Distance(transform.position, currentTarget.position) < 0.1f)
            {
                currentTarget = (currentTarget == pointA) ? pointB : pointA;
            }
        }
        
        // Move towards current target
        float speed = isChasing ? chaseSpeed : moveSpeed;
        transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, speed * Time.deltaTime);
        
        // Flip sprite based on movement direction
        if (currentTarget.position.x > transform.position.x)
        {
            spriteRenderer.flipX = false;
        }
        else if (currentTarget.position.x < transform.position.x)
        {
            spriteRenderer.flipX = true;
        }
    }

    private void PatrolBetweenPoints()
    {
        // Cek reach if patrol
        if (Vector3.Distance(transform.position, currentTarget.position) <= waypointThreshold)
        {
            // Chanhe target
            currentTarget = (currentTarget == pointA) ? pointB : pointA;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        PossessController possess = other.GetComponent<PossessController>();
        if (possess != null && possess.IsPossessing())
        {
            PossessedController possessed = possess.GetCurrentPossessed();
            if (possessed != null)
            {
                StartCoroutine(BlinkEffect(possessed));

                ResetAllSystems();
                possessed.ResetToInitialPosition();
                
                // Return possess to spawn point
                if (possessSpawnPoint != null)
                {
                    possess.transform.position = possessSpawnPoint.position;
                    possess.StopPossessing();
                    possess.BindToNewPossessed(null);
                }
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
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}