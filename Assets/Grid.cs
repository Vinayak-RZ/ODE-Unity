using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class GridRenderer : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int gridSize = 10;       // Number of units in +x/-x and +y/-y
    [SerializeField] private float spacing = 1f;      // Spacing between lines
    [SerializeField] private Color gridColor = Color.gray;
    [SerializeField] private float lineWidth = 0.02f;

    private void Start()
    {
        DrawGrid();
    }

    private void DrawGrid()
    {
        // Parent container for all lines
        GameObject parent = new GameObject("GridLines");
        parent.transform.SetParent(transform, false);

        // Draw vertical lines
        for (int x = -gridSize; x <= gridSize; x++)
        {
            CreateLine(
                new Vector3(x * spacing, -gridSize * spacing, 0),
                new Vector3(x * spacing, gridSize * spacing, 0),
                parent.transform
            );
        }

        // Draw horizontal lines
        for (int y = -gridSize; y <= gridSize; y++)
        {
            CreateLine(
                new Vector3(-gridSize * spacing, y * spacing, 0),
                new Vector3(gridSize * spacing, y * spacing, 0),
                parent.transform
            );
        }
    }

    private void CreateLine(Vector3 start, Vector3 end, Transform parent)
    {
        GameObject lineObj = new GameObject("GridLine");
        lineObj.transform.SetParent(parent, false);

        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);

        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.material = new Material(Shader.Find("Sprites/Default")); // simple unlit shader
        lr.startColor = gridColor;
        lr.endColor = gridColor;
        lr.sortingOrder = -1; // draw behind other lines
    }
}
