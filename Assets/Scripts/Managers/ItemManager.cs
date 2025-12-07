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

        // Iterar sobre todos los grafos y asegurarse de que los ítems del primer ítem de cada categoría se desbloqueen.
        for (int g = 0; g < allGraphs.Count; g++)
        {
            var graph = allGraphs[g];
            if (graph == null) continue;

            graph.InitializeGraphItems();  // Aseguramos que el primer ítem del gráfico se desbloquee.

            var items = graph.GetCleanItems();
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] != null)
                    unique.Add(items[i]);
            }
        }

        allItems = new List<ItemData>(unique);
    }

    // ---------- ESTADO INICIAL ----------
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

        // Aseguramos que el primer ítem de cada categoría se desbloquee
        UnlockFirstItemOfEachCategory();
    }

    // Inicializa los estados de los ítems (desbloqueo y compra)
    public void InitializeItemStates()
    {
        foreach (var graph in allGraphs)  // Recorremos todos los gráficos de ítems
        {
            if (graph == null) continue;

            // Desbloquear el primer ítem de cada categoría (independientemente de otras categorías)
            if (graph.items.Count > 0 && graph.items[0] != null)
            {
                graph.items[0].isUnlocked = true;  // Desbloquear el primer ítem de esta categoría
            }

            // Reiniciar todos los ítems de la categoría para que estén bloqueados y no comprados
            foreach (var item in graph.items)
            {
                if (item != null)
                {
                    item.isPurchased = false;  // Reiniciar compra
                    if (item != graph.items[0])  // Solo desbloquear el primero
                    {
                        item.isUnlocked = false;
                    }
                }
            }
        }
    }

    // Método para desbloquear el primer ítem de cada categoría de manera independiente
    private void UnlockFirstItemOfEachCategory()
    {
        foreach (var graph in allGraphs)  // Iterar sobre todos los grafos de ítems
        {
            if (graph == null) continue;

            // Desbloquear el primer ítem de cada categoría (ataque, defensa, velocidad) si está disponible
            if (graph.items.Count > 0 && graph.items[0] != null)
            {
                graph.items[0].isUnlocked = true;  // Desbloquear el primer ítem de esta categoría
            }
        }
    }
}
