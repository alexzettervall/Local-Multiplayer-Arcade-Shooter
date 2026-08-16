using System.Collections.Generic;
using UnityEngine;

public static class AStar
{
    public static List<Vector2> FindPath(NavGraph navGraph, Waypoint start, Waypoint goal)
    {
        if (start == null) return new List<Vector2>();

        var open = new List<PathNode>();
        var closed = new HashSet<Waypoint>();

        PathNode startNode = new PathNode(start, null, 0f, Heuristic(start, goal));
        open.Add(startNode);

        while (open.Count > 0)
        {
            // Get node with lowest F = G + H
            open.Sort((a, b) => a.F.CompareTo(b.F));
            PathNode current = open[0];
            open.RemoveAt(0);

            if (current.waypoint == goal)
                return ReconstructPath(current);

            closed.Add(current.waypoint);

            Debug.Log(current.waypoint.connections);
            foreach (var connection in current.waypoint.connections)
            {
                Waypoint neighbor = navGraph.GetWaypoint(connection.GetOther(current.waypoint));

                if (closed.Contains(neighbor)) continue;

                float gScore = current.G + Vector2.Distance(current.waypoint.position, neighbor.position);

                PathNode existing = open.Find(n => n.waypoint == neighbor);
                if (existing != null)
                {
                    if (gScore < existing.G)
                    {
                        existing.G = gScore;
                        existing.Parent = current;
                    }
                }
                else
                {
                    open.Add(new PathNode(neighbor, current, gScore, Heuristic(neighbor, goal)));
                }
            }
        }

        // No path found
        return new List<Vector2>();
    }

    private static float Heuristic(Waypoint a, Waypoint b)
    {
        return Vector2.Distance(a.position, b.position);
    }

    private static List<Vector2> ReconstructPath(PathNode node)
    {
        List<Vector2> path = new List<Vector2>();
        while (node != null)
        {
            path.Insert(0, node.waypoint.position); // Add randomness
            node = node.Parent;
        }
        return path;
    }

    private class PathNode
    {
        public Waypoint waypoint;
        public PathNode Parent;
        public float G; // cost from start
        public float H; // heuristic to goal
        public float F => G + H;

        public PathNode(Waypoint waypoint, PathNode parent, float g, float h)
        {
            this.waypoint = waypoint;
            this.Parent = parent;
            this.G = g;
            this.H = h;
        }
    }
}
