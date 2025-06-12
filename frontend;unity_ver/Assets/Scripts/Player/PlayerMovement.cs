using System.Collections;
using System.Data.Common;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

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
    public Vector2 wallJumpPower = new Vector2 (5f, 10f);

    [Header("Dash")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.1f;
    public float dashCooldown = 0.1f;
    bool isDashing;
    bool canDash = true;
    TrailRenderer tr;

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    float horizontalInput;
    bool faceRight = true;
    float offsetValue = 0.75f;
    

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    public void Start()
    {
        tr = GetComponent<TrailRenderer>();
    }
    private void Update()
    {
        isGround();
        Gravity();
        WallSlide();
        WallJump();
        if(!isWallJump){
            rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
            Flip();
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalInput = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if(jumpRemain > 0){
            if(context.performed){
                rb.velocity = new Vector2(rb.velocity.x, jumpPower);
                jumpRemain--;
            }
            else if(context.canceled){
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
                jumpRemain--;
            }
        }
        
        if(context.performed && WallJumpTimer > 0){
            isWallJump = true;
            rb.velocity = new Vector2(WallJumpDirec * wallJumpPower.x, wallJumpPower.y);
            WallJumpTimer = 0;
            
            if(transform.localScale.x != WallJumpDirec){
                Flip();
            }

            Invoke(nameof(CancelWallJump), WallJumpTime +0.1f);
        }
    }
    private void isGround()
    {
        if(Physics2D.OverlapBox(groundChecker.position,groundSizeChecker, 0.5f, groundLayer)){
            jumpRemain =  maxJump;
            isGrounded = true;
        }
        else{
            isGrounded = false;
        }
    }
    
    private bool isWall()
    {
        return Physics2D.OverlapBox(wallChecker.position,wallSizeChecker, 0, wallLayer);
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
        if(faceRight && horizontalInput < 0 || !faceRight && horizontalInput > 0){
            faceRight = !faceRight;
            Vector3 pos = transform.position;
            Vector3 ls = transform.localScale;
            ls.x *= -1;
            transform.localScale = ls;
            if(!faceRight){
                pos.x += 1.2f;
            }
            else{
                pos.x -= 1.2f;
            }
            transform.position = pos;
        }
    }
    private void Gravity()
    {
        if(rb.velocity.y < 0){
            rb.gravityScale = baseGr * fallSpeedMux;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -maxFallSpeed));
        }
        else{
            rb.gravityScale = baseGr;
        }
    }

    private void WallSlide()
    {
        if(!isGrounded && isWall() && horizontalInput != 0){
            isSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -slideSpeed));
        }
        else{
            isSliding = false;
        }
    }

    private void WallJump()
    {
        if(isSliding){
            isWallJump = false;
            WallJumpDirec = -transform.localScale.x;
            WallJumpTimer = WallJumpTime;

            CancelInvoke(nameof(CancelWallJump));
        }
        else if(WallJumpTimer > 0f){
            WallJumpTimer -= Time.deltaTime;
        }
    }

    private void CancelWallJump()
    {
        isWallJump = false;
    }
    public void Dash(InputAction.CallbackContext context)
    {
        if(context.performed && canDash){
            StartCoroutine(DashCoroutine());
        }
    }
    private IEnumerator DashCoroutine(){
        canDash = false;
        isDashing = true;
        tr.emitting = true;
        float dashDirection = faceRight ? 1.2f : -1.2f;  

        rb.velocity = new Vector2(dashDuration *dashSpeed, rb.velocity.y);
        yield return new WaitForSeconds(dashDuration);

        rb.velocity = new Vector2(0f, rb.velocity.y);

        isDashing = false;
        tr.emitting = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;

    }
}