using UnityEngine;

public class ChickenUtilityScorer : UtilityScorer<ChickenBlackboard>
{
    public ChickenUtilityScorer(ChickenBlackboard blackboard) : base(blackboard)
    {
        
    }

    public override void CalculateUtility()
    {
        if (blackboard.agroTarget == null)
        {
            utilities["Eat"] = 100f;
            utilities["Attack"] = 0f;
            blackboard.targetStructure = blackboard.GetNearest(blackboard.structureDatas);
        }
        else
        {
            utilities["Attack"] = 100f;
            utilities["Eat"] = 0f;
        }
    }
}