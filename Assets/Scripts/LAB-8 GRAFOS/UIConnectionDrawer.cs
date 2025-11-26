using UnityEngine;

public class UIConnectionDrawer : MonoBehaviour
{
    [Header("Referencias")]
    public UILineDrawer linePrefab;

    public void DrawConnections()
    {
        // Limpiar conexiones anteriores
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        if (linePrefab == null) return;

        ItemGraphUI graphUI = GetComponentInParent<ItemGraphUI>();
        if (graphUI == null) return;

        // Dibujar conexiones entre nodos
        foreach (var item in graphUI.nodeLookup.Keys)
        {
            if (item.requiredItems == null) continue;

            foreach (var req in item.requiredItems)
            {
                if (graphUI.nodeLookup.ContainsKey(req) && graphUI.nodeLookup.ContainsKey(item))
                {
                    DrawConnection(graphUI.nodeLookup[req], graphUI.nodeLookup[item], item);
                }
            }
        }
    }

    private void DrawConnection(ItemNodeUI from, ItemNodeUI to, ItemData item)
    {
        RectTransform fromRT = from.GetComponent<RectTransform>();
        RectTransform toRT = to.GetComponent<RectTransform>();

        UILineDrawer line = Instantiate(linePrefab, transform);
        line.start = fromRT.anchoredPosition;
        line.end = toRT.anchoredPosition;
        line.thickness = 4f;
        line.lineColor = item.isUnlocked ? Color.yellow : Color.gray;
    }
}