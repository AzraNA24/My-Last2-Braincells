using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public float GhostDelay;
    public float lifeTime = 0.2f;
    public GameObject ghost;

    private SpriteRenderer spriteRenderer;
    private float GhostDelaySec; 
    public bool makeGhost = false;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        GhostDelaySec = GhostDelay;
    }

    // Update is called once per frame
    void Update()
    {
        if(makeGhost){
            if(GhostDelaySec > 0){
            GhostDelaySec -= Time.deltaTime;
            } else{
            //generate ghost
            CreateGhost();
            GhostDelaySec = GhostDelay;
        }
        }
    }
    private void CreateGhost()
    {
        GameObject ghost = GhostPool.Instance.GetGhost();
        ghost.transform.position = transform.position;
        ghost.transform.rotation = transform.rotation;
        
        // Copy sprite properties
        SpriteRenderer ghostSprite = ghost.GetComponent<SpriteRenderer>();
        ghostSprite.sprite = spriteRenderer.sprite;
        ghostSprite.flipX = spriteRenderer.flipX;
        ghostSprite.color = new Color(1f, 1f, 1f, 0.5f); // Semi-transparent
        
        // Return to pool after lifetime
        StartCoroutine(ReturnToPoolAfterTime(ghost, lifeTime));
    }

    private IEnumerator ReturnToPoolAfterTime(GameObject ghost, float delay)
    {
        yield return new WaitForSeconds(delay);
        GhostPool.Instance.ReturnGhost(ghost);
    }

    public void SetMakeGhost(bool value)
    {
        makeGhost = value;
    }
}
