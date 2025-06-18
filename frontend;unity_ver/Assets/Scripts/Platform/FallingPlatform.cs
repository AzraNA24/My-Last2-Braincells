// using System.Collections;
// using System.Collections.Generic;
// using UnityEditor.Tilemaps;
// using UnityEngine;

// public class FallingPlatform : MonoBehaviour
// {
//     public float falling = 2f;
//     public float wait = 1f;

//     bool isfalling;
//     Rigidbody2D rb;
//     void Start()
//     {
//         rb = GetComponent<Rigidbody2D>();
//     }

//     private void OnCollisionEnter2D(Collision2D collision)
//     {
//         if(!isfalling && collision.gameObject.CompareTag("Player")){
//             StartCoroutine(Fall());
//         }
//     }

//     private IEnumerator Fall(){
//         isfalling = true;
//         yield return new WaitForSeconds(wait);
//         rb.bodyType = RigidbodyType2D.Dynamic;
//         Destroy(gameObject, wait);
//     }
// }
