using UnityEngine;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

    [Header("Items por Categoría")]
    public List<ItemData> attackItems = new List<ItemData>();
    public List<ItemData> defenseItems = new List<ItemData>();
    public List<ItemData> speedItems = new List<ItemData>();

    [Header("Todos los ítems combinados (solo lectura)")]
    public List<ItemData> allItems = new List<ItemData>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        RebuildAllItemsList();
    }

    private void RebuildAllItemsList()
    {
        HashSet<ItemData> unique = new HashSet<ItemData>();

        AddListToHash(unique, attackItems);
        AddListToHash(unique, defenseItems);
        AddListToHash(unique, speedItems);

        allItems = new List<ItemData>(unique);
    }

    private void AddListToHash(HashSet<ItemData> hash, List<ItemData> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] != null)
                hash.Add(list[i]);
        }
    }

    public void Initialize()
    {
        InitializeItemStates();
    }

    private void InitializeItemStates()
    {
        for (int i = 0; i < allItems.Count; i++)
        {
            ItemData item = allItems[i];
            if (item == null) continue;

            item.isPurchased = false;
            item.isUnlocked = item.requiredItems == null || item.requiredItems.Length == 0;
        }
    }

    public bool CanPurchaseItem(ItemData item)
    {
        if (item == null) return false;
        if (!item.isUnlocked) return false;
        if (item.isPurchased) return false;

        if (item.requiredItems != null)
        {
            for (int i = 0; i < item.requiredItems.Length; i++)
            {
                ItemData req = item.requiredItems[i];
                if (req != null && !req.isPurchased)
                    return false;
            }
        }

        return true;
    }

    public void PurchaseItem(ItemData item)
    {
        if (item == null) return;
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

            if (ok) item.isUnlocked = true;
        }
    }

    public void ResetAllItems()
    {
        for (int i = 0; i < allItems.Count; i++)
        {
            ItemData item = allItems[i];
            if (item == null) continue;

            item.isPurchased = false;
            item.isUnlocked = item.requiredItems == null || item.requiredItems.Length == 0;
        }
    }
    public List<ItemData> GetItemsForGraph(string type)
    {
        List<ItemData> list;

        switch (type.ToLower())
        {
            case "attack":
                list = attackItems;
                break;

            case "defense":
                list = defenseItems;
                break;

            case "speed":
                list = speedItems;
                break;

            default:
                list = allItems;
                break;
        }

        // LA SOLUCIÓN:
        // 1. Filtrar nulls
        // 2. Eliminar repetidos
        // 3. Crear lista final limpia
        HashSet<ItemData> unique = new HashSet<ItemData>();

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] != null)
                unique.Add(list[i]);
        }

        return new List<ItemData>(unique);
    }

    public List<ItemData> GetRootItems(List<ItemData> items)
    {
        List<ItemData> roots = new List<ItemData>();

        for (int i = 0; i < items.Count; i++)
        {
            ItemData item = items[i];
            if (item.requiredItems == null || item.requiredItems.Length == 0)
                roots.Add(item);
        }

        return roots;
    }
    public List<ItemData> GetChildren(ItemData parent, List<ItemData> items)
    {
        List<ItemData> list = new List<ItemData>();

        for (int i = 0; i < items.Count; i++)
        {
            ItemData item = items[i];
            if (item == null) continue;
            if (item.requiredItems == null) continue;

            for (int r = 0; r < item.requiredItems.Length; r++)
            {
                if (item.requiredItems[r] == parent)
                {
                    list.Add(item);
                    break;
                }
            }
        }

        return list;
    }
    public Dictionary<ItemData, Vector2> CalculateLayout(List<ItemData> items, float xSpacing, float ySpacing)
    {
        Dictionary<ItemData, Vector2> layout = new Dictionary<ItemData, Vector2>();

        List<ItemData> roots = GetRootItems(items);

        float startX = -((roots.Count - 1) * xSpacing / 2);

        for (int i = 0; i < roots.Count; i++)
        {
            Vector2 pos = new Vector2(startX + i * xSpacing, 0);
            SetRecursivePosition(roots[i], pos, items, layout, xSpacing, ySpacing);
        }

        return layout;
    }

    private void SetRecursivePosition(
        ItemData item,
        Vector2 pos,
        List<ItemData> items,
        Dictionary<ItemData, Vector2> layout,
        float xSpacing,
        float ySpacing)
    {
        layout[item] = pos;

        List<ItemData> children = GetChildren(item, items);
        if (children.Count == 0) return;

        float totalWidth = (children.Count - 1) * xSpacing;
        float startX = pos.x - totalWidth / 2;

        for (int i = 0; i < children.Count; i++)
        {
            Vector2 npos = new Vector2(startX + i * xSpacing, pos.y - ySpacing);
            SetRecursivePosition(children[i], npos, items, layout, xSpacing, ySpacing);
        }
    }
}
