using UnityEngine;

public class PossessedController : MonoBehaviour
{
    [Header("Settings")]
    public float detectionRadius = 5f; // Radius untuk menarik Possess
    public LayerMask possessLayer; // Layer untuk deteksi Possess
    public Animator animator;

    private PossessController targetPossess;
    private PossessController currentPossess;

    void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
        animator.SetBool("isPossessed", false);
    }
    void Update()
    {
        // Cari Possess dalam radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, possessLayer);
        foreach (var hit in hits)
        {
            PossessController possess = hit.GetComponent<PossessController>();
            if (possess != null)
            {
                // Jika Possess belum terikat atau terikat ke lain, curi
                if (possess.currentPossessed != this)
                {
                    possess.BindToNewPossessed(this);
                    targetPossess = possess;
                }
            }
        }
    }

    public void BindPossess(PossessController possess)
    {
        currentPossess = possess;
        animator.SetBool("isPossessed", true); // Trigger ke PossessedState

        // Jika perlu, atur ulang posisi Possess
        possess.transform.position = transform.position;
    }

    // Dipanggil ketika Possess lepas
    public void ReleasePossess()
    {
        currentPossess = null;
        animator.SetBool("isPossessed", false); // Kembali ke StayState
    }

    // Visualisasi radius di Editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}