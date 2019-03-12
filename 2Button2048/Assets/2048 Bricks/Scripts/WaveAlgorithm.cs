using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class WaveAlgorithm
{
    class PathNode
    {
        public Vector2Int coords;
        public PathNode previous;

        public PathNode(Vector2Int coords, PathNode previous)
        {
            this.coords = coords;
            this.previous = previous;
        }
    }

    public static List<Vector2Int> GetArea<T>(T[,] field, Vector2Int start, Predicate<T> predicate)
    {
        List<Vector2Int> area = new List<Vector2Int> {start};

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            Vector2Int coords = queue.Dequeue();


            foreach (Vector2Int c in GetAdjacentOnes(field, coords))
                if (!area.Contains(c) && predicate(field[c.x, c.y]))
                {
                    queue.Enqueue(c);
                    area.Add(c);
                }
        }

        return area;
    }

    public static List<Vector2Int> GetPath<T>(T[,] field, Vector2Int start, Vector2Int target, Predicate<T> predicate)
    {
        List<Vector2Int> path = new List<Vector2Int>();

        List<PathNode> linkedPath = new List<PathNode> {new PathNode(start, null)};

        Queue<PathNode> queue = new Queue<PathNode>();
        queue.Enqueue(linkedPath[0]);

        while (queue.Count > 0)
        {
            PathNode node = queue.Dequeue();

            if (node.coords == target)
            {
                while (node.previous != null)
                {
                    path.Add(node.coords);
                    node = node.previous;
                }

                path.Add(node.coords);
                path.Reverse();

                return path;
            }

            foreach (Vector2Int c in GetAdjacentOnes(field, node.coords))
                if (linkedPath.All(n => n.coords != c) && predicate(field[c.x, c.y]))
                {
                    linkedPath.Add(new PathNode(c, node));
                    queue.Enqueue(linkedPath[linkedPath.Count - 1]);
                }
        }

        return path;
    }

    static List<Vector2Int> GetAdjacentOnes<T>(T[,] field, Vector2Int coords)
    {
        List<Vector2Int> adjacent = new List<Vector2Int>();

        Vector2Int up = new Vector2Int(coords.x, coords.y + 1);
        if (up.y < field.GetLength(1))
            adjacent.Add(up);

        Vector2Int down = new Vector2Int(coords.x, coords.y - 1);
        if (down.y >= 0)
            adjacent.Add(down);

        Vector2Int left = new Vector2Int(coords.x - 1, coords.y);
        if (left.x >= 0)
            adjacent.Add(left);

        Vector2Int right = new Vector2Int(coords.x + 1, coords.y);
        if (right.x < field.GetLength(0))
            adjacent.Add(right);

        return adjacent;
    }
}