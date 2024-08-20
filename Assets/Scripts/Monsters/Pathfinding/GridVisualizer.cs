using System.Collections.Generic;
using UnityEngine;

public class GridVisualizer : MonoBehaviour
{
    public Grid grid;

    private void OnDrawGizmos()
    {
        if (grid == null)
        {
            Debug.LogWarning("Grid is not assigned to GridVisualizer.");
            return;
        }

        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                Node node = grid.Grid1[x, y];
                Gizmos.color = node.isWalkable ? Color.white : Color.red;
                Gizmos.DrawWireCube(node.worldPosition, Vector3.one * grid.CellSize);
            }
        }
    }
}