using UnityEngine;
using UnityEngine.Events;
using System.Text;

public class ShopManager : MonoBehaviour
{
    public ItemManager itemManager;
    public GoldManager goldManager;
    public InventoryManager inventoryManager;
    public PlayerStats playerStats;
    public ShopUI shopUI;
    public GameObject shopPanel;

    private bool isShopOpen = false;

    public UnityEvent<ItemData> OnItemPurchased = new UnityEvent<ItemData>();

    public void Initialize()
    {
        Debug.Log("ShopManager Inicializado");
    }

    public bool IsShopOpen()
    {
        return shopPanel != null && shopPanel.activeSelf;
    }

    public void ToggleShop()
    {
        if (shopPanel == null)
        {
            Debug.LogWarning("ShopManager.ToggleShop: shopPanel no asignado");
            isShopOpen = !isShopOpen;
            return;
        }

        bool newState = !shopPanel.activeSelf;
        shopPanel.SetActive(newState);
        isShopOpen = newState;

        if (shopUI != null)
        {
            if (newState) shopUI.OpenShop();
            else shopUI.CloseShop();
        }
    }

    public bool CanPurchase(ItemData item)
    {
        if (item == null) return false;

        if (item.isPurchased) return false;

        if (itemManager == null)
        {
            Debug.LogWarning("ShopManager.CanPurchase: ItemManager no asignado");
            return false;
        }

        if (!itemManager.CanPurchaseItem(item)) return false;

        if (goldManager == null)
        {
            Debug.LogWarning("ShopManager.CanPurchase: GoldManager no asignado");
            return false;
        }

        if (goldManager.currentGold < item.cost) return false;

        return true;
    }

    public bool PurchaseItem(ItemData item)
    {
        if (item == null)
        {
            Debug.LogWarning("PurchaseItem: item nulo");
            return false;
        }

        if (!CanPurchase(item))
        {
            Debug.Log("PurchaseItem: no se puede comprar " + item.itemName);
            return false;
        }

        if (goldManager == null)
        {
            Debug.LogWarning("PurchaseItem: GoldManager no asignado");
            return false;
        }

        bool spent = goldManager.SpendGold(item.cost);
        if (!spent)
        {
            Debug.LogWarning("PurchaseItem: no se pudo gastar el oro");
            return false;
        }

        if (itemManager == null)
        {
            Debug.LogWarning("PurchaseItem: ItemManager no asignado");
            return false;
        }

        itemManager.PurchaseItem(item);

        if (inventoryManager != null)
            inventoryManager.AddItem(item);
        else
            Debug.LogWarning("PurchaseItem: InventoryManager no asignado - no se agregó al inventario");

        ApplyStats(item);

        // Notificar via evento
        OnItemPurchased?.Invoke(item);

        // Refrescar UI si está asignada
        if (shopUI != null)
            shopUI.RefreshUI();

        Debug.Log("PurchaseItem: compra exitosa " + item.itemName);
        return true;
    }

    private void ApplyStats(ItemData item)
    {
        if (playerStats == null || item == null) return;

        if (item.bonusDamage > 0) playerStats.IncreaseDamage(item.bonusDamage);
        if (item.bonusArmor > 0) playerStats.IncreaseArmor(item.bonusArmor);
        if (item.bonusSpeed > 0) playerStats.IncreaseSpeed(item.bonusSpeed);
    }

    public string GetItemInfo(ItemData item)
    {
        if (item == null) return "Selecciona un ítem para ver detalles.";

        string estado;
        if (item.isPurchased) estado = "Comprado";
        else if (item.isUnlocked) estado = "Disponible";
        else estado = "Bloqueado";

        var sb = new StringBuilder();
        sb.AppendLine(item.itemName);
        sb.AppendLine();
        sb.AppendLine(item.description);
        sb.AppendLine();
        sb.AppendLine($"Costo: {item.cost}G");
        sb.AppendLine($"Estado: {estado}");
        sb.AppendLine();

        if (item.bonusDamage > 0) sb.AppendLine($"Daño: +{item.bonusDamage}");
        if (item.bonusArmor > 0) sb.AppendLine($"Armadura: +{item.bonusArmor}");
        if (item.bonusSpeed > 0) sb.AppendLine($"Velocidad: +{item.bonusSpeed}");

        if (item.requiredItems != null && item.requiredItems.Length > 0)
        {
            sb.AppendLine();
            sb.AppendLine("Requisitos:");
            foreach (var req in item.requiredItems)
            {
                if (req == null) continue;
                string estadoReq = req.isPurchased ? "(Listo)" : "(Falta)";
                sb.AppendLine($"{req.itemName} {estadoReq}");
            }
        }

        return sb.ToString();
    }
}
