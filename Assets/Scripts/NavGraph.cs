using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavGraph : MonoBehaviour
{
    public List<Waypoint> nodes = new List<Waypoint>();
    public List<WaypointConnection> connections = new List<WaypointConnection>();
    public float editorGridSize = 0.25f;

    public NavGraph(List<Waypoint> nodes) {
        this.nodes = nodes;
    }

    public Waypoint GetNearest(Vector2 position)
    {
        Waypoint closestWaypoint = null;
        float closestDist = float.MaxValue;
        foreach (var node in nodes)
        {
            float dist = Vector2.Distance(node.position, position);
            if (dist < closestDist) {
                closestDist = dist; closestWaypoint = node;
            }
        }
        return closestWaypoint;
    }

    public List<Vector2> FindPath(Vector2 start, Vector2 goal)
    {
        Waypoint a = GetNearest(start);
        Waypoint b = GetNearest(goal);
        if (a == null || b == null) return new List<Vector2>();
        return AStar.FindPath(this, a, b);
    }

    public float FindDistance(Vector2 start, Vector2 goal, out List<Vector2> path) {
        path = FindPath(start, goal);
        float distance = 0f;
        Vector2 lastNode = start;
        foreach (Vector2 node in path) {
            distance += Vector2.Distance(lastNode, node);
            lastNode = node;
        }
        return distance;
    }

    public Waypoint GetWaypoint(int id)
    {
        foreach (Waypoint waypoint in nodes)
        {
            if (waypoint.id == id) return waypoint;
        }

        return null;
    }

    public void BuildCache()
    {
        // Clear existing cache
        foreach (Waypoint waypoint in nodes)
        {
            waypoint.connections = new List<WaypointConnection>();
        }

        // Build it from the serialized connections
        foreach (WaypointConnection connection in connections)
        {
            Waypoint a = GetWaypoint(connection.a);
            Waypoint b = GetWaypoint(connection.b);

            if (a == null || b == null) continue;

            a.connections.Add(connection);
            b.connections.Add(connection);
        }
    }
}
