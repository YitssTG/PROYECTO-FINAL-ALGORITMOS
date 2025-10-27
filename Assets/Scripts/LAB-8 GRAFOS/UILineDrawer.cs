using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class UILineDrawer : MaskableGraphic
{
    public Vector2 start;
    public Vector2 end;
    public float thickness = 3f;
    public Color lineColor = Color.white;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        Vector2 direction = (end - start).normalized;
        Vector2 normal = new Vector2(-direction.y, direction.x) * thickness / 2f;

        UIVertex v = UIVertex.simpleVert;
        v.color = lineColor;

        v.position = start - normal; vh.AddVert(v);
        v.position = start + normal; vh.AddVert(v);
        v.position = end + normal; vh.AddVert(v);
        v.position = end - normal; vh.AddVert(v);

        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(2, 3, 0);
    }
}
