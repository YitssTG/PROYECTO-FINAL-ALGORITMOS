using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItemGraph", menuName = "Graph/Item Graph")]
public class ItemGraphData : ScriptableObject
{
    public string graphName;
    [Header("Items de este árbol")]
    public List<ItemData> items = new List<ItemData>();

    // Devuelve una lista limpia (sin nulls ni duplicados)
    public List<ItemData> GetCleanItems()
    {
        HashSet<ItemData> unique = new HashSet<ItemData>();
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] != null) unique.Add(items[i]);
        }
        return new List<ItemData>(unique);
    }
}
