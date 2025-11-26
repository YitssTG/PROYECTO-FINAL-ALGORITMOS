using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ItemNodeUI : MonoBehaviour, IPointerClickHandler
{
    [Header("Componentes UI")]
    public Image icon;
    public TMP_Text nameText;
    public TMP_Text costText;
    public Image background;

    [Header("Colores de estado")]
    public Color purchasedColor = Color.green;
    public Color lockedColor = Color.gray;
    public Color availableColor = Color.white;
    public Color selectedColor = Color.yellow;

    private ItemData itemData;
    private ItemGraphUI graphUI;
    private bool isSelected = false;

    public void Initialize(ItemData data, ItemGraphUI graph)
    {
        itemData = data;
        graphUI = graph;

        // Configurar UI
        if (icon != null && data.icon != null)
            icon.sprite = data.icon;

        if (nameText != null)
            nameText.text = data.itemName;

        if (costText != null)
            costText.text = $"{data.cost}G";

        UpdateAppearance();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        graphUI?.OnItemSelected(itemData);
        SetSelected(true);
    }

    public void UpdateAppearance()
    {
        if (itemData == null) return;

        Color targetColor = availableColor;

        if (itemData.isPurchased)
            targetColor = purchasedColor;
        else if (!itemData.isUnlocked)
            targetColor = lockedColor;

        if (background != null)
        {
            background.color = isSelected ? selectedColor : targetColor;
        }

        if (icon != null)
        {
            icon.color = itemData.isPurchased ? purchasedColor :
                        !itemData.isUnlocked ? lockedColor : availableColor;
        }
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateAppearance();
    }
}