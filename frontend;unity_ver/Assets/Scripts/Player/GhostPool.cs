using UnityEngine;
using System.Collections.Generic;

public class GhostPool : MonoBehaviour
{
    public static GhostPool Instance;
    
    [SerializeField] private GameObject ghostPrefab;
    [SerializeField] private int poolSize = 10;
    
    private Queue<GameObject> ghostPool = new Queue<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject ghost = Instantiate(ghostPrefab);
            ghost.SetActive(false);
            ghostPool.Enqueue(ghost);
        }
    }

    public GameObject GetGhost()
    {
        if (ghostPool.Count == 0)
        {
            // Jika pool kosong, buat ghost baru
            GameObject ghost = Instantiate(ghostPrefab);
            return ghost;
        }
        
        GameObject pooledGhost = ghostPool.Dequeue();
        pooledGhost.SetActive(true);
        return pooledGhost;
    }

    public void ReturnGhost(GameObject ghost)
    {
        ghost.SetActive(false);
        ghostPool.Enqueue(ghost);
    }
}