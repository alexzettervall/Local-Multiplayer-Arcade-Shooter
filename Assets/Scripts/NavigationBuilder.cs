using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NavigationBuilder
{
    public static List<Waypoint> BuildFromScene(float spacing, float connectRadius)
    {
        LayerMask layerMask = GameAssets.i.structuresOnly;
        List<Waypoint> nodes = new List<Waypoint>();
        
        // Step 1: sample grid
        for (float x = -17; x <= 17; x += spacing) {
            for (float y = -10; y <= 10; y += spacing)
            {
                Vector2 position = new Vector2(x, y);
                if (!Physics2D.OverlapCircle(position, spacing * 0.4f, layerMask)) {
                    nodes.Add(new Waypoint(position));
                }
            
            }
        }

        // Step 2: connect nodes in line of sight
        foreach (Waypoint a in nodes) {
            foreach (var b in nodes)
            {
                if (a == b) continue;
                if (Vector2.Distance(a.position, b.position) > connectRadius) continue;
                if (!Physics2D.Linecast(a.position, b.position, layerMask)) {
                    a.neighbors.Add(b);
                }
            }
        }

        return nodes;
    }
}
