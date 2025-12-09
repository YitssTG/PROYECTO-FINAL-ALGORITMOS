using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

    [Header("Referencia a todos los grafos (SO)")]
    public List<ItemGraphData> allGraphs = new List<ItemGraphData>();

    private List<ItemData> allItems = new List<ItemData>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        BuildAllItems();
    }

    private void BuildAllItems()
    {
        HashSet<ItemData> unique = new HashSet<ItemData>();

        for (int g = 0; g < allGraphs.Count; g++)
        {
            var graph = allGraphs[g];
            if (graph == null) continue;

            graph.InitializeGraphItems(); 

            var items = graph.GetCleanItems();
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] != null)
                    unique.Add(items[i]);
            }
        }

        allItems = new List<ItemData>(unique);
    }

    public void Initialize()
    {
        ResetAllItems();
    }

    public bool CanPurchaseItem(ItemData item)
    {
        if (item == null) return false;
        if (!item.isUnlocked) return false;
        if (item.isPurchased) return false;

        if (item.requiredItems == null) return true;

        for (int i = 0; i < item.requiredItems.Length; i++)
        {
            ItemData req = item.requiredItems[i];
            if (req != null && !req.isPurchased)
                return false;
        }

        return true;
    }

    public void PurchaseItem(ItemData item)
    {
        if (!CanPurchaseItem(item)) return;

        item.isPurchased = true;
        UnlockDependents(item);
    }

    private void UnlockDependents(ItemData purchased)
    {
        for (int i = 0; i < allItems.Count; i++)
        {
            ItemData item = allItems[i];
            if (item == null) continue;
            if (item.isUnlocked) continue;
            if (item.requiredItems == null) continue;

            bool ok = true;

            for (int r = 0; r < item.requiredItems.Length; r++)
            {
                ItemData req = item.requiredItems[r];
                if (req != null && !req.isPurchased)
                {
                    ok = false;
                    break;
                }
            }

            if (ok)
                item.isUnlocked = true;
        }
    }

    public void ResetAllItems()
    {
        foreach (var item in allItems)
        {
            if (item != null)
            {
                item.isPurchased = false;
                item.isUnlocked = false;
            }
        }

        UnlockFirstItemOfEachCategory();
    }

    public void InitializeItemStates()
    {
        foreach (var graph in allGraphs) 
        {
            if (graph == null) continue;

            if (graph.items.Count > 0 && graph.items[0] != null)
            {
                graph.items[0].isUnlocked = true; 
            }

            foreach (var item in graph.items)
            {
                if (item != null)
                {
                    item.isPurchased = false;  
                    if (item != graph.items[0])  
                    {
                        item.isUnlocked = false;
                    }
                }
            }
        }
    }

    private void UnlockFirstItemOfEachCategory()
    {
        foreach (var graph in allGraphs)  
        {
            if (graph == null) continue;

            if (graph.items.Count > 0 && graph.items[0] != null)
            {
                graph.items[0].isUnlocked = true; 
            }
        }
    }
}
