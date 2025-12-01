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
        buyButton?.onClick.AddListener(OnBuyClicked);
        undoButton?.onClick.AddListener(OnUndoClicked);
        closeButton?.onClick.AddListener(CloseShop);

        GetManagerReferences();

        // Asignar ShopUI a los gráficos SIN FindObjectOfType
        if (attackGraphUI != null) attackGraphUI.shopUI = this;
        if (defenseGraphUI != null) defenseGraphUI.shopUI = this;
        if (speedGraphUI != null) speedGraphUI.shopUI = this;

        CloseShop();
    }

    private void Update()
    {
        UpdateGoldDisplay();
    }

    #region Métodos públicos
    public void OpenShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(true);
            ClearSelection();
            RefreshUI();
        }
    }

    public void CloseShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
            ClearSelection();
        }
    }

    public void RefreshUI()
    {
        RefreshAllGraphs();
        UpdateInfoPanel();
        UpdateGoldDisplay();
    }

    public void SelectItem(ItemData item)
    {
        selectedItem = item;

        ClearGraphSelections();

        UpdateInfoPanel();
    }
    #endregion

    #region UI Interna
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

        string status = selectedItem.isPurchased ? "<color=#00FF00> COMPRADO</color>" :
                       selectedItem.isUnlocked ? "<color=#FFFF00> DISPONIBLE</color>" :
                       "<color=#FF0000> BLOQUEADO</color>";

        infoText.text = $"<size=120%><b>{selectedItem.itemName}</b></size>\n" +
                       $"{status}\n\n" +
                       $"<color=#FFA500><b>Costo: {selectedItem.cost}G</b></color>\n\n" +
                       $"{selectedItem.description}\n\n";

        if (selectedItem.bonusDamage > 0)
            infoText.text += $"<color=#FF4444> Daño: +{selectedItem.bonusDamage}</color>\n";
        if (selectedItem.bonusArmor > 0)
            infoText.text += $"<color=#44FF44> Armadura: +{selectedItem.bonusArmor}</color>\n";
        if (selectedItem.bonusSpeed > 0)
            infoText.text += $"<color=#4444FF> Velocidad: +{selectedItem.bonusSpeed}</color>\n";

        if (selectedItem.requiredItems != null && selectedItem.requiredItems.Length > 0)
        {
            infoText.text += $"\n<color=#FFFF44><b> Requisitos:</b></color>\n";
            foreach (var req in selectedItem.requiredItems)
            {
                if (req != null)
                {
                    string reqStatus = req.isPurchased ? "<color=#00FF00>" : "<color=#FF0000>";
                    infoText.text += $"{reqStatus} {req.itemName}</color>\n";
                }
            }
        }

        UpdateBuyButtonState();
    }

    private void UpdateBuyButtonState()
    {
        if (buyButton == null) return;

        bool canBuy = CanPurchaseSelectedItem();
        bool alreadyPurchased = selectedItem != null && selectedItem.isPurchased;

        buyButton.interactable = canBuy && !alreadyPurchased;

        string buttonText = alreadyPurchased ? "COMPRADO" :
                           canBuy ? $"COMPRAR - {selectedItem.cost}G" : "NO DISPONIBLE";

        buyButton.GetComponentInChildren<TMP_Text>().text = buttonText;

        Image buttonImage = buyButton.GetComponent<Image>();
        if (buttonImage != null)
        {
            if (alreadyPurchased)
                buttonImage.color = Color.green;
            else if (canBuy)
                buttonImage.color = new Color(0.2f, 0.7f, 0.2f);
            else
                buttonImage.color = Color.gray;
        }
    }

    private bool CanPurchaseSelectedItem()
    {
        if (selectedItem == null) return false;

        if (itemManager != null && goldManager != null)
        {
            return itemManager.CanPurchaseItem(selectedItem) &&
                   goldManager.currentGold >= selectedItem.cost;
        }

        return false;
    }

    private void OnBuyClicked()
    {
        if (selectedItem == null) return;

        if (shopManager != null)
        {
            shopManager.PurchaseItem(selectedItem);
            RefreshUI();
        }
    }

    private void UpdateGoldDisplay()
    {
        if (goldText == null) return;

        int currentGold = goldManager != null ? goldManager.currentGold : 0;

        goldText.text = $" {currentGold}G";

        if (selectedItem != null && currentGold < selectedItem.cost && !selectedItem.isPurchased)
            goldText.color = Color.red;
        else
            goldText.color = Color.yellow;
    }

    private void OnUndoClicked()
    {
        // implement if needed
    }

    private void ClearSelection()
    {
        selectedItem = null;
        ClearGraphSelections();
        UpdateInfoPanel();
    }

    private void RefreshAllGraphs()
    {
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
        if (GameManager.Instance != null)
        {
            if (shopManager == null) shopManager = GameManager.Instance.GetShopManager();
            if (itemManager == null) itemManager = GameManager.Instance.GetItemManager();
            if (goldManager == null) goldManager = GameManager.Instance.GetGoldManager();
        }
    }
    #endregion

    #region Utilidad
    public bool IsShopOpen() => shopPanel != null && shopPanel.activeInHierarchy;

    public ItemData GetSelectedItem() => selectedItem;

    public void SetManagerReferences(ShopManager shopMgr, ItemManager itemMgr, GoldManager goldMgr)
    {
        shopManager = shopMgr;
        itemManager = itemMgr;
        goldManager = goldMgr;
    }
    #endregion
}
