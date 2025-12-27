using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D))]
public class Ground : MonoBehaviour
{
    public void UpdateBorders()
    {
        EdgeCollider2D edgeCollider = GetComponent<EdgeCollider2D>();
        Vector2[] points = new Vector2[]
        {
            new Vector2(-0.5f, -0.5f), // Bottom-left
            new Vector2(-0.5f, 0.5f),  // Top-left
            new Vector2(0.5f, 0.5f),   // Top-right
            new Vector2(0.5f, -0.5f),  // Bottom-right
            new Vector2(-0.5f, -0.5f)  // Close loop (back to Bottom-left)
        };
        edgeCollider.points = points;
    }
}
