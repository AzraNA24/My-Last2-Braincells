using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    
    [Header("Jump Settings")]
    [SerializeField] private float jumpPower = 10f;
    [SerializeField] private int maxJump = 2;
    int jumpRemain;
    public Transform groundChecker;
    public Vector2 groundSizeChecker = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;
    bool isGrounded;
    public Transform wallChecker;
    public Vector2 wallSizeChecker = new Vector2(0.5f, 0.05f);
    public LayerMask wallLayer;

    [Header("Gravity")]
    [SerializeField] private float baseGr = 2f;
    [SerializeField] private float maxFallSpeed = 10f;
    [SerializeField] private float fallSpeedMux = 2f;

    [Header("Wall Jump Settings")]
    [SerializeField] private float slideSpeed = 2f;
    bool isSliding;
    bool isWallJump;
    float WallJumpDirec;
    float WallJumpTime = 0.5f;
    float WallJumpTimer;
    public Vector2 wallJumpPower = new Vector2(5f, 10f);

    [Header("Dash")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.1f;
    public float dashCooldown = 0.1f;
    bool isDashing;
    bool canDash = true;
    TrailRenderer tr;

    [Header("Inactivity Settings")]
    [SerializeField] private float inactivityTimeLimit = 3f;
    [SerializeField] private Slider inactivitySlider;
    private float inactivityTimer;
    private Vector3 spawnPoint;
    private Vector2 lastPosition;
    private bool wasMoving;

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    float horizontalInput;
    bool faceRight = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        spawnPoint = transform.position;
        lastPosition = rb.position;
    }

    public void Start()
    {
        tr = GetComponent<TrailRenderer>();
        ResetInactivityTimer();
    }

    private void Update()
    {
        CheckMovement();
        horizontalInput = Input.GetAxisRaw("Horizontal");
        isGround();
        Gravity();
        WallSlide();
        WallJump();
        
        // Handle jump input in Update
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryJump();
        }

        if (!isWallJump)
        {
            rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
            Flip();
        }
    }

    private void TryJump()
    {
        if (jumpRemain > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            jumpRemain--;
        }
        else if (WallJumpTimer > 0)
        {
            isWallJump = true;
            rb.velocity = new Vector2(WallJumpDirec * wallJumpPower.x, wallJumpPower.y);
            WallJumpTimer = 0;

            if (transform.localScale.x != WallJumpDirec)
            {
                Flip();
            }

            Invoke(nameof(CancelWallJump), WallJumpTime + 0.1f);
        }
    }

    private void CheckMovement()
    {
        bool isMoving = rb.position != lastPosition || 
                       horizontalInput != 0 || 
                       Input.GetKey(KeyCode.Space) || 
                       isDashing;

        if (isMoving)
        {
            ResetInactivityTimer();
            wasMoving = true;
        }
        else if (wasMoving)
        {
            inactivityTimer += Time.deltaTime;
            
            if (inactivityTimer >= inactivityTimeLimit)
            {
                DieFromInactivity();
            }
        }

        lastPosition = rb.position;
    }

    private void ResetInactivityTimer()
    {
        inactivityTimer = 0f;
    }

    private void UpdateInactivityUI() 
    {
        if (inactivitySlider != null)
        {
            inactivitySlider.value = inactivityTimer / inactivityTimeLimit;
        }
    }

    private void DieFromInactivity()
    {
        transform.position = spawnPoint;
        rb.velocity = Vector2.zero;
        isWallJump = false;
        isDashing = false;
        canDash = true;
        ResetInactivityTimer();
        wasMoving = false;
        Debug.Log("Player died from inactivity!");
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalInput = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            TryJump();
        }
        else if (context.canceled && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
    }

    private void isGround()
    {
        if (Physics2D.OverlapBox(groundChecker.position, groundSizeChecker, 0.5f, groundLayer))
        {
            jumpRemain = maxJump;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
    
    private bool isWall()
    {
        return Physics2D.OverlapBox(wallChecker.position, wallSizeChecker, 0, wallLayer);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.grey;
        Gizmos.DrawWireCube(groundChecker.position, groundSizeChecker);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(wallChecker.position, wallSizeChecker);
    }

    private void Flip()
    {
        if ((faceRight && horizontalInput < 0) || (!faceRight && horizontalInput > 0))
        {
            faceRight = !faceRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1;
            transform.localScale = ls;
        }
    }

    private void Gravity()
    {
        if (rb.velocity.y < 0)
        {
            rb.gravityScale = baseGr * fallSpeedMux;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -maxFallSpeed));
        }
        else
        {
            rb.gravityScale = baseGr;
        }
    }

    private void WallSlide()
    {
        if (!isGrounded && isWall() && horizontalInput != 0)
        {
            isSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -slideSpeed));
        }
        else
        {
            isSliding = false;
        }
    }

    private void WallJump()
    {
        if (isSliding)
        {
            isWallJump = false;
            WallJumpDirec = -transform.localScale.x;
            WallJumpTimer = WallJumpTime;
            CancelInvoke(nameof(CancelWallJump));
        }
        else if (WallJumpTimer > 0f)
        {
            WallJumpTimer -= Time.deltaTime;
        }
    }

    private void CancelWallJump()
    {
        isWallJump = false;
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash)
        {
            StartCoroutine(DashCoroutine());
        }
    }

    private IEnumerator DashCoroutine()
    {
        canDash = false;
        isDashing = true;
        tr.emitting = true;
        float dashDirection = faceRight ? 1f : -1f;

        rb.velocity = new Vector2(dashDirection * dashSpeed, 0f);
        yield return new WaitForSeconds(dashDuration);

        tr.emitting = false;
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}