using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathfinding
{
    private Grid grid;

    public EnemyPathfinding(Grid gridClass)
    {
        grid = gridClass;
    }

    class Node
    {
        public Vector2Int pos;
        public int gCost;
        public int hCost;
        public int fCost => gCost + hCost;
        public Node parent;

        public Node(Vector2Int position)
        {
            pos = position;
        }
    }

    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int finish)
    {
        List<Node> openList = new List<Node>();
        HashSet<Vector2Int> closedList = new HashSet<Vector2Int>();

        Node startNode = new Node(start);
        startNode.gCost = 0;
        startNode.hCost = Heuristic(start, finish);

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            Node currentNode = openList[0];

            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].fCost < currentNode.fCost)
                    currentNode = openList[i];
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode.pos);

            if (currentNode.pos == finish)
                return ReconstructPath(currentNode);

            foreach (Vector2Int adj in GetAdjacent(currentNode.pos))
            {
                if (closedList.Contains(adj))
                    continue;

                if (!IsInsideGrid(adj))
                    continue;

                if (IsObstacle(adj))
                    continue;

                int newCost = currentNode.gCost + 1;

                Node existing = openList.Find(n => n.pos == adj);

                if (existing == null)
                {
                    Node node = new Node(adj);
                    node.gCost = newCost;
                    node.hCost = Heuristic(adj, finish);
                    node.parent = currentNode;

                    openList.Add(node);
                }
                else if (newCost < existing.gCost)
                {
                    existing.gCost = newCost;
                    existing.parent = currentNode;
                }
            }
        }

        return null;
    }

    private bool IsInsideGrid(Vector2Int pos)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.x < grid.layerWidth && pos.y < grid.layerHeight;
    }

    private bool IsObstacle(Vector2Int pos)
    {
        Space space = grid.GetSpace(pos.x, pos.y);
        if (space.containType == occupier.Blocked) return true;
        return false;
    }

    private List<Vector2Int> ReconstructPath(Node end)
    {
        List<Vector2Int> path = new List<Vector2Int>();

        Node currentNode = end;

        while (currentNode != null)
        {
            path.Add(currentNode.pos);
            currentNode = currentNode.parent;
        }

        path.Reverse();

        if (path.Count > 0)
            path.RemoveAt(0);

        return path;
    }

    private int Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private IEnumerable<Vector2Int> GetAdjacent(Vector2Int p)
    {
        yield return new Vector2Int(p.x + 1, p.y);
        yield return new Vector2Int(p.x - 1, p.y);
        yield return new Vector2Int(p.x, p.y + 1);
        yield return new Vector2Int(p.x, p.y - 1);
    }
}
