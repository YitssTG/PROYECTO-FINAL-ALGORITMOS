using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemGraphUI : MonoBehaviour
{
    [Header("Referencias de UI")]
    public GameObject nodePrefab;
    public RectTransform container;

    [Header("Espaciado visual")]
    public float xSpacing = 180f;
    public float ySpacing = 200f;

    [Header("Tipo de gráfico")]
    public string graphType; // "Attack", "Defense", "Speed"

    [Header("Referencia a ShopUI (ASIGNADA DESDE ShopUI)")]
    public ShopUI shopUI;

    public Dictionary<ItemData, ItemNodeUI> nodeLookup { get; private set; } = new Dictionary<ItemData, ItemNodeUI>();
    private List<ItemData> itemsToDisplay = new List<ItemData>();

    public static ItemGraphUI ActiveGraph { get; set; }

    private void Start()
    {
        // No generar nada automáticamente
    }

    public void RefreshGraph()
    {
        if (container != null)
        {
            foreach (Transform child in container)
                Destroy(child.gameObject);
        }

        nodeLookup.Clear();
        itemsToDisplay.Clear();

        if (ItemManager.Instance != null)
        {
            foreach (var item in ItemManager.Instance.allItems)
            {
                if (ShouldDisplayInThisGraph(item))
                {
                    itemsToDisplay.Add(item);
                }
            }
        }

        BuildVisualGraph();
    }

    private bool ShouldDisplayInThisGraph(ItemData item)
    {
        if (string.IsNullOrEmpty(graphType)) return true;

        return graphType.ToLower() switch
        {
            "attack" => item.bonusDamage > 0,
            "defense" => item.bonusArmor > 0,
            "speed" => item.bonusSpeed > 0,
            _ => true
        };
    }

    private void BuildVisualGraph()
    {
        if (nodePrefab == null || container == null) return;

        foreach (var item in itemsToDisplay)
        {
            GameObject nodeGO = Instantiate(nodePrefab, container);
            ItemNodeUI nodeUI = nodeGO.GetComponent<ItemNodeUI>();
            if (nodeUI != null)
            {
                nodeUI.Initialize(item, this);
                nodeLookup[item] = nodeUI;
            }
        }

        ArrangeTreeLayout();
    }

    private void ArrangeTreeLayout()
    {
        List<ItemData> roots = itemsToDisplay.FindAll(i =>
            i.requiredItems == null || i.requiredItems.Length == 0);

        float startX = -((roots.Count - 1) * xSpacing / 2);
        for (int i = 0; i < roots.Count; i++)
        {
            PositionRecursive(roots[i], new Vector2(startX + i * xSpacing, 0), 0);
        }
    }

    private void PositionRecursive(ItemData item, Vector2 pos, int depth)
    {
        if (!nodeLookup.ContainsKey(item)) return;

        RectTransform rt = nodeLookup[item].GetComponent<RectTransform>();
        if (rt != null)
            rt.anchoredPosition = pos;

        var children = GetChildren(item);
        if (children.Count == 0) return;

        float totalWidth = (children.Count - 1) * xSpacing;
        float startX = pos.x - totalWidth / 2;

        for (int i = 0; i < children.Count; i++)
        {
            float childX = startX + i * xSpacing;
            float childY = pos.y - ySpacing;
            PositionRecursive(children[i], new Vector2(childX, childY), depth + 1);
        }
    }

    private List<ItemData> GetChildren(ItemData parent)
    {
        List<ItemData> children = new List<ItemData>();

        foreach (var item in itemsToDisplay)
        {
            if (item.requiredItems != null)
            {
                foreach (var req in item.requiredItems)
                {
                    if (req == parent)
                    {
                        children.Add(item);
                        break;
                    }
                }
            }
        }

        return children;
    }

    public void OnItemSelected(ItemData item)
    {
        if (shopUI != null)
        {
            shopUI.SelectItem(item);
        }
    }

    public void ClearSelection()
    {
        foreach (var node in nodeLookup.Values)
        {
            node.SetSelected(false);
        }
    }
}
