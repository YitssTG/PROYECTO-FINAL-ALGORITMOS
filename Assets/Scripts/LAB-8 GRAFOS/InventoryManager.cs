using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public Transform inventoryContainer;
    public GameObject itemNodePrefab;
    private List<GameObject> currentItems = new();

    public void AddItem(ItemData data)
    {
        GameObject newItem = Instantiate(itemNodePrefab, inventoryContainer);
        newItem.GetComponentInChildren<TMP_Text>().text = data.itemName;
        currentItems.Add(newItem);
    }

    public void RemoveItem(ItemData data)
    {
        GameObject obj = currentItems.Find(o => o.GetComponentInChildren<TMP_Text>().text == data.itemName);
        if (obj != null)
        {
            currentItems.Remove(obj);
            Destroy(obj);
        }
    }
}
