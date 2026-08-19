using UnityEngine;

[CreateAssetMenu(menuName = "AI/AISettings")]
public class AISettings : ScriptableObject
{
    public float jitter = 0.1f; // Use to add randomness

    [Header("Perception")]
    public float perceptionPeriod;

    [Header("Utility Scorer")]
    public float utilityRecalcPeriod = 2f;
    public float goalSwitchPenalty = 0.2f;

    [Header("Combat")]
    public float stayEngagedRange = 3f; // Stay enganged even if no weapon in this range
    public float aimErrorMin = 1f;
    public float aimErrorMax = 5f;
    public float reactionDelay = 0.2f;

    [Header("Navigation")]
    public AStarSettings aStarSettings;
    public float repathPeriod = 0.5f;
    public float structureAvoidanceRadius = 1f;
    public float structureAvoidanceWeight = 1f;
    public float grenadeAvoidanceRadius = 1f;
    public float grenadeAvoidanceWeight = 1f;
    public float gasAvoidanceRadius = 1f;
    public float gasAvoidanceWeight = 1f;
    
    public float movementSmoothingResponsiveness = 5f;
    public float directMovementThreshold = 1f;
    public float directMovementAvoidanceWeight = 0.2f;
    public float stuckTimeThreshold = 1.5f;

    [Header("Debug")]
    public bool drawDebugVision = false;
}
