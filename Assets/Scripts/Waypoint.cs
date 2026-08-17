using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Waypoint
{
    public int id;
    public Vector2 position;

    [System.NonSerialized]
    public List<WaypointConnection> connections = new List<WaypointConnection>();

    public Waypoint(int id, Vector2 position) {
        this.id = id;
        this.position = position;
    }

    public WaypointConnection GetConnection(Waypoint other)
    {
        foreach (WaypointConnection connection in connections)
        {
            if (connection.GetOther(this) == other.id)
            {
                return connection;
            }
        }
        return null;
    }
}

[System.Serializable]
public class WaypointConnection
{
    public int a;
    public int b;
    [System.NonSerialized]
    public List<Structure> structures;
    [System.NonSerialized]
    public float distance;
    [System.NonSerialized]
    public float damageNeeded;

    public WaypointConnection(int a, int b)
    {
        this.a = a;
        this.b = b;
    }

    public int GetOther(Waypoint waypoint)
    {
        return waypoint.id == a ? b : a;
    }
}
