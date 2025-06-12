using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float moveSpeed = 2f;

    private Vector3 nextPosition;

    void Start()
    {
        nextPosition = pointB.position;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, nextPosition, moveSpeed = Time.deltaTime);

        if(transform.position == nextPosition){
            nextPosition = (nextPosition == pointA.position) ? pointB.position : pointA.position;
        }
    }

    private void OnCollisionEnter2D(Collision2D coll){
        if(coll.gameObject.CompareTag("Player")){
            coll.gameObject.transform.parent = transform;
        }
    }

    private void OnCollisionExit2D(Collision2D coll){
        if(coll.gameObject.CompareTag("Player")){
            coll.gameObject.transform.parent = null;
        }
    }
}
