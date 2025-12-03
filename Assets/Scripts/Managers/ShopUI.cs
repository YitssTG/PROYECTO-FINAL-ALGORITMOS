using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUI : MonoBehaviour
{
    public GameObject shopPanel;
    public TMP_Text infoText;
    public TMP_Text goldText;

    public Button buyButton;
    public Button undoButton;
    //public Button closeButton;

    public ItemGraphUI attackGraphUI;
    public ItemGraphUI defenseGraphUI;
    public ItemGraphUI speedGraphUI;

    public ShopManager shopManager;
    public ItemManager itemManager;
    public GoldManager goldManager;

    private ItemData selectedItem;

    private void Start()
    {
        buyButton.onClick.AddListener(OnBuyClicked);
        undoButton.onClick.AddListener(OnUndoClicked);
        //closeButton.onClick.AddListener(CloseShop);

        ResolveManagers();
        AssignGraphReferences();

        CloseShop();
    }

    private void Update()
    {
        UpdateGoldDisplay();
    }

    public void OpenShop()
    {
        shopPanel.SetActive(true);
        ClearSelection();
        RefreshUI();
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
        ClearSelection();
    }

    public void SelectItem(ItemData item)
    {
        selectedItem = item;
        ClearGraphSelections();
        UpdateInfoPanel();
    }


    private void UpdateInfoPanel()
    {
        if (infoText == null) return;

        if (selectedItem == null)
        {
            ShowEmptyInfo();
            return;
        }

        infoText.text = shopManager.GetItemInfo(selectedItem);
        bool purchased = selectedItem.isPurchased;
        bool canBuy = shopManager.CanPurchase(selectedItem);

        if (purchased)
            UpdateBuyButton(false, "COMPRADO");
        else if (canBuy)
            UpdateBuyButton(true, "COMPRAR " + selectedItem.cost + "G");
        else
            UpdateBuyButton(false, "NO DISPONIBLE");
    }

    private void ShowEmptyInfo()
    {
        infoText.text = "Selecciona un ítem para ver detalles.";
        UpdateBuyButton(false, "COMPRAR");
    }

    private void UpdateBuyButton(bool interactable, string text)
    {
        buyButton.interactable = interactable;
        buyButton.GetComponentInChildren<TMP_Text>().text = text;
    }

    private void OnBuyClicked()
    {
        if (selectedItem == null) return;

        if (shopManager.PurchaseItem(selectedItem))
            RefreshUI();
    }

    private void OnUndoClicked()
    {
    }


    public void RefreshUI()
    {
        RefreshAllGraphs();
        UpdateInfoPanel();
        UpdateGoldDisplay();
    }

    private void RefreshAllGraphs()
    {
        attackGraphUI?.RefreshGraph();
        defenseGraphUI?.RefreshGraph();
        speedGraphUI?.RefreshGraph();
    }

    private void UpdateGoldDisplay()
    {
        if (goldText == null) return;

        int g = goldManager != null ? goldManager.currentGold : 0;
        goldText.text = g + "G";
    }

    private void ClearSelection()
    {
        selectedItem = null;
        ClearGraphSelections();
        UpdateInfoPanel();
    }

    private void ClearGraphSelections()
    {
        attackGraphUI?.ClearSelection();
        defenseGraphUI?.ClearSelection();
        speedGraphUI?.ClearSelection();
    }

    private void ResolveManagers()
    {
        if (GameManager.Instance != null)
        {
            if (shopManager == null) shopManager = GameManager.Instance.GetShopManager();
            if (itemManager == null) itemManager = GameManager.Instance.GetItemManager();
            if (goldManager == null) goldManager = GameManager.Instance.GetGoldManager();
        }
    }

    private void AssignGraphReferences()
    {
        if (attackGraphUI != null) attackGraphUI.shopUI = this;
        if (defenseGraphUI != null) defenseGraphUI.shopUI = this;
        if (speedGraphUI != null) speedGraphUI.shopUI = this;
    }
}
