// using UnityEngine;

// public class Soul : MonoBehaviour
// {
//     public float dashSpeed = 10f;
//     public float dashDuration = 0.2f;
//     public float returnSpeed = 5f;
//     public KeyCode dashKey = KeyCode.Space;

//     private bool isDashing = false;
//     private float dashTimeLeft;
//     private Vector2 dashDirection;
//     private Vector3 originalPosition;
//     private GameObject currentPossessTarget;
//     private Rigidbody2D rb;

//     void Start()
//     {
//         rb = GetComponent<Rigidbody2D>();
//         originalPosition = transform.position;
//     }

//     void Update()
//     {
//         if (Input.GetKeyDown(dashKey) && !isDashing)
//         {
//             StartDash();
//         }

//         if (isDashing)
//         {
//             dashTimeLeft -= Time.deltaTime;
//             if (dashTimeLeft <= 0)
//             {
//                 EndDash();
//             }
//             else
//             {
//                 rb.velocity = dashDirection * dashSpeed;
//             }
//         }
//         else if (currentPossessTarget == null)
//         {
//             // Kembali ke posisi asal jika tidak sedang memposes
//             transform.position = Vector3.MoveTowards(
//                 transform.position,
//                 originalPosition,
//                 returnSpeed * Time.deltaTime
//             );
//         }
//     }

//     void StartDash()
//     {
//         float moveX = Input.GetAxisRaw("Horizontal");
//         float moveY = Input.GetAxisRaw("Vertical");

//         if (moveX == 0 && moveY == 0) return; // Tidak ada input

//         dashDirection = new Vector2(moveX, moveY).normalized;
//         dashTimeLeft = dashDuration;
//         isDashing = true;
//     }

//     void EndDash()
//     {
//         isDashing = false;
//         rb.velocity = Vector2.zero;

//         // Cek apakah ada tubuh yang bisa diposes
//         Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.5f);
//         foreach (Collider2D hit in hits)
//         {
//             if (hit.CompareTag("Possessable") && hit.gameObject != currentPossessTarget)
//             {
//                 PossessNewBody(hit.gameObject);
//                 break;
//             }
//         }
//     }

//     void PossessNewBody(GameObject newBody)
//     {
//         // Lepaskan tubuh lama (jika ada)
//         if (currentPossessTarget != null)
//         {
//             currentPossessTarget.GetComponent<Possess>().IsPossessed = false;
//         }

//         // Poses tubuh baru
//         currentPossessTarget = newBody;
//         newBody.GetComponent<Possess>().IsPossessed = true;

//         // Pindahkan roh ke tubuh baru
//         originalPosition = newBody.transform.position;
//         transform.position = newBody.transform.position;
//     }

//     public void ReleaseBody()
//     {
//         if (currentPossessTarget != null)
//         {
//             currentPossessTarget.GetComponent<Possess>().IsPossessed = false;
//             currentPossessTarget = null;
//         }
//     }
// }