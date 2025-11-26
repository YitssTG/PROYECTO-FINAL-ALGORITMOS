using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("Referencias UI")]
    public Transform inventoryContainer;
    public GameObject itemNodePrefab;

    private List<GameObject> currentItems = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Initialize()
    {
        // Limpiar inventario al inicializar
        ClearInventory();
        Debug.Log("✅ InventoryManager inicializado");
    }

    public void AddItem(ItemData data)
    {
        if (data == null || inventoryContainer == null || itemNodePrefab == null) return;

        GameObject newItem = Instantiate(itemNodePrefab, inventoryContainer);
        TMP_Text text = newItem.GetComponentInChildren<TMP_Text>();
        if (text != null) text.text = data.itemName;

        currentItems.Add(newItem);

        // Aplicar stats del item al jugador
        ApplyItemToPlayer(data);
    }

    private void ApplyItemToPlayer(ItemData item)
    {
        PlayerStats playerStats = FindObjectOfType<PlayerStats>();
        if (playerStats != null)
        {
            playerStats.ApplyItemStats(item);
        }
    }

    public void RemoveItem(ItemData data)
    {
        if (data == null) return;

        GameObject obj = currentItems.Find(o =>
            o.GetComponentInChildren<TMP_Text>()?.text == data.itemName);

        if (obj != null)
        {
            currentItems.Remove(obj);
            Destroy(obj);

            // Remover stats del item del jugador
            RemoveItemFromPlayer(data);
        }
    }

    private void RemoveItemFromPlayer(ItemData item)
    {
        PlayerStats playerStats = FindObjectOfType<PlayerStats>();
        if (playerStats != null)
        {
            playerStats.RemoveItemStats(item);
        }
    }

    public void ClearInventory()
    {
        foreach (var item in currentItems)
        {
            Destroy(item);
        }
        currentItems.Clear();
        Debug.Log("🎒 Inventario limpiado");
    }
}