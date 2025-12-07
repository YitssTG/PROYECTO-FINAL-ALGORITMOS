using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class ItemGraphUI : MonoBehaviour
{
    [Header("Prefabs / Contenedor")]
    public GameObject nodePrefab;
    public RectTransform container;

    [Header("Layout")]
    public float xSpacing = 180f;
    public float ySpacing = 200f;

    [Header("Datos del grafo (ScriptableObject)")]
    public ItemGraphData graphData;

    [Header("Referencia UI")]
    public ShopUI shopUI;

    // lookup público para que otros componentes (UIConnectionDrawer) puedan leerlo
    public Dictionary<ItemData, ItemNodeUI> nodeLookup = new Dictionary<ItemData, ItemNodeUI>();

    private List<ItemData> itemsToDisplay = new List<ItemData>();

    // --- Refresh/UI build ---
    public void RefreshGraph()
    {
        if (nodePrefab == null || container == null)
        {
            Debug.LogWarning("ItemGraphUI: nodePrefab o container no asignado");
            return;
        }

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

    // --- Limpia nodos existentes ---
    private void ClearUI()
    {
        if (container != null)
        {
            for (int i = container.childCount - 1; i >= 0; i--)
                Destroy(container.GetChild(i).gameObject);
        }

        nodeLookup.Clear();
        itemsToDisplay.Clear();
    }

    // --- Crea los nodos (una sola vez por ItemData) ---
    private void BuildNodes()
    {
        for (int i = 0; i < itemsToDisplay.Count; i++)
        {
            ItemData item = itemsToDisplay[i];
            if (item == null) continue;

            if (nodeLookup.ContainsKey(item)) continue;

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

    // --- Calcula y aplica posiciones ---
    private void ApplyLayout()
    {
        var layout = CalculateLayout(itemsToDisplay);

        foreach (var kvp in layout)
        {
            if (kvp.Key == null) continue;
            if (nodeLookup.TryGetValue(kvp.Key, out var node))
            {
                RectTransform rt = node.GetComponent<RectTransform>();
                if (rt != null)
                    rt.anchoredPosition = kvp.Value;
            }
        }
    }

    // --- API pública usada por ItemNodeUI al hacer click ---
    public void OnItemSelected(ItemData item)
    {
        if (item == null) return;

        ClearSelection();

        if (nodeLookup.ContainsKey(item))
            nodeLookup[item].SetSelected(true);

        // Notificar al ShopUI
        shopUI?.SelectItem(item);
    }

    // --- Limpia selección visual de todos los nodos ---
    public void ClearSelection()
    {
        foreach (var node in nodeLookup.Values)
            node.SetSelected(false);
    }

    // --- Layout helpers (idéntico a antes pero encapsulado aquí) ---
    public Dictionary<ItemData, Vector2> CalculateLayout(List<ItemData> items)
    {
        Dictionary<ItemData, Vector2> layout = new Dictionary<ItemData, Vector2>();

        List<ItemData> roots = GetRootItems(items);
        float startX = -((roots.Count - 1) * xSpacing / 2f);

        for (int i = 0; i < roots.Count; i++)
        {
            Vector2 pos = new Vector2(startX + i * xSpacing, 0f);
            SetRecursivePosition(roots[i], pos, items, layout);
        }

        return layout;
    }

    private void SetRecursivePosition(ItemData item, Vector2 pos, List<ItemData> items, Dictionary<ItemData, Vector2> layout)
    {
        layout[item] = pos;

        List<ItemData> children = GetChildren(item, items);
        if (children.Count == 0) return;

        float totalWidth = (children.Count - 1) * xSpacing;
        float startX = pos.x - totalWidth / 2f;

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
            var it = items[i];
            if (it == null) continue;
            if (it.requiredItems == null || it.requiredItems.Length == 0) roots.Add(it);
        }
        return roots;
    }

    private List<ItemData> GetChildren(ItemData parent, List<ItemData> items)
    {
        List<ItemData> children = new List<ItemData>();
        for (int i = 0; i < items.Count; i++)
        {
            var it = items[i];
            if (it == null || it.requiredItems == null) continue;
            for (int r = 0; r < it.requiredItems.Length; r++)
            {
                if (it.requiredItems[r] == parent)
                {
                    children.Add(it);
                    break;
                }
            }
        }
        return children;
    }
}
