using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int collectibleCount = 0;
    public TMP_Text collectibleText;
    public GameObject[] characterPrefabs;
    
    private void Start()
    {
        int selectedIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);
        Instantiate(characterPrefabs[selectedIndex]);
    }

    public void AddCollectible()
    {
        collectibleCount++;
        collectibleText.text = $"{collectibleCount}";
        Debug.Log($"Collected: {collectibleCount}");
    }

    public int GetCollectibleItemsCount()
    {
        return collectibleCount;
    }

    public void ResetCollectibles()
    {
        collectibleCount = 0;
        collectibleText.text = "0";
        var allCollectibles = FindObjectsOfType<Collectible>(true);
        foreach (var collectible in allCollectibles)
        {
            collectible.Respawn();
        }
    }
}