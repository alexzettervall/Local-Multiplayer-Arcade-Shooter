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

    public AStar.Path FindPath(Vector2 start, Vector2 goal, float moveSpeed, float dps)
    {
        Waypoint a = GetNearest(start);
        Waypoint b = GetNearest(goal);
        if (a == null || b == null) return null;
        return AStar.FindPath(this, a, b, moveSpeed, dps);
    }

    public float FindDistance(Vector2 start, Vector2 goal, float moveSpeed, float dps, out AStar.Path path) {
        path = FindPath(start, goal, moveSpeed, dps);
        float distance = 0f;
        foreach (AStar.PathStep step in path.steps) {
            distance += step.connection.distance;
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
        UpdateWaypointsCache();
        UpdateConnectionsCache();
    }

    private void UpdateWaypointsCache()
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

    private void UpdateConnectionsCache()
    {
        foreach (WaypointConnection connection in connections)
        {
            Waypoint a = GetWaypoint(connection.a);
            Waypoint b = GetWaypoint(connection.b);
            connection.distance = Vector2.Distance(a.position, b.position);
            connection.structures = new List<Structure>();
            RaycastHit2D[] hits = Physics2D.LinecastAll(a.position, b.position, GameAssets.i.structuresOnly);
            foreach (RaycastHit2D hit in hits)
            {
                Structure structure = hit.transform.gameObject.GetComponent<Structure>();
                if (structure != null)
                {
                    connection.structures.Add(structure);
                }
            }
            float damageNeeded = 0;
            foreach (Structure structure in connection.structures)
            {
                if (!structure.IsBreakable())
                {
                    damageNeeded = float.MaxValue;
                    break;
                }
                damageNeeded += structure.GetHealth();
            }
            connection.damageNeeded = damageNeeded;
        }
    }
}
