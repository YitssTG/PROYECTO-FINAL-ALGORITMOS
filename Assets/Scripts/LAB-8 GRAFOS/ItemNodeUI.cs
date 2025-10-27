using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ItemNodeUI : MonoBehaviour, IPointerClickHandler
{
    public Image icon;
    public TMP_Text nameText;
    public TMP_Text costText;
    private Button button;

    private ItemData itemData;
    private ItemGraphUI graphUI;

    public void Initialize(ItemData data, ItemGraphUI graph)
    {
        itemData = data;
        graphUI = graph;

        if (button == null)
            button = GetComponent<Button>();

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => graphUI.SelectItem(itemData));
        }

        if (icon != null) icon.sprite = data.icon;
        if (nameText != null) nameText.text = data.itemName;
        if (costText != null) costText.text = $"{data.cost} oro";

        UpdateState();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (graphUI != null)
            graphUI.SelectItem(itemData);
    }

    public void UpdateState()
    {
        if (icon == null || itemData == null) return;

        if (itemData.isPurchased)
            icon.color = Color.green;
        else if (!itemData.isUnlocked)
            icon.color = Color.gray;
        else
            icon.color = Color.white;
    }
}
