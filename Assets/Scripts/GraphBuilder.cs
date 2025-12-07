using System.Collections.Generic;
using UnityEngine;

public static class GraphBuilder
{
    public static OrientedGraph<ItemData> BuildGraph(ItemGraphData data)
    {
        OrientedGraph<ItemData> graph = new OrientedGraph<ItemData>();
        Dictionary<ItemData, Node<ItemData>> lookup = new Dictionary<ItemData, Node<ItemData>>();

        if (data == null) return graph;

        List<ItemData> items = data.GetCleanItems();

        // Crear nodos
        for (int i = 0; i < items.Count; i++)
        {
            ItemData item = items[i];
            if (item == null) continue;

            Node<ItemData> node = graph.AddNode(item);
            lookup[item] = node;
        }

        // Crear conexiones basadas en "requiredItems"
        for (int i = 0; i < items.Count; i++)
        {
            ItemData to = items[i];
            if (to == null) continue;
            if (to.requiredItems == null) continue;

            for (int r = 0; r < to.requiredItems.Length; r++)
            {
                ItemData from = to.requiredItems[r];
                if (from == null) continue;

                if (lookup.ContainsKey(from) && lookup.ContainsKey(to))
                {
                    graph.AddEdge(lookup[from], lookup[to]);
                }
            }
        }

        return graph;
    }
}
