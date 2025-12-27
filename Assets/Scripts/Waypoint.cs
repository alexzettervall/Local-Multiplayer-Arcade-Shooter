using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Waypoint
{
    public int id;
    public Vector2 position;
    public List<int> neighborIds = new List<int>();

    [System.NonSerialized] 
    public List<Waypoint> neighbors = new List<Waypoint>();

    public Waypoint(Vector2 position) {
        this.position = position;
    }
}
