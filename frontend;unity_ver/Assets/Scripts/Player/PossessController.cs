using UnityEngine;
using System.Collections;
using System.Linq;

public class PossessController : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashSpeed = 10f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    public float moveSpeed = 5f;
    public Animator animator;
    [SerializeField] private Ghost ghost;

    [Header("Collider Settings")]
    public float colliderDelay = 0.1f;

    [Header("Binding")]
    public PossessedController currentPossessed;
    public float returnSpeed = 5f;
    public float returnThreshold = 0.1f;

    [Header("Detection")]
    public float detectionRadius = 3f;
    public LayerMask possessableLayer;

    [Header("Input")]
    public string horizontalInputAxis = "Horizontal";
    public string verticalInputAxis = "Vertical";
    public KeyCode dashModifierKey = KeyCode.LeftShift;

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private bool isDashing = false;
    private float dashTimer = 0f;
    private float cooldownTimer = 0f;
    private bool isPossessing = false;

    [Header("Respawn")]
    public Transform spawnPoint;

    [Header("Collection")]
    public LayerMask collectibleLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        if (animator == null) animator = GetComponent<Animator>();
        if (ghost == null) ghost = GetComponent<Ghost>();
        
        boxCollider.enabled = false;

        // Auto-possess nearest at start
        FindAndPossessNearest();
    }

    void Update()
    {
        // Check for new possessable objects
        if (!isDashing && !isPossessing)
        {
            FindAndPossessNearest();
        }

        // Input dash (Shift + WASD)
        if (Input.GetKey(dashModifierKey) && !isDashing && cooldownTimer <= 0)
        {
            float horizontalInput = Input.GetAxis(horizontalInputAxis);
            float verticalInput = Input.GetAxis(verticalInputAxis);
            
            if (Mathf.Abs(horizontalInput) > 0.1f || Mathf.Abs(verticalInput) > 0.1f)
            {
                Dash();
            }
        }

        // Update dash timer
        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            ghost.SetMakeGhost(true);
            if (dashTimer <= 0)
            {
                StopDash();
            }
        }

        // Cooldown timer
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        // If not dashing, return to possessed
        if (!isDashing && currentPossessed != null && !isPossessing)
        {
            ReturnToPossessed();
        }

        if (boxCollider.enabled)
        {
            CheckCollectibleCollision();
        }

        // Handle movement when possessing
        if (isPossessing && currentPossessed != null)
        {
            HandlePossessedMovement();
            SyncPositionWithPossessed();
        }
    }

    void FindAndPossessNearest()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, possessableLayer);
        
        foreach (var hit in hits)
        {
            if (hit.gameObject.layer == LayerMask.NameToLayer("Ground"))
                continue;

            PossessedController pc = hit.GetComponent<PossessedController>();
            if (pc != null && pc != currentPossessed)
            {
                // Switch possession to this new object
                if (currentPossessed != null)
                {
                    currentPossessed.ReleasePossess();
                }
                
                BindToNewPossessed(pc);
                pc.BindPossess(this);
                
                if (Vector2.Distance(transform.position, pc.transform.position) <= returnThreshold)
                {
                    StartPossessing();
                }
                break; // Only possess one at a time
            }
        }
    }

    private bool IsGroundLayer(GameObject obj)
    {
        return obj.layer == LayerMask.NameToLayer("Ground");
    }

    void ReturnToPossessed()
    {
        Vector2 direction = (currentPossessed.transform.position - transform.position).normalized;
        rb.velocity = direction * returnSpeed;
        
        if (Vector2.Distance(transform.position, currentPossessed.transform.position) <= returnThreshold)
        {
            StartPossessing();
        }
    }

    void SyncPositionWithPossessed()
    {
        transform.position = currentPossessed.transform.position;
    }

    void StartPossessing()
    {
        isPossessing = true;
        ghost.SetMakeGhost(false);
        sprite.enabled = false;
        currentPossessed.animator.SetBool("IsPossessed", true);
        rb.velocity = Vector2.zero;
    }

    public void StopPossessing()
    {
        isPossessing = false;
        if (currentPossessed != null)
        {
            currentPossessed.animator.SetBool("IsPossessed", false);
        }
        sprite.enabled = true;
    }

    void HandlePossessedMovement()
    {
        float horizontalInput = Input.GetAxis(horizontalInputAxis);
        
        if (Mathf.Abs(horizontalInput) > 0.1f)
        {
            float moveAmount = horizontalInput * moveSpeed * Time.deltaTime;
            currentPossessed.Move(moveAmount);
            currentPossessed.FlipSprite(horizontalInput < 0);
            currentPossessed.TriggerWalkAnimation();
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StopPossessing();
        }
    }

    void Dash()
    {
        float horizontalInput = Input.GetAxis(horizontalInputAxis);
        float verticalInput = Input.GetAxis(verticalInputAxis);

        Vector2 dashDirection = new Vector2(horizontalInput, verticalInput).normalized;
        if (dashDirection == Vector2.zero)
        {
            dashDirection = Vector2.right;
        }
        
        UpdateAnimationDirection(dashDirection);
        
        isDashing = true;
        sprite.enabled = true;
        isPossessing = false;
        animator.SetBool("isDashing", true);
        dashTimer = dashDuration;
        cooldownTimer = dashCooldown;
        sprite.enabled = true;
        
        if (currentPossessed != null)
        {
            currentPossessed.animator.SetBool("IsPossessed", false);
        }
        
        StartCoroutine(ActivateColliderAfterDelay());
        rb.velocity = dashDirection * dashSpeed;
    }

    void UpdateAnimationDirection(Vector2 direction)
    {
        sprite.flipX = direction.x < 0;
    }

    IEnumerator ActivateColliderAfterDelay()
    {
        yield return new WaitForSeconds(colliderDelay);
        if (isDashing)
        {
            boxCollider.enabled = true;
        }
    }

    void StopDash()
    {
        isDashing = false;
        animator.SetBool("isDashing", false);
        rb.velocity = Vector2.zero;
        boxCollider.enabled = false;
        ghost.SetMakeGhost(false);

        FindAndPossessNearest();
    }

    public void BindToNewPossessed(PossessedController newPossessed)
    {
        currentPossessed = newPossessed;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    public void Respawn()
    {
        if (spawnPoint != null)
        {
            transform.position = spawnPoint.position;
        }
        else
        {
            transform.position = Vector3.zero;
        }
        
        StopPossessing();
        currentPossessed = null;
        sprite.enabled = true;
    }

    void CheckCollectibleCollision()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, boxCollider.size.x / 2, collectibleLayer);
        foreach (var hit in hits)
        {
            Collectible collectible = hit.GetComponent<Collectible>();
            if (collectible != null)
            {
                GameManager gameManager = FindObjectOfType<GameManager>();
                if (gameManager != null)
                {
                    gameManager.AddCollectible();
                };
                Destroy(hit.gameObject);
            }
        }
    }

    public bool IsPossessing()
    {
        return currentPossessed != null;
    }

    public PossessedController GetCurrentPossessed()
    {
        return currentPossessed;
    }

}