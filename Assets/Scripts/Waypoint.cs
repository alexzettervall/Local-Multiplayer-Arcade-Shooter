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
}

[System.Serializable]
public class WaypointConnection
{
    public int a;
    public int b;
    public float cost;

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
