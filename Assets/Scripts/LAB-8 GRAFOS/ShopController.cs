using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ShopController : MonoBehaviour
{
    [Header("Panel de tienda")]
    public GameObject shopPanel;
    public TMP_Text goldText;

    [Header("Referencias de árboles")]
    public ItemGraphUI attackGraph;
    public ItemGraphUI defenseGraph;
    public ItemGraphUI speedGraph;

    private bool isOpen;
    private GoldManager goldManager;

    void Start()
    {
        // ✅ Usamos el Singleton, no FindObjectOfType
        goldManager = GoldManager.Instance;

        if (goldManager != null)
            goldManager.OnGoldChanged.AddListener(UpdateGold);
        else
            Debug.LogError("❌ GoldManager no encontrado en la escena.");

        shopPanel.SetActive(false);

        if (goldManager != null)
            UpdateGold(goldManager.currentGold);
    }

    public void OnToggleShop(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        isOpen = !isOpen;
        shopPanel.SetActive(isOpen);

        if (isOpen)
        {
            // 🧹 Limpia selección al abrir
            ItemGraphUI.ActiveGraph = null;
            if (attackGraph != null) attackGraph.ClearSelection();
            if (defenseGraph != null) defenseGraph.ClearSelection();
            if (speedGraph != null) speedGraph.ClearSelection();
        }
    }

    private void UpdateGold(int gold)
    {
        if (goldText != null)
            goldText.text = $"Oro: {gold}";
    }
}
