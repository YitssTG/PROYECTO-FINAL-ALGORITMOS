using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Graph/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Datos del Item")]
    public string itemName;
    public Sprite icon;
    public int cost;
    [TextArea] public string description;

    [Header("Bonos")]
    public float bonusDamage;
    public float bonusArmor;
    public float bonusSpeed;

    [Header("Dependencias")]
    public ItemData[] requiredItems;

    [HideInInspector] public bool isUnlocked;
    [HideInInspector] public bool isPurchased;
}
