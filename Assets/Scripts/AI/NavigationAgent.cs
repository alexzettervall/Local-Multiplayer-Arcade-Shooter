using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationAgent
{
    Blackboard blackboard;
    List<Vector2> path = null;
    float repathTimer = 0f;
    Vector2 currentDirection = Vector2.zero;

    public NavigationAgent(Blackboard blackboard) {
        this.blackboard = blackboard;
    }

    public void Update() {
        repathTimer -= Time.deltaTime;
        if (repathTimer <= 0) {
            UpdatePath();
            repathTimer = GameAssets.i.AISettings.repathPeriod;
        }

        if (blackboard.move) {
            Move();
        }
        else {
            blackboard.movement = Vector2.zero;
        }
    }

    public void UpdatePath() {
        if (blackboard.target == null) return;
        path = GameObject.FindObjectOfType<Level>().FindPath(blackboard.player.transform.position, blackboard.target.GetPosition());
    }

    public void Move() {
        if (path == null) return;
        if (blackboard.target == null) return;

        float distToTarget = Vector2.Distance(blackboard.target.GetPosition(), blackboard.player.transform.position);
        Vector2 desiredDirection = Vector2.zero;
        // Determine wether to go directly for target or follow the path
        float avoidanceWeight = 1f;
        bool hasLineOfSight = !Physics2D.Linecast(blackboard.player.transform.position, blackboard.target.GetPosition(), GameAssets.i.structuresOnly);
        bool noPath = path.Count < 1;
        bool shouldMoveDirectly = distToTarget <= GameAssets.i.AISettings.directMovementThreshold && hasLineOfSight;
        if (noPath || shouldMoveDirectly) {
            // Direct movement
            desiredDirection = blackboard.target.GetPosition() - (Vector2)blackboard.player.transform.position;
            avoidanceWeight = GameAssets.i.AISettings.directMovementAvoidanceWeight;
        }
        else {
            // Follow path
            desiredDirection = path[0] - (Vector2)blackboard.player.transform.position;
            avoidanceWeight = GameAssets.i.AISettings.structureAvoidanceWeight;

            float dist = Vector2.Distance(blackboard.player.transform.position, path[0]);
            if (dist < 1.42f) {
                path.RemoveAt(0);
            }
        }
        desiredDirection += (ComputeAvoidance() * avoidanceWeight);
        desiredDirection.Normalize();
        currentDirection = Vector2.Lerp(currentDirection, desiredDirection, GameAssets.i.AISettings.movementSmoothingResponsiveness * Time.deltaTime);
        currentDirection.Normalize();
        blackboard.movement = currentDirection;
        blackboard.lookDirection = currentDirection;
    }

    public Vector2 ComputeAvoidance() {
        Vector2 position = blackboard.player.transform.position;
        Vector2 avoidance = Vector2.zero;

        AISettings aiSettings = GameAssets.i.AISettings;
        float maxAvoidanceRadius = Mathf.Max(aiSettings.structureAvoidanceRadius, aiSettings.grenadeAvoidanceRadius, aiSettings.gasAvoidanceRadius);
        Collider2D[] allCols = Physics2D.OverlapCircleAll(position, maxAvoidanceRadius);

        
        foreach (Collider2D hit in allCols) {
            Vector2 closest = hit.ClosestPoint(position);
            Vector2 toObstacle = position - closest;
            float distance = toObstacle.magnitude;
            Grenade grenade = hit.gameObject.GetComponent<Grenade>();

            if (distance <= 0)
            {
                continue;
            }

            // Structure avoidance
            if (distance <= aiSettings.structureAvoidanceRadius && ((GameAssets.i.structuresOnly.value & (1 << hit.gameObject.layer)) != 0))
            {
                avoidance += GameAssets.i.AISettings.structureAvoidanceWeight * toObstacle.normalized / distance;
            }
            // Grenade Avoidance
            else if (distance <= aiSettings.grenadeAvoidanceRadius && grenade != null && grenade.HasTag("dangerous"))
            {
               avoidance += GameAssets.i.AISettings.grenadeAvoidanceWeight * toObstacle.normalized / distance;
            }
            // Gas avoidance
            else if (distance <= aiSettings.gasAvoidanceRadius && hit.gameObject.tag == "Poison Gas")
            {
                avoidance += GameAssets.i.AISettings.gasAvoidanceWeight * toObstacle.normalized / distance;
            }
        }

        return avoidance;
    }

    public void DrawGizmos() {
        if (blackboard.player == null) {
            return;
        }
        Color nodeColor = new Color(0, 0, 1f);
        Color lineColor = new Color(0, 1f, 0);
        Vector2 prevNode = blackboard.player.transform.position;
        foreach (Vector2 node in path) {
            Gizmos.color = lineColor;
            if (prevNode != null) {
                Gizmos.DrawLine(prevNode, node);
            }
            Gizmos.color = nodeColor;
            prevNode = node;
            Gizmos.DrawSphere(node, 0.1f);
        }
    }
}

