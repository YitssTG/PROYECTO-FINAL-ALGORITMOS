using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemGraphUI : MonoBehaviour
{
    [Header("Referencias de UI")]
    public GameObject nodePrefab;
    public RectTransform container;

    [Header("Panel de información (compartido)")]
    public TMP_Text infoText;
    public Button buyButton;
    public Button undoButton;

    [Header("Datos del árbol")]
    public List<ItemData> allItems = new List<ItemData>();

    [Header("Referencias externas")]
    public GoldManager goldManager;
    public InventoryManager inventory;

    [Header("Espaciado visual")]
    public float xSpacing = 180f;
    public float ySpacing = 200f;

    [HideInInspector] public readonly Dictionary<ItemData, ItemNodeUI> nodeLookup = new();
    private readonly List<ItemData> purchaseHistory = new();
    private ItemData selectedItem;

    private bool initialized = false;

    // 🧩 Aquí conectamos con tus estructuras
    private OrientedGraph<ItemData> graph = new OrientedGraph<ItemData>();
    private Dictionary<ItemData, Node<ItemData>> graphNodes = new();

    public static ItemGraphUI ActiveGraph;

    private void OnEnable()
    {
        if (!initialized)
        {
            ResetItemsState(); // Solo la primera vez
            initialized = true;
        }

        BuildGraph();
        ArrangeTreeLayout();
        UpdateUnlocks();

        if (buyButton != null)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(() => ActiveGraph?.ConfirmPurchase());
        }

        if (undoButton != null)
        {
            undoButton.onClick.RemoveAllListeners();
            undoButton.onClick.AddListener(() => ActiveGraph?.UndoPurchase());
        }

        // 🧠 Solo para probar que se usa tu grafo real
        graph.PrintAdjacencyList();
        GetComponentInChildren<UIConnectionDrawer>()?.DrawConnections();
    }

    private void BuildGraph()
    {
        if (container == null || nodePrefab == null) return;
        foreach (Transform t in container)
            Destroy(t.gameObject);

        nodeLookup.Clear();
        graphNodes.Clear();
        graph = new OrientedGraph<ItemData>();

        // 🟢 1. Creamos un nodo del grafo para cada ítem
        foreach (var item in allItems)
        {
            var node = graph.AddNode(item);
            graphNodes[item] = node;

            GameObject go = Instantiate(nodePrefab, container);
            ItemNodeUI ui = go.GetComponent<ItemNodeUI>();
            ui.Initialize(item, this);
            nodeLookup[item] = ui;
        }

        // 🟢 2. Creamos las conexiones según los requiredItems
        foreach (var item in allItems)
        {
            if (item.requiredItems == null) continue;

            foreach (var req in item.requiredItems)
            {
                if (graphNodes.ContainsKey(req) && graphNodes.ContainsKey(item))
                    graph.AddEdge(graphNodes[req], graphNodes[item]);
            }
        }
    }

    private void ArrangeTreeLayout()
    {
        List<ItemData> roots = allItems.FindAll(i => i.requiredItems == null || i.requiredItems.Length == 0);
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
        rt.anchoredPosition = pos;

        // 🧩 Usamos el grafo real para obtener los hijos
        if (!graphNodes.ContainsKey(item)) return;
        var node = graphNodes[item];

        // 🟢 Lista de hijos
        List<Node<ItemData>> children = node.Neighbors;
        if (children.Count == 0) return;

        // 🔹 Calcular separación horizontal
        float totalWidth = (children.Count - 1) * xSpacing;
        float startX = pos.x - totalWidth / 2;

        // 🔹 Repartir los hijos horizontalmente
        for (int i = 0; i < children.Count; i++)
        {
            ItemData child = children[i].Value;
            float childX = startX + i * xSpacing;
            float childY = pos.y - ySpacing;

            PositionRecursive(child, new Vector2(childX, childY), depth + 1);
        }
    }

    public void SelectItem(ItemData item)
    {
        selectedItem = item;
        ActiveGraph = this;

        if (infoText != null)
            infoText.text = $"{item.itemName}\nCosto: {item.cost}\n\n{item.description}";
    }

    private void ConfirmPurchase()
    {
        if (selectedItem == null || selectedItem.isPurchased || !selectedItem.isUnlocked) return;

        if (!goldManager.SpendGold(selectedItem.cost))
        {
            if (infoText != null)
                infoText.text = "❌ No tienes suficiente oro.";
            return;
        }

        selectedItem.isPurchased = true;
        purchaseHistory.Add(selectedItem);
        inventory.AddItem(selectedItem);
        UpdateUnlocks();

        if (infoText != null)
            infoText.text = $"✅ Compraste {selectedItem.itemName}";
    }

    private void UndoPurchase()
    {
        if (purchaseHistory.Count == 0) return;

        ItemData last = purchaseHistory[^1];
        purchaseHistory.RemoveAt(purchaseHistory.Count - 1);
        last.isPurchased = false;

        goldManager.AddGold(last.cost);
        inventory.RemoveItem(last);
        UpdateUnlocks();

        if (infoText != null)
            infoText.text = $"↩️ Revertiste {last.itemName}";
    }

    private void UpdateUnlocks()
    {
        foreach (var item in allItems)
        {
            if (item.requiredItems == null || item.requiredItems.Length == 0)
                item.isUnlocked = true;
            else
            {
                bool unlocked = true;
                foreach (var req in item.requiredItems)
                    if (!req.isPurchased) unlocked = false;
                item.isUnlocked = unlocked;
            }

            if (nodeLookup.ContainsKey(item))
                nodeLookup[item].UpdateState();
        }
        GetComponentInChildren<UIConnectionDrawer>()?.DrawConnections();

    }

    private void ResetItemsState()
    {
        foreach (var item in allItems)
        {
            item.isUnlocked = false;
            item.isPurchased = false;
        }
    }

    public void ClearSelection()
    {
        selectedItem = null;
        if (infoText != null)
            infoText.text = "Selecciona un ítem para ver detalles.";
    }
}
