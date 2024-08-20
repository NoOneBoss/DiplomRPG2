using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    private int width;
    private int height;
    private float cellSize;
    private Vector3 origin;
    private LayerMask obstacleLayer;
    private Node[,] grid;

    public Grid(int width, int height, float cellSize, Vector3 origin, LayerMask obstacleLayer)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.origin = origin;
        this.obstacleLayer = obstacleLayer;

        grid = new Node[width, height];

        CreateGrid();
    }

    public int Width => width;
    public int Height => height;
    public float CellSize => cellSize;
    public Node[,] Grid1 => grid;

    public void UpdateGrid(Vector3 newOrigin)
    {
        origin = newOrigin;
        CreateGrid();
    }

    private void CreateGrid()
    {
        Vector3 worldBottomLeft = origin - Vector3.right * (width * cellSize) / 2 - Vector3.up * (height * cellSize) / 2;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * cellSize + cellSize / 2) + Vector3.up * (y * cellSize + cellSize / 2);
                bool isObstacle = Physics2D.OverlapCircle(worldPoint, cellSize / 2, obstacleLayer) == null;
                grid[x, y] = new Node(isObstacle, worldPoint, x, y);
            }
        }
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        Vector3 localPosition = worldPosition - origin;
        int x = Mathf.Clamp(Mathf.FloorToInt(localPosition.x / cellSize + width / 2), 0, width - 1);
        int y = Mathf.Clamp(Mathf.FloorToInt(localPosition.y / cellSize + height / 2), 0, height - 1);
        return grid[x, y];
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0)
                    continue;

                int checkX = node.gridX + dx;
                int checkY = node.gridY + dy;

                if (checkX >= 0 && checkX < width && checkY >= 0 && checkY < height)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }
}