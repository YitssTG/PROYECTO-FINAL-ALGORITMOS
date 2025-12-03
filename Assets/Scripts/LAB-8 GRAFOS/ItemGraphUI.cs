using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemGraphUI : MonoBehaviour
{
    public GameObject nodePrefab;
    public RectTransform container;

    public float xSpacing = 180f;
    public float ySpacing = 200f;

    public string graphType;

    public ShopUI shopUI;

    public Dictionary<ItemData, ItemNodeUI> nodeLookup = new Dictionary<ItemData, ItemNodeUI>();

    private List<ItemData> itemsToDisplay = new List<ItemData>();

    public static ItemGraphUI ActiveGraph { get; set; }

    public void RefreshGraph()
    {
        if (nodePrefab == null || container == null)
        {
            Debug.LogWarning("ItemGraphUI: nodePrefab o container no asignado");
            return;
        }

        ClearUI();

        itemsToDisplay = ItemManager.Instance.GetItemsForGraph(graphType);

        itemsToDisplay = itemsToDisplay.Where(x => x != null).Distinct().ToList();

        BuildNodes();

        ApplyLayout();

        Debug.Log($"ItemGraphUI ({graphType}) - nodos instanciados: {nodeLookup.Count}");
    }

    private void ClearUI()
    {
        if (container != null)
        {
            for (int i = container.childCount - 1; i >= 0; i--)
            {
                Destroy(container.GetChild(i).gameObject);
            }
        }

        nodeLookup.Clear();
        itemsToDisplay.Clear();
    }

    private void BuildNodes()
    {
        for (int i = 0; i < itemsToDisplay.Count; i++)
        {
            var item = itemsToDisplay[i];
            if (item == null) continue;

            if (nodeLookup.ContainsKey(item))
            {
                continue;
            }

            GameObject nodeGO = Instantiate(nodePrefab, container);
            ItemNodeUI nodeUI = nodeGO.GetComponent<ItemNodeUI>();
            if (nodeUI == null)
            {
                Debug.LogError("ItemGraphUI: nodePrefab no contiene ItemNodeUI");
                Destroy(nodeGO);
                continue;
            }

            nodeUI.Initialize(item, this);
            nodeLookup[item] = nodeUI;
        }
    }

    private void ApplyLayout()
    {
        var layout = ItemManager.Instance.CalculateLayout(itemsToDisplay, xSpacing, ySpacing);

        foreach (var kvp in layout)
        {
            var item = kvp.Key;
            Vector2 pos = kvp.Value;
            if (item == null) continue;

            if (nodeLookup.TryGetValue(item, out var node))
            {
                RectTransform rt = node.GetComponent<RectTransform>();
                if (rt != null)
                    rt.anchoredPosition = pos;
            }
        }
    }

    public void OnItemSelected(ItemData item)
    {
        if (shopUI != null)
            shopUI.SelectItem(item);
    }

    public void ClearSelection()
    {
        foreach (var node in nodeLookup.Values)
            node.SetSelected(false);
    }
}
