using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HealthBlocks : MonoBehaviour
{
    public Image healthBlockPrefab; // Drag the HealthBlock prefab here in the inspector
    public float spacing = 10f; // Spacing between blocks
    private List<Image> healthBlocks = new List<Image>();

    public void SetHealth(int health)
    {
        // Clear current blocks
        foreach (var block in healthBlocks)
        {
            Destroy(block.gameObject);
        }
        healthBlocks.Clear();

        // Create new blocks
        for (int i = 0; i < health; i++)
        {
            Image newBlock = Instantiate(healthBlockPrefab, transform);
            newBlock.rectTransform.anchoredPosition = new Vector2(i * (healthBlockPrefab.rectTransform.sizeDelta.x + spacing), 0);
            healthBlocks.Add(newBlock);
        }
    }
}
