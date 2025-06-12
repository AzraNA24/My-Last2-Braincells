using UnityEngine;
using System.Collections;
using UnityEditor.Animations;

public class PossessController : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashSpeed = 10f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    public Animator animator;
    [SerializeField] private Ghost ghost;

    [Header("Collider Settings")]
    public float colliderDelay = 0.1f;

    [Header("Binding")]
    public PossessedController currentPossessed; // Possessed yang diikat
    public GameObject Effect;
    public float returnSpeed = 5f; // Kecepatan ditarik kembali
    public float returnThreshold = 0.1f;

    [Header("Chain Visual")]
    public GameObject chainLinkPrefab;
    public float chainLinkSize = 0.2f;
    public float chainLinkSpacing = 0.3f;
    public Color chainColor = Color.white;

    private ChainController chainController;

    [Header("Input")]
    public string horizontalInputAxis = "Horizontal"; // Input axis untuk horizontal (A/D atau LeftArrow/RightArrow)
    public string verticalInputAxis = "Vertical";
    
    private BoxCollider2D boxCollider;
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private bool isDashing = false;
    private float dashTimer = 0f;
    private float cooldownTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        if (animator == null) animator = GetComponent<Animator>();
        if (ghost == null) ghost = GetComponent<Ghost>();
        sprite.enabled = false; // Sprite mati awal
        boxCollider.enabled = false;

        chainController = gameObject.AddComponent<ChainController>();
        chainController.possessObject = this.gameObject;
        chainController.chainLinkPrefab = chainLinkPrefab;
        chainController.linkSize = chainLinkSize;
        chainController.linkSpacing = chainLinkSpacing;
        chainController.chainColor = chainColor;
        chainController.SetVisibility(false);
        UpdateChainReference();
    }

    void Update()
    {
        // Input dash (contoh: tombol Space)
        if (Input.GetKeyDown(KeyCode.Space) && !isDashing && cooldownTimer <= 0)
        {
            Dash();
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

        // Jika tidak dashing, tarik ke Possessed
        if (!isDashing && currentPossessed != null)
        {
            Vector2 direction = (currentPossessed.transform.position - transform.position).normalized;
            rb.velocity = direction * returnSpeed;
            float distanceToPossessed = Vector2.Distance(transform.position, currentPossessed.transform.position);
            if (distanceToPossessed <= returnThreshold)
            {
                ghost.SetMakeGhost(false);
                sprite.enabled = false;
                currentPossessed.animator.SetBool("isPossessed", true);
            }
        }
    }

    void Dash()
    {
        // Ambil input arah
        float horizontalInput = Input.GetAxis(horizontalInputAxis);
        float verticalInput = Input.GetAxis(verticalInputAxis);

        // Jika tidak ada input, dash ke kanan (default)
        Vector2 dashDirection = new Vector2(horizontalInput, verticalInput).normalized;
        if (dashDirection == Vector2.zero)
        {
            dashDirection = Vector2.right; // Default ke kanan jika tidak ada input
        }
        UpdateAnimationDirection(dashDirection);
        // Jalankan dash
        isDashing = true;
        StartCoroutine(PlayEffectAndPossess());
        animator.SetBool("isDashing", true);
        dashTimer = dashDuration;
        cooldownTimer = dashCooldown;
        sprite.enabled = true;
        currentPossessed.animator.SetBool("isPossessed", false);
        StartCoroutine(ActivateColliderAfterDelay());
        rb.velocity = dashDirection * dashSpeed;
        if (chainController != null)
        {
            chainController.SetVisibility(true);
        }
    }

    void UpdateAnimationDirection(Vector2 direction)
    {
        if (direction.x > 0)
        {
            sprite.flipX = false;
        }
        else if (direction.x < 0)
        {
            sprite.flipX = true;
        }
    }
    IEnumerator PlayEffectAndPossess()
    {
        // 1. Mainkan animasi Effect terlebih dahulu
        Animator childAnimator = Effect.GetComponentInChildren<Animator>();
        childAnimator.SetTrigger("isPossessed");
        yield return new WaitForSeconds(0.5f); // Ganti dengan durasi sebenarnya
    }
    IEnumerator ActivateColliderAfterDelay()
    {
        yield return new WaitForSeconds(colliderDelay); // Tunggu jeda
        if (isDashing) // Pastikan masih dalam keadaan dashing
        {
            boxCollider.enabled = true; // Aktifkan collider setelah delay
        }
    }

    void StopDash()
    {
        isDashing = false;
        animator.SetBool("isDashing", false);
        rb.velocity = Vector2.zero;
        boxCollider.enabled = false;
        StartCoroutine(HideChainAfterDelay(0.2f));
    }

    IEnumerator HideChainAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (chainController != null)
        {
            chainController.SetVisibility(false);
        }
    }
    void UpdatePossessedReference()
    {
        if (chainController != null)
        {
            chainController.possessedObject = currentPossessed != null ? currentPossessed.gameObject : null;
        }
    }
    // Dipanggil oleh Possessed untuk mengikat ulang
    public void BindToNewPossessed(PossessedController newPossessed)
    {
        currentPossessed = newPossessed;
        UpdatePossessedReference();
        UpdateChainReference();
    }
    void UpdateChainReference()
    {
        if (chainController != null && currentPossessed != null)
        {
            chainController.UpdatePossessedObject(currentPossessed.gameObject);
        }
        else if (chainController != null)
        {
            chainController.UpdatePossessedObject(null);
        }
    }
}