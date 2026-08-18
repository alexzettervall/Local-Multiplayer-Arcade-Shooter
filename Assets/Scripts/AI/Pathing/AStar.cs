using System.Collections.Generic;
using UnityEngine;

public static class AStar
{
    public static Path FindPath(NavGraph navGraph, Waypoint start, Waypoint goal, float moveSpeed, float dps)
    {
        if (start == null) return null;

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
            {
                return ReconstructPath(current);
            }

            closed.Add(current.waypoint);

            foreach (var connection in current.waypoint.connections)
            {
                Waypoint neighbor = navGraph.GetWaypoint(connection.GetOther(current.waypoint));

                if (closed.Contains(neighbor)) continue;

                //float gScore = current.G + Vector2.Distance(current.waypoint.position, neighbor.position);f
                float gScore = current.G + (connection.distance / moveSpeed) + (connection.damageNeeded / dps);

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
        return null; // finish converting all paths to be of type AStarPath
    }

    private static float Heuristic(Waypoint a, Waypoint b)
    {
        return Vector2.Distance(a.position, b.position);
    }

    private static Path ReconstructPath(PathNode node)
    {
        if (node.Parent == null) return null;

        Path path = new Path();
        while (node.Parent != null)
        {
            PathStep step = new PathStep();
            step.to = node.waypoint;
            step.from = node.Parent.waypoint;
            step.connection = node.connection;
            path.steps.Insert(0, step);
            node = node.Parent;
        }
        return path;
    }

    public class PathNode
    {
        public Waypoint waypoint;
        public WaypointConnection connection;
        public PathNode Parent;
        public float G; // cost from start
        public float H; // heuristic to goal
        public float F => G + H;

        public PathNode(Waypoint waypoint, PathNode parent, float g, float h)
        {
            this.waypoint = waypoint;
            this.Parent = parent;
            if (waypoint != null && parent != null)
            {
                this.connection = waypoint.GetConnection(parent.waypoint);
            }
            this.G = g;
            this.H = h;
        }
    }

    public class Path
    {
        public List<PathStep> steps = new List<PathStep>();
    }
    public class PathStep
    {
        public Waypoint from;
        public Waypoint to;
        public WaypointConnection connection;
    }
}
