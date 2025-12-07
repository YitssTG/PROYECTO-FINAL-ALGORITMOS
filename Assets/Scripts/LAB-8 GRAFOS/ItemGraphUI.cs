using System.Collections.Generic;
using UnityEngine;

public class ItemGraphUI : MonoBehaviour
{
    public GameObject nodePrefab;
    public RectTransform container;

    public float xSpacing = 180f;
    public float ySpacing = 200f;

    [Header("Nuevo sistema")]
    public ItemGraphData graphData;

    public ShopUI shopUI;

    public Dictionary<ItemData, ItemNodeUI> nodeLookup = new Dictionary<ItemData, ItemNodeUI>();
    private List<ItemData> itemsToDisplay = new List<ItemData>();

    public void RefreshGraph()
    {
        if (graphData == null)
        {
            Debug.LogWarning("ItemGraphUI: graphData no asignado");
            return;
        }

        ClearUI();

        itemsToDisplay = graphData.GetCleanItems();

        BuildNodes();
        ApplyLayout();
    }

    private void ClearUI()
    {
        for (int i = container.childCount - 1; i >= 0; i--)
        {
            Destroy(container.GetChild(i).gameObject);
        }

        nodeLookup.Clear();
        itemsToDisplay.Clear();
    }

    private void BuildNodes()
    {
        for (int i = 0; i < itemsToDisplay.Count; i++)
        {
            ItemData item = itemsToDisplay[i];
            if (item == null) continue;

            if (nodeLookup.ContainsKey(item))
                continue;

            GameObject nodeGO = Instantiate(nodePrefab, container);
            ItemNodeUI nodeUI = nodeGO.GetComponent<ItemNodeUI>();

            nodeUI.Initialize(item, this);
            nodeLookup[item] = nodeUI;
        }
    }

    private void ApplyLayout()
    {
        var layout = CalculateLayout(itemsToDisplay);

        foreach (var kvp in layout)
        {
            if (nodeLookup.TryGetValue(kvp.Key, out var node))
            {
                RectTransform rt = node.GetComponent<RectTransform>();
                rt.anchoredPosition = kvp.Value;
            }
        }
    }

    public Dictionary<ItemData, Vector2> CalculateLayout(List<ItemData> items)
    {
        Dictionary<ItemData, Vector2> layout = new Dictionary<ItemData, Vector2>();

        List<ItemData> roots = GetRootItems(items);

        float startX = -((roots.Count - 1) * xSpacing / 2f);

        for (int i = 0; i < roots.Count; i++)
        {
            Vector2 pos = new Vector2(startX + i * xSpacing, 0);
            SetRecursivePosition(roots[i], pos, items, layout);
        }

        return layout;
    }

    private void SetRecursivePosition(ItemData item, Vector2 pos, List<ItemData> items,
        Dictionary<ItemData, Vector2> layout)
    {
        layout[item] = pos;

        List<ItemData> children = GetChildren(item, items);
        if (children.Count == 0) return;

        float totalWidth = (children.Count - 1) * xSpacing;
        float startX = pos.x - totalWidth / 2;

        for (int i = 0; i < children.Count; i++)
        {
            Vector2 npos = new Vector2(startX + i * xSpacing, pos.y - ySpacing);
            SetRecursivePosition(children[i], npos, items, layout);
        }
    }

    private List<ItemData> GetRootItems(List<ItemData> items)
    {
        List<ItemData> roots = new List<ItemData>();

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].requiredItems == null || items[i].requiredItems.Length == 0)
                roots.Add(items[i]);
        }

        return roots;
    }

    private List<ItemData> GetChildren(ItemData parent, List<ItemData> items)
    {
        List<ItemData> children = new List<ItemData>();

        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            if (item == null || item.requiredItems == null) continue;

            for (int r = 0; r < item.requiredItems.Length; r++)
            {
                if (item.requiredItems[r] == parent)
                {
                    children.Add(item);
                    break;
                }
            }
        }

        return children;
    }

    public void OnItemSelected(ItemData item)
    {
        shopUI?.SelectItem(item);
    }

    public void ClearSelection()
    {
        foreach (var node in nodeLookup.Values)
            node.SetSelected(false);
    }
}
