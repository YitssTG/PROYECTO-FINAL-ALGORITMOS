using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [Header("Dependencies")]
    public ItemManager itemManager;
    public GoldManager goldManager;
    public InventoryManager inventoryManager;
    public PlayerStats playerStats;
    public ShopUI shopUI;

    private bool isShopOpen = false;

    public void Initialize()
    {
        Debug.Log("✅ ShopManager inicializado");
    }

    public void ToggleShop()
    {
        isShopOpen = !isShopOpen;

        if (shopUI != null)
        {
            if (isShopOpen)
            {
                shopUI.OpenShop();
                Debug.Log("🛒 Tienda abierta");
            }
            else
            {
                shopUI.CloseShop();
                Debug.Log("🛒 Tienda cerrada");
            }
        }
    }

    public void PurchaseItem(ItemData item)
    {
        if (item == null)
        {
            Debug.LogWarning("❌ Item nulo, no se puede comprar");
            return;
        }

        Debug.Log($"🛒 Intentando comprar: {item.itemName} por {item.cost} oro");

        // 1. Verificar si se puede comprar
        if (!CanPurchaseItem(item))
        {
            Debug.LogWarning($"❌ No se puede comprar {item.itemName}");
            return;
        }

        // 2. Gastar oro (VERIFICAR QUE SE DESCUEENTE)
        if (goldManager != null)
        {
            bool spentGold = goldManager.SpendGold(item.cost);
            Debug.Log($"💰 Gastar oro: {item.cost} - Resultado: {spentGold} - Oro restante: {goldManager.currentGold}");

            if (!spentGold)
            {
                Debug.LogError("❌ No se pudo gastar el oro");
                return;
            }
        }
        else
        {
            Debug.LogError("❌ GoldManager no disponible");
            return;
        }

        // 3. Marcar item como comprado
        if (itemManager != null)
        {
            itemManager.PurchaseItem(item);
            Debug.Log($"✅ {item.itemName} marcado como comprado");
        }

        // 4. Agregar al inventario
        if (inventoryManager != null)
        {
            inventoryManager.AddItem(item);
            Debug.Log($"🎒 {item.itemName} agregado al inventario");
        }

        // 5. Aplicar estadísticas al jugador
        ApplyItemStatsToPlayer(item);

        // 6. Actualizar UI
        if (shopUI != null)
        {
            shopUI.RefreshUI();
            Debug.Log("🔄 UI actualizada después de compra");
        }

        Debug.Log($"✅ COMPRA EXITOSA: {item.itemName} comprado por {item.cost} oro");
    }

    private bool CanPurchaseItem(ItemData item)
    {
        if (item == null) return false;

        bool canPurchase = true;

        // Verificar si ya está comprado
        if (item.isPurchased)
        {
            Debug.Log($"❌ {item.itemName} ya está comprado");
            return false;
        }

        // Verificar con ItemManager
        if (itemManager != null)
        {
            canPurchase = itemManager.CanPurchaseItem(item);
            if (!canPurchase) Debug.Log($"❌ ItemManager dice que no se puede comprar {item.itemName}");
        }

        // Verificar oro
        if (goldManager != null)
        {
            bool hasEnoughGold = goldManager.currentGold >= item.cost;
            if (!hasEnoughGold) Debug.Log($"❌ Oro insuficiente: {goldManager.currentGold}/{item.cost}");
            canPurchase = canPurchase && hasEnoughGold;
        }

        Debug.Log($"🔍 {item.itemName} - Puede comprar: {canPurchase}");
        return canPurchase;
    }

    private void ApplyItemStatsToPlayer(ItemData item)
    {
        if (playerStats != null && item != null)
        {
            Debug.Log($"🎯 Aplicando estadísticas de {item.itemName} al jugador");

            // Aplicar bonos del item
            if (item.bonusDamage > 0)
            {
                playerStats.IncreaseDamage(item.bonusDamage);
                Debug.Log($"⚔️ Daño aumentado: +{item.bonusDamage}");
            }

            if (item.bonusArmor > 0)
            {
                playerStats.IncreaseArmor(item.bonusArmor);
                Debug.Log($"🛡️ Armadura aumentada: +{item.bonusArmor}");
            }

            if (item.bonusSpeed > 0)
            {
                playerStats.IncreaseSpeed(item.bonusSpeed);
                Debug.Log($"🏃 Velocidad aumentada: +{item.bonusSpeed}");
            }

            Debug.Log($"📊 Estadísticas finales - Daño: {playerStats.CurrentDamage}, Armadura: {playerStats.CurrentArmor}, Velocidad: {playerStats.CurrentSpeed}");
        }
        else
        {
            Debug.LogWarning("❌ No se pudo aplicar estadísticas: PlayerStats o Item nulo");
        }
    }

    public bool IsShopOpen() => isShopOpen;
}