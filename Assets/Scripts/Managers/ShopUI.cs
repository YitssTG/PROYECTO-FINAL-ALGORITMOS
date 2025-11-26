using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUI : MonoBehaviour
{
    [Header("UI Elements - Solo componentes visuales")]
    public GameObject shopPanel;
    public TMP_Text infoText;
    public Button buyButton;
    public Button undoButton;
    public Button closeButton;

    [Header("Referencias a Graph UIs")]
    public ItemGraphUI attackGraphUI;
    public ItemGraphUI defenseGraphUI;
    public ItemGraphUI speedGraphUI;

    [Header("Estado del Oro")]
    public TMP_Text goldText;

    [Header("Referencias a Managers")]
    public ShopManager shopManager;
    public ItemManager itemManager;
    public GoldManager goldManager;

    private ItemData selectedItem;

    private void Start()
    {
        // Solo configuración UI
        buyButton?.onClick.AddListener(OnBuyClicked);
        undoButton?.onClick.AddListener(OnUndoClicked);
        closeButton?.onClick.AddListener(CloseShop);

        // Obtener referencias de managers
        GetManagerReferences();

        CloseShop();
    }

    private void Update()
    {
        // Actualizar oro en tiempo real
        UpdateGoldDisplay();
    }

    #region Métodos públicos para que GameManager/ShopManager los llamen
    public void OpenShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(true);
            ClearSelection();
            RefreshUI();
            Debug.Log("🛒 ShopUI: Tienda abierta");
        }
    }

    public void CloseShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
            ClearSelection();
            Debug.Log("🛒 ShopUI: Tienda cerrada");
        }
    }

    public void RefreshUI()
    {
        // Actualizar todos los gráficos
        RefreshAllGraphs();

        // Actualizar panel de información
        UpdateInfoPanel();

        // Actualizar display de oro
        UpdateGoldDisplay();

        Debug.Log("🛒 ShopUI: Interfaz actualizada");
    }

    public void SelectItem(ItemData item)
    {
        selectedItem = item;

        // Deseleccionar en todos los gráficos
        ClearGraphSelections();

        UpdateInfoPanel();

        Debug.Log($"🛒 ShopUI: Item seleccionado - {item?.itemName}");
    }
    #endregion

    #region 🔒 Métodos privados - solo lógica de UI
    private void UpdateInfoPanel()
    {
        if (infoText == null) return;

        if (selectedItem == null)
        {
            infoText.text = "Selecciona un ítem para ver detalles.";
            if (buyButton != null)
            {
                buyButton.interactable = false;
                buyButton.GetComponentInChildren<TMP_Text>().text = "COMPRAR";
            }
            return;
        }

        // Mostrar información detallada del item con formato mejorado
        string status = selectedItem.isPurchased ? "<color=#00FF00>✅ COMPRADO</color>" :
                       selectedItem.isUnlocked ? "<color=#FFFF00>🔓 DISPONIBLE</color>" : "<color=#FF0000>🔒 BLOQUEADO</color>";

        infoText.text = $"<size=120%><b>{selectedItem.itemName}</b></size>\n" +
                       $"{status}\n\n" +
                       $"<color=#FFA500><b>Costo: {selectedItem.cost}G</b></color>\n\n" +
                       $"{selectedItem.description}\n\n";

        // Mostrar bonos con colores
        if (selectedItem.bonusDamage > 0)
            infoText.text += $"<color=#FF4444>⚔️ Daño: +{selectedItem.bonusDamage}</color>\n";
        if (selectedItem.bonusArmor > 0)
            infoText.text += $"<color=#44FF44>🛡️ Armadura: +{selectedItem.bonusArmor}</color>\n";
        if (selectedItem.bonusSpeed > 0)
            infoText.text += $"<color=#4444FF>🏃 Velocidad: +{selectedItem.bonusSpeed}</color>\n";

        // Mostrar requerimientos si existen
        if (selectedItem.requiredItems != null && selectedItem.requiredItems.Length > 0)
        {
            infoText.text += $"\n<color=#FFFF44><b>🔗 Requisitos:</b></color>\n";
            foreach (var req in selectedItem.requiredItems)
            {
                if (req != null)
                {
                    string reqStatus = req.isPurchased ? "<color=#00FF00>✅" : "<color=#FF0000>❌";
                    infoText.text += $"{reqStatus} {req.itemName}</color>\n";
                }
            }
        }

        // Actualizar estado del botón de compra
        UpdateBuyButtonState();
    }

    private void UpdateBuyButtonState()
    {
        if (buyButton == null) return;

        bool canBuy = CanPurchaseSelectedItem();
        bool alreadyPurchased = selectedItem != null && selectedItem.isPurchased;

        buyButton.interactable = canBuy && !alreadyPurchased;

        string buttonText = alreadyPurchased ? "✅ COMPRADO" :
                           canBuy ? $"COMPRAR - {selectedItem.cost}G" : "NO DISPONIBLE";

        buyButton.GetComponentInChildren<TMP_Text>().text = buttonText;

        // Cambiar color del botón según estado
        Image buttonImage = buyButton.GetComponent<Image>();
        if (buttonImage != null)
        {
            if (alreadyPurchased)
                buttonImage.color = Color.green;
            else if (canBuy)
                buttonImage.color = new Color(0.2f, 0.7f, 0.2f); // Verde oscuro
            else
                buttonImage.color = Color.gray;
        }
    }

    private bool CanPurchaseSelectedItem()
    {
        if (selectedItem == null) return false;

        // Verificar con managers locales primero
        if (itemManager != null && goldManager != null)
        {
            bool canPurchase = itemManager.CanPurchaseItem(selectedItem) &&
                              goldManager.currentGold >= selectedItem.cost;
            return canPurchase;
        }

        // Fallback: verificar con GameManager
        if (GameManager.Instance != null)
        {
            return GameManager.Instance.CanPurchaseItem(selectedItem);
        }

        return false;
    }

    private void OnBuyClicked()
    {
        if (selectedItem != null)
        {
            Debug.Log($"🛒 ShopUI: Intentando comprar {selectedItem.itemName} por {selectedItem.cost}G");
            Debug.Log($"🛒 Estado actual - Comprado: {selectedItem.isPurchased}, Oro: {goldManager?.currentGold}");

            // Verificar estado actual ANTES de comprar
            if (selectedItem.isPurchased)
            {
                Debug.LogWarning("⚠️ Item ya comprado, no se puede comprar de nuevo");
                return;
            }

            if (goldManager != null && goldManager.currentGold < selectedItem.cost)
            {
                Debug.LogWarning($"⚠️ Oro insuficiente: {goldManager.currentGold}/{selectedItem.cost}");
                return;
            }

            // Intentar con shopManager local primero
            if (shopManager != null)
            {
                shopManager.PurchaseItem(selectedItem);

                // Actualizar UI inmediatamente después de la compra
                RefreshUI();

                // Verificar estado DESPUÉS de la compra
                Debug.Log($"🛒 Estado después - Comprado: {selectedItem.isPurchased}, Oro: {goldManager?.currentGold}");
            }
            else
            {
                Debug.LogError("❌ No hay ShopManager disponible");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ No hay item seleccionado para comprar");
        }
    }

    private void UpdateGoldDisplay()
    {
        if (goldText == null) return;

        int currentGold = 0;

        // Obtener oro del manager local primero
        if (goldManager != null)
        {
            currentGold = goldManager.currentGold;
        }

        goldText.text = $"💰 {currentGold}G";

        // Cambiar color si no hay suficiente oro para el item seleccionado
        if (selectedItem != null && currentGold < selectedItem.cost && !selectedItem.isPurchased)
        {
            goldText.color = Color.red;
        }
        else
        {
            goldText.color = Color.yellow;
        }
    }


    private void OnUndoClicked()
    {
        // Lógica de UI para undo (opcional)
        Debug.Log("↩️ ShopUI: Undo clicked");

        // Podrías implementar un sistema de deshacer última compra aquí
        // if (shopManager != null) shopManager.UndoLastPurchase();
    }

    private void ClearSelection()
    {
        selectedItem = null;
        ClearGraphSelections();
        UpdateInfoPanel();
    }

    private void RefreshAllGraphs()
    {
        // Actualizar gráficos usando referencias directas
        attackGraphUI?.RefreshGraph();
        defenseGraphUI?.RefreshGraph();
        speedGraphUI?.RefreshGraph();
    }

    private void ClearGraphSelections()
    {
        attackGraphUI?.ClearSelection();
        defenseGraphUI?.ClearSelection();
        speedGraphUI?.ClearSelection();
    }

    private void GetManagerReferences()
    {
        // Obtener referencias a través de GameManager si está disponible
        if (GameManager.Instance != null)
        {
            if (shopManager == null) shopManager = GameManager.Instance.GetShopManager();
            if (itemManager == null) itemManager = GameManager.Instance.GetItemManager();
            if (goldManager == null) goldManager = GameManager.Instance.GetGoldManager();
        }

        // Log de referencias obtenidas
        if (shopManager != null && itemManager != null && goldManager != null)
        {
            Debug.Log("✅ ShopUI: Todas las referencias de managers obtenidas");
        }
        else
        {
            Debug.LogWarning("⚠️ ShopUI: Algunas referencias de managers no están disponibles");
        }
    }
    #endregion

    #region Métodos de utilidad para otros scripts
    public bool IsShopOpen() => shopPanel != null && shopPanel.activeInHierarchy;

    public ItemData GetSelectedItem() => selectedItem;

    // Método para asignar referencias externamente
    public void SetManagerReferences(ShopManager shopMgr, ItemManager itemMgr, GoldManager goldMgr)
    {
        shopManager = shopMgr;
        itemManager = itemMgr;
        goldManager = goldMgr;
        Debug.Log("🔗 ShopUI: Referencias asignadas externamente");
    }
    #endregion
}