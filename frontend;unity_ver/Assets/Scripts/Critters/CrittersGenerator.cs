using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrittersGenerator : MonoBehaviour
{
    [SerializeField] private Sprite[] possibleSprites; 
    private SpriteRenderer spriteRenderer; 

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (possibleSprites != null && possibleSprites.Length > 0)
        {
            // Pilih sprite random dari array
            int randomIndex = Random.Range(0, possibleSprites.Length);
            Sprite selectedSprite = possibleSprites[randomIndex];
            
            // Terapkan sprite yang dipilih
            spriteRenderer.sprite = selectedSprite;
        }
        else
        {
            Debug.LogWarning("No sprites assigned to possibleSprites array", this);
        }
    }
}