using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NavGraph))]
public class WaypointGraphEditor : Editor
{
    private NavGraph graph;
    private Waypoint connectionStartWaypoint;
    private Waypoint selectedWaypoint;

    private void OnEnable()
    {
        graph = (NavGraph)target;
        connectionStartWaypoint = null;
    }

    private void OnSceneGUI()
    {
        HandleWaypointCreation();
        HandleDeletion();
        HandleWaypointMovement();
        HandleConnections();
        HandleConnectionDeletion();

        DrawWaypoints();
        DrawConnections();
    }

    private void HandleWaypointCreation()
    {
        Event e = Event.current;

        if (e.shift && !e.control && e.type == EventType.MouseDown && e.button == 0)
        {
            Vector2 mousePosition = GetMouseWorldPosition(e);

            Vector2 snappedPosition = SnapToGrid(mousePosition);

            Undo.RecordObject(graph, "Create Waypoint");

            int id = GetNextWaypointId();
            if (id == -1)
            {
                Debug.LogError("Node limit of 1000 reached.");
            }

            Waypoint waypoint = new Waypoint(id, snappedPosition);
            graph.nodes.Add(waypoint);
            selectedWaypoint = waypoint;

            EditorUtility.SetDirty(graph);
            e.Use();
        }
    }

    private void HandleDeletion()
    {
        Event e = Event.current;

        if (e.type != EventType.KeyDown) return;

        if (e.keyCode != KeyCode.Delete && e.keyCode != KeyCode.Backspace) return;

        if (selectedWaypoint == null) return;

        Undo.RecordObject(graph, "Delete Waypoint");

        int id = selectedWaypoint.id;

        // Remove connections involving this waypoint
        graph.connections.RemoveAll(connection => connection.a == id || connection.b == id);

        // Remove the waypoint
        graph.nodes.Remove(selectedWaypoint);

        selectedWaypoint = null;

        EditorUtility.SetDirty(graph);
        SceneView.RepaintAll();

        e.Use();
    }

    private int GetNextWaypointId()
    {
        for (int id = 0; id < 1000; id++)
        {
            bool taken = false;

            foreach (Waypoint wp in graph.nodes)
            {
                if (wp.id == id)
                {
                    taken = true;
                    break;
                }
            }

            if (!taken) return id;
        }

        return -1;
    }

    private void HandleWaypointMovement()
    {
        Event e = Event.current;
        
        if (e.control) return;
        
        foreach (Waypoint waypoint in graph.nodes)
        {
            Vector3 position = new Vector3(
                waypoint.position.x,
                waypoint.position.y,
                0f
            );

            float handleSize = HandleUtility.GetHandleSize(position) * 0.1f;

            EditorGUI.BeginChangeCheck();

            Handles.color = waypoint == selectedWaypoint ? Color.yellow : Color.white;
            Vector3 newPosition = Handles.FreeMoveHandle(
                position,
                handleSize,
                Vector3.zero,
                Handles.DotHandleCap
            );

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(graph, "Move Waypoint");

                Vector2 snappedPosition = SnapToGrid(newPosition);

                waypoint.position = snappedPosition;
                selectedWaypoint = waypoint;

                EditorUtility.SetDirty(graph);
            }
        }
    }

    private void HandleConnections()
    {
        Event e = Event.current;

        // Control was released
        if (e.type == EventType.KeyUp && e.keyCode == KeyCode.LeftControl)
        {
            connectionStartWaypoint = null;
            return;
        }

        // Control pressed
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.LeftControl)
        {
            selectedWaypoint = null;
            return;
        }

        if (!e.control || e.type != EventType.MouseDown || e.button != 0) return;

        Waypoint clickedWaypoint = GetWaypointAtMouse(e.mousePosition);

        if (clickedWaypoint == null) return;

        if (connectionStartWaypoint == null)
        {
            // First waypoint selected
            connectionStartWaypoint = clickedWaypoint;
        }
        else
        {
            // Second waypoint selected
            if (connectionStartWaypoint != clickedWaypoint)
            {
                ConnectWaypoints(connectionStartWaypoint, clickedWaypoint);
            }

            connectionStartWaypoint = null;
        }

        e.Use();
    }

    private void HandleConnectionDeletion()
    {
        Event e = Event.current;

        if (!e.control || e.type != EventType.MouseDown || e.button != 1) return;

        Vector2 mousePosition = GetMouseWorldPosition(e.mousePosition);

        WaypointConnection closestConnection = null;
        float closestDistance = 0.5f;

        foreach (WaypointConnection connection in graph.connections)
        {
            Waypoint a = GetWaypoint(connection.a);
            Waypoint b = GetWaypoint(connection.b);

            if (a == null || b == null) continue;

            float distance = DistanceToLineSegment(mousePosition ,a.position, b.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestConnection = connection;
            }
        }

        if (closestConnection != null)
        {
            Undo.RecordObject(graph, "Delete Connection");

            graph.connections.Remove(closestConnection);

            EditorUtility.SetDirty(graph);
            SceneView.RepaintAll();

            e.Use();
        }
    }

    private float DistanceToLineSegment(Vector2 point, Vector2 lineStart, Vector2 lineEnd)
    {
        Vector2 line = lineEnd - lineStart;

        float lengthSquared = line.sqrMagnitude;

        if (lengthSquared == 0f) return Vector2.Distance(point, lineStart);

        float t = Vector2.Dot(point - lineStart, line) / lengthSquared;

        t = Mathf.Clamp01(t);

        Vector2 closestPoint = lineStart + line * t;

        return Vector2.Distance(point, closestPoint);
    }

    private void ConnectWaypoints(Waypoint a, Waypoint b)
    {
        Undo.RecordObject(graph, "Connect Waypoints");

        // Make sure connection doesn't already exist
        foreach (WaypointConnection connection in graph.connections)
        {
            if (GetWaypoint(connection.a) == a && GetWaypoint(connection.b) == b) return;
            if (GetWaypoint(connection.b) == a && GetWaypoint(connection.a) == b) return;
        }
        graph.connections.Add(new WaypointConnection(a.id, b.id));

        EditorUtility.SetDirty(graph);
    }

    private Waypoint GetWaypointAtMouse(Vector2 mousePosition)
    {
        Vector2 worldPosition = GetMouseWorldPosition(mousePosition);

        float closestDistance = 0.5f;
        Waypoint closest = null;

        foreach (Waypoint waypoint in graph.nodes)
        {
            float distance = Vector2.Distance(
                waypoint.position,
                worldPosition
            );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = waypoint;
            }
        }

        return closest;
    }

    private Vector2 GetMouseWorldPosition(Event e)
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

        float distance = -ray.origin.z / ray.direction.z;

        Vector3 worldPosition = ray.origin + ray.direction * distance;

        return new Vector2(
            worldPosition.x,
            worldPosition.y
        );
    }
    private Vector2 GetMouseWorldPosition(Vector2 mousePosition)
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(
            mousePosition
        );

        float distance =
            -ray.origin.z / ray.direction.z;

        Vector3 worldPosition =
            ray.origin + ray.direction * distance;

        return new Vector2(
            worldPosition.x,
            worldPosition.y
        );
    }

    private void DrawWaypoints()
    {
        foreach (Waypoint waypoint in graph.nodes)
        {
            Vector3 position = new Vector3(
                waypoint.position.x,
                waypoint.position.y,
                0f
            );

            Handles.color = waypoint == selectedWaypoint ? Color.yellow : Color.cyan;

            if (waypoint == connectionStartWaypoint)
            {
                Handles.color = Color.yellow;
            }

            Handles.DrawSolidDisc(position, Vector3.forward, 0.15f);

            Handles.Label( position + Vector3.up * 0.2f, $"WP {waypoint.id}");
        }
    }

    private void DrawConnections()
    {
        foreach (WaypointConnection connection in graph.connections)
        {
            Vector2 a = GetWaypoint(connection.a).position;
            Vector2 b = GetWaypoint(connection.b).position;
            Handles.color = Color.blue;
            Handles.DrawAAPolyLine(8f, a, b);
        }
    }

    private Vector2 SnapToGrid(Vector2 position)
    {
        if (graph.editorGridSize == 0)
        {
            return position;
        }

        return new Vector2(
            Mathf.Round(position.x / graph.editorGridSize) * graph.editorGridSize,
            Mathf.Round(position.y / graph.editorGridSize) * graph.editorGridSize
        );
    }

    private Waypoint GetWaypoint(int id)
    {
        foreach (Waypoint waypoint in graph.nodes)
        {
            if (waypoint.id == id) return waypoint;
        }

        return null;
    }
}
