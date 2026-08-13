using UnityEngine;

public class ChickenUtilityScorer : UtilityScorer
{
    public ChickenUtilityScorer(Blackboard blackboard) : base(blackboard)
    {
        
    }

    public override void CalculateUtility()
    {
        utilities["Eat"] = 100f;
        blackboard.targetContainer = GetNearestContainer();
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