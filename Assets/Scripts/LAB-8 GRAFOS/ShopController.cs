using UnityEngine;
using UnityEngine.InputSystem;

public class ShopController : MonoBehaviour
{
    [Header("Panel de tienda")]
    public GameObject shopPanel;

    [Header("Referencias de árboles")]
    public ItemGraphUI attackGraph;
    public ItemGraphUI defenseGraph;
    public ItemGraphUI speedGraph;

    private bool isOpen;

    void Start()
    {
        shopPanel.SetActive(false);
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
}
