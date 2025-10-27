using UnityEngine;

public class UIConnectionDrawer : MonoBehaviour
{
    public ItemGraphUI graphUI;
    public UILineDrawer linePrefab;

    public void DrawConnections()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        if (graphUI == null || linePrefab == null) return;

        foreach (var item in graphUI.allItems)
        {
            if (item.requiredItems == null) continue;

            foreach (var req in item.requiredItems)
            {
                if (!graphUI.nodeLookup.ContainsKey(item) || !graphUI.nodeLookup.ContainsKey(req))
                    continue;

                RectTransform from = graphUI.nodeLookup[req].GetComponent<RectTransform>();
                RectTransform to = graphUI.nodeLookup[item].GetComponent<RectTransform>();

                UILineDrawer line = Instantiate(linePrefab, transform);
                line.start = from.anchoredPosition;
                line.end = to.anchoredPosition;
                line.thickness = 4f;
                line.lineColor = item.isUnlocked ? Color.yellow : Color.gray;
            }
        }
    }
}
