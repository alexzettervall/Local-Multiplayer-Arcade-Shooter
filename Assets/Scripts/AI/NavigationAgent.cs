using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationAgent
{
    protected Blackboard blackboard;
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
            repathTimer = blackboard.settings.repathPeriod;
        }

        if (blackboard.move) {
            Move();
        }
        else {
            blackboard.movement = Vector2.zero;
        }
    }

    public void UpdatePath() {
        if (blackboard.target is not Vector2 target)
        {
            return;
        }
        path = GameObject.FindObjectOfType<Level>().FindPath(blackboard.entity.transform.position, target, blackboard.entity.GetMoveSpeed(), blackboard.entity.GetDPS());
        // Skip first node if its behind entity
        if (path.Count > 1)
        {
            Vector2 dir1 = path[0] - (Vector2)blackboard.entity.transform.position;
            Vector2 dir2 = path[1] - (Vector2)blackboard.entity.transform.position;

            float dot = Vector2.Dot(dir1, dir2);
            if (dot < 0)
            {
                path.RemoveAt(0);
            }
        }
    }

    public void Move() {
        if (path == null) return;
        if (blackboard.target is not Vector2 target)
        {
            return;
        }

        float distToTarget = Vector2.Distance(target, blackboard.entity.transform.position);
        Vector2 desiredDirection = Vector2.zero;
        // Determine wether to go directly for target or follow the path
        float avoidanceWeight = 1f;
        bool hasLineOfSight = !Physics2D.Linecast(blackboard.entity.transform.position, target, GameAssets.i.structuresOnly);
        bool noPath = path.Count < 1;
        bool shouldMoveDirectly = distToTarget <= blackboard.settings.directMovementThreshold && hasLineOfSight;
        if (noPath || shouldMoveDirectly) {
            // Direct movement
            desiredDirection = target - (Vector2)blackboard.entity.transform.position;
            avoidanceWeight = blackboard.settings.directMovementAvoidanceWeight;
        }
        else {
            // Follow path
            desiredDirection = path[0] - (Vector2)blackboard.entity.transform.position;
            avoidanceWeight = blackboard.settings.structureAvoidanceWeight;

            float dist = Vector2.Distance(blackboard.entity.transform.position, path[0]);
            if (dist < 1.42f) {
                path.RemoveAt(0);
            }
        }
        desiredDirection += (ComputeAvoidance() * avoidanceWeight);
        desiredDirection.Normalize();
        currentDirection = Vector2.Lerp(currentDirection, desiredDirection, blackboard.settings.movementSmoothingResponsiveness * Time.deltaTime);
        currentDirection.Normalize();
        blackboard.movement = currentDirection;
        blackboard.lookDirection = currentDirection;
    }

    public Vector2 ComputeAvoidance() {
        Vector2 position = blackboard.entity.transform.position;
        Vector2 avoidance = Vector2.zero;

        float maxAvoidanceRadius = Mathf.Max(blackboard.settings.structureAvoidanceRadius, blackboard.settings.grenadeAvoidanceRadius, blackboard.settings.gasAvoidanceRadius);
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
            if (distance <= blackboard.settings.structureAvoidanceRadius && ((GameAssets.i.structuresOnly.value & (1 << hit.gameObject.layer)) != 0))
            {
                avoidance += blackboard.settings.structureAvoidanceWeight * toObstacle.normalized / distance;
            }
            // Grenade Avoidance
            else if (distance <= blackboard.settings.grenadeAvoidanceRadius && grenade != null && grenade.HasTag("dangerous"))
            {
               avoidance += blackboard.settings.grenadeAvoidanceWeight * toObstacle.normalized / distance;
            }
            // Gas avoidance
            else if (distance <= blackboard.settings.gasAvoidanceRadius && hit.gameObject.tag == "Poison Gas")
            {
                avoidance += blackboard.settings.gasAvoidanceWeight * toObstacle.normalized / distance;
            }
        }

        return avoidance;
    }

    public void DrawGizmos() {
        if (blackboard.entity == null) {
            return;
        }
        Color nodeColor = new Color(0, 0, 1f);
        Color lineColor = new Color(0, 1f, 0);
        Vector2 prevNode = blackboard.entity.transform.position;
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

