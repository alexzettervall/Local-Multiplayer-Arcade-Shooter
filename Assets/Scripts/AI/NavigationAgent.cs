using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NavigationAgent
{
    protected Blackboard blackboard;
    AStar.Path path = null;
    int pathStepIndex = -1;
    float repathTimer = 0f;
    Vector2 currentDirection = Vector2.zero;

    bool wantToUse = false;
    bool overridedUse = false;

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

        if (wantToUse && !blackboard.use)
        {
            blackboard.use = true;
            overridedUse = true;
        }
        else if (!wantToUse && overridedUse)
        {
            blackboard.use = false;
            overridedUse = false;
        }
    }

    public void UpdatePath() {
        if (blackboard.target is not Vector2 target)
        {
            return;
        }
        path = GameObject.FindObjectOfType<Level>().FindPath(blackboard.entity.transform.position, target, blackboard.entity.GetMoveSpeed(), blackboard.entity.GetDPS());
        pathStepIndex = -1;
        if (path != null)
        {
            Waypoint first = path.steps[0].from;
            Waypoint second = path.steps[0].to;

            Vector2 dir1 = first.position - (Vector2)blackboard.entity.transform.position;
            Vector2 dir2 = second.position - (Vector2)blackboard.entity.transform.position;

            float dot = Vector2.Dot(dir1, dir2);
            if (dot < 0)
            {
                pathStepIndex = 0;
            }
        }
    }

    public void Move() {
        if (blackboard.target is not Vector2 target) return;

        Vector2 desiredDirection;
        if (GetNextPosition() is not Vector2 nextPosition) return;
        List<Structure> structuresInWay = GetStructuresInWay();
        float avoidanceWeight;

        // Determine wether to go directly for target or follow the path
        float distToTarget = Vector2.Distance(target, blackboard.entity.transform.position);
        bool hasLineOfSight = !Physics2D.Linecast(blackboard.entity.transform.position, target, GameAssets.i.structuresOnly);
        bool shouldMoveDirectly = distToTarget <= blackboard.settings.directMovementThreshold && hasLineOfSight;

        if (shouldMoveDirectly) {
            // Direct movement
            desiredDirection = target - nextPosition;
            avoidanceWeight = blackboard.settings.directMovementAvoidanceWeight;
        }
        else {
            // Follow path
            desiredDirection = nextPosition - (Vector2)blackboard.entity.transform.position;
            avoidanceWeight = blackboard.settings.structureAvoidanceWeight;

            float dist = Vector2.Distance(blackboard.entity.transform.position, nextPosition);
            if (dist < 1.42f) {
                pathStepIndex++;
            }
        }
        desiredDirection += (ComputeAvoidance() * avoidanceWeight);
        desiredDirection.Normalize();
        currentDirection = Vector2.Lerp(currentDirection, desiredDirection, blackboard.settings.movementSmoothingResponsiveness * Time.deltaTime);
        currentDirection.Normalize();
        blackboard.movement = currentDirection;
        blackboard.lookDirection = currentDirection;
        wantToUse = structuresInWay.Count > 0;
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

    public Vector2? GetNextPosition()
    {
        if (path == null || pathStepIndex >= path.steps.Count) return blackboard.target;

        if (pathStepIndex == -1)
        {
            return path.steps[0].from.position;
        }
        return path.steps[pathStepIndex].to.position;
    }

    public List<Structure> GetStructuresInWay()
    {
        if (path == null || pathStepIndex == -1 || pathStepIndex >= path.steps.Count) return new List<Structure>();
        return path.steps[pathStepIndex].connection.structures;
    }

    public void DrawGizmos() {
        if (blackboard.entity == null) return;
        if (path == null) return;

        Color nodeColor = new Color(0, 0, 1f);
        Color lineColor = new Color(0, 1f, 0);
        
        foreach (AStar.PathStep step in path.steps) {

            Gizmos.color = lineColor;
            Gizmos.DrawLine(step.from.position, step.to.position);
            Gizmos.color = nodeColor;
            Gizmos.DrawSphere(step.from.position, 0.3f);
        }
    }
}

