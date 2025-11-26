using UnityEngine;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

    [Header("Todos los items del juego")]
    public List<ItemData> allItems = new List<ItemData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Initialize()
    {
        InitializeItemStates();
        Debug.Log($"✅ ItemManager inicializado con {allItems.Count} items");
    }

    private void InitializeItemStates()
    {
        foreach (var item in allItems)
        {
            if (item != null)
            {
                // Items sin requerimientos empiezan desbloqueados
                item.isUnlocked = item.requiredItems == null || item.requiredItems.Length == 0;
                item.isPurchased = false;

                Debug.Log($"🔧 {item.itemName} - Unlocked: {item.isUnlocked}, Purchased: {item.isPurchased}");
            }
        }
    }

    public bool CanPurchaseItem(ItemData item)
    {
        if (item == null)
        {
            Debug.LogWarning("❌ Item nulo en CanPurchaseItem");
            return false;
        }

        bool canPurchase = item.isUnlocked && !item.isPurchased;

        // Verificar requerimientos
        if (item.requiredItems != null && item.requiredItems.Length > 0)
        {
            foreach (var req in item.requiredItems)
            {
                if (req != null && !req.isPurchased)
                {
                    canPurchase = false;
                    break;
                }
            }
        }

        Debug.Log($"🔍 {item.itemName} - CanPurchase: {canPurchase} (Unlocked: {item.isUnlocked}, Purchased: {item.isPurchased})");
        return canPurchase;
    }

    public void PurchaseItem(ItemData item)
    {
        if (item != null && CanPurchaseItem(item))
        {
            item.isPurchased = true;

            // Desbloquear items que dependen de este
            UnlockDependentItems(item);

            Debug.Log($"🛍️ {item.itemName} marcado como comprado");
        }
        else
        {
            Debug.LogWarning($"❌ No se pudo comprar {item.itemName}");
        }
    }

    private void UnlockDependentItems(ItemData purchasedItem)
    {
        int unlockedCount = 0;

        foreach (var item in allItems)
        {
            if (item != null && item.requiredItems != null && !item.isUnlocked)
            {
                bool allRequirementsMet = true;

                foreach (var req in item.requiredItems)
                {
                    if (req == purchasedItem && !req.isPurchased)
                    {
                        allRequirementsMet = false;
                        break;
                    }
                }

                if (allRequirementsMet)
                {
                    item.isUnlocked = true;
                    unlockedCount++;
                    Debug.Log($"🔓 {item.itemName} desbloqueado por comprar {purchasedItem.itemName}");
                }
            }
        }

        if (unlockedCount > 0)
        {
            Debug.Log($"🎯 {unlockedCount} items desbloqueados tras comprar {purchasedItem.itemName}");
        }
    }

    public void ResetAllItems()
    {
        foreach (var item in allItems)
        {
            if (item != null)
            {
                item.isPurchased = false;
                item.isUnlocked = item.requiredItems == null || item.requiredItems.Length == 0;
            }
        }
        Debug.Log("🔄 Todos los items reiniciados");
    }

    // Método para debuggear conexiones
    public void DebugItemConnections()
    {
        Debug.Log("🔗 CONEXIONES ENTRE ITEMS:");
        foreach (var item in allItems)
        {
            if (item != null)
            {
                string requirements = item.requiredItems != null ?
                    string.Join(", ", System.Array.ConvertAll(item.requiredItems, x => x?.itemName)) : "Ninguno";

                Debug.Log($"   {item.itemName} → Requiere: [{requirements}] | Unlocked: {item.isUnlocked} | Purchased: {item.isPurchased}");
            }
        }
    }
}