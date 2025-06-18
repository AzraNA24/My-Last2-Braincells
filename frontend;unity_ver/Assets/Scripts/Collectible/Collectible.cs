using UnityEngine;

public class Collectible : MonoBehaviour
{
    public int value = 1;

    public bool isCollected = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.GetComponent<PossessController>() != null)
        {
            isCollected = true;
            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.AddCollectible();
            }
            gameObject.SetActive(false);
        }
    }

    public void Respawn()
    {
        isCollected = false;
        gameObject.SetActive(true);
    }
}