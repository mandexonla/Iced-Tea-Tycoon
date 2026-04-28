using UnityEngine;

public class GridRenderer : MonoBehaviour
{
    [SerializeField] private float gridSize = 1f;
    [SerializeField] private int gridWidth = 20;
    [SerializeField] private int gridHeight = 15;
    [SerializeField] private Color lineColor = new Color(1f, 1f, 1f, 0.1f);

    private void OnDrawGizmos()
    {
        Gizmos.color = lineColor;

        float halfW = gridWidth * gridSize * 0.5f;
        float halfH = gridHeight * gridSize * 0.5f;

        for (int x = 0; x <= gridWidth; x++)
        {
            float xPos = -halfW + x * gridSize;
            Gizmos.DrawLine(new Vector3(xPos, -halfH, 0), new Vector3(xPos, halfH, 0));
        }

        for (int y = 0; y <= gridHeight; y++)
        {
            float yPos = -halfH + y * gridSize;
            Gizmos.DrawLine(new Vector3(-halfW, yPos, 0), new Vector3(halfW, yPos, 0));
        }
    }
}
