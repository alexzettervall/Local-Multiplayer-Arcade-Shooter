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
            blackboard.targetContainer = GetNearestContainer();
        }
        else
        {
            utilities["Attack"] = 100f;
            utilities["Eat"] = 0f;
        }
    }

    public Blackboard.ContainerData? GetNearestContainer()
    {
        Blackboard.ContainerData? nearestContainer = null;
        float nearestDist = float.MaxValue;

        foreach (Blackboard.ContainerData containerData in blackboard.containerDatas)
        {
            float distance = Vector2.Distance(blackboard.entity.transform.position, containerData.position);
            if (distance < nearestDist)
            {
                nearestDist = distance;
                nearestContainer = containerData;
            }
        }

        return nearestContainer;
    }
}