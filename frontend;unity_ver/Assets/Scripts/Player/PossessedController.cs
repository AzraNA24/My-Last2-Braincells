using UnityEngine;

public class PossessedController : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 5f;
    public Animator animator;

    private PossessController currentPossess;
    private Rigidbody2D rb;
    private Vector3 initialPosition;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        initialPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (animator == null) animator = GetComponent<Animator>();
        initialPosition = transform.position;
        animator.SetBool("IsPossessed", false);
    }

    public void Move(float amount)
    {
        Vector2 newPosition = rb.position + Vector2.right * amount;
        rb.MovePosition(newPosition);
    }

    public void BindPossess(PossessController possess)
    {
        currentPossess = possess;
        animator.SetBool("IsPossessed", true);
    }

    public void ReleasePossess()
    {
        currentPossess = null;
        animator.SetBool("IsPossessed", false);
    }

    public void FlipSprite(bool flipLeft)
    {
        transform.localScale = new Vector3(flipLeft ? -1 : 1, 1, 1);
    }

    public void TriggerWalkAnimation()
    {
        animator.SetTrigger("Walk");
    }

    public void ResetToInitialPosition()
    {
        transform.position = initialPosition;
        rb.velocity = Vector2.zero;
        ReleasePossess();
    }
}