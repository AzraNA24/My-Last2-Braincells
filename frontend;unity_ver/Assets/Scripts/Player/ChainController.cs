using UnityEngine;
using System.Collections.Generic;

public class ChainController : MonoBehaviour
{
    [Header("Chain Settings")]
    public GameObject possessObject;
    public GameObject possessedObject;
    public GameObject chainLinkPrefab; // Prefab untuk satu lingkaran rantai
    public float linkSize = 0.2f; // Ukuran setiap lingkaran
    public float linkSpacing = 0.3f; // Jarak antar lingkaran
    public Color chainColor = Color.white;

    [Header("Visibility")]
    public bool isVisible = false;

    private List<GameObject> chainLinks = new List<GameObject>();

    void Update()
    {
        UpdateChain();
        UpdateChainVisibility();
    }

    void UpdateChain()
    {
        // Clear existing links if we're not visible
        if (!isVisible && chainLinks.Count > 0)
        {
            ClearChain();
            return;
        }

        // Only create chain if visible
        if (!isVisible) return;

        if (possessObject == null || possessedObject == null || chainLinkPrefab == null)
        {
            ClearChain();
            return;
        }

        Vector3 startPos = possessObject.transform.position;
        Vector3 endPos = possessedObject.transform.position;
        Vector3 direction = endPos - startPos;
        float distance = direction.magnitude;

        int linkCount = Mathf.FloorToInt(distance / (linkSize + linkSpacing));
        if (linkCount < 1) linkCount = 1;

        // If we already have the correct number of links, just update positions
        if (chainLinks.Count == linkCount)
        {
            UpdateExistingChainLinks(startPos, endPos);
            return;
        }

        // Otherwise, create new chain
        ClearChain();
        CreateNewChain(startPos, endPos, linkCount);
    }

    void CreateNewChain(Vector3 startPos, Vector3 endPos, int linkCount)
    {
        for (int i = 0; i < linkCount; i++)
        {
            float t = (i + 0.5f) / linkCount;
            Vector3 linkPosition = Vector3.Lerp(startPos, endPos, t);

            GameObject link = Instantiate(chainLinkPrefab, transform);
            link.transform.position = linkPosition;
            link.transform.localScale = Vector3.one * linkSize;
            
            SpriteRenderer renderer = link.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.color = chainColor;
            }

            chainLinks.Add(link);
        }
    }

    void UpdateExistingChainLinks(Vector3 startPos, Vector3 endPos)
    {
        for (int i = 0; i < chainLinks.Count; i++)
        {
            float t = (i + 0.5f) / chainLinks.Count;
            Vector3 linkPosition = Vector3.Lerp(startPos, endPos, t);
            chainLinks[i].transform.position = linkPosition;
        }
    }

    void UpdateChainVisibility()
    {
        foreach (var link in chainLinks)
        {
            if (link != null)
            {
                link.SetActive(isVisible);
            }
        }
    }

    void ClearChain()
    {
        foreach (var link in chainLinks)
        {
            if (link != null)
            {
                Destroy(link);
            }
        }
        chainLinks.Clear();
    }

    public void SetVisibility(bool visible)
    {
        isVisible = visible;
        if (!visible)
        {
            ClearChain();
        }
    }
    public void UpdatePossessedObject(GameObject newPossessedObject)
{
    possessedObject = newPossessedObject;
    // Clear chain untuk dibuat ulang dengan objek baru
    ClearChain();
}
}