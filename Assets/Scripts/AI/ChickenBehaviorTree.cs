using UnityEngine;

public class ChickenBehaviorTree : BehaviorTree
{
    public ChickenBehaviorTree(Blackboard blackboard) : base(blackboard)
    {
        
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (blackboard.goal == "Eat")
        {
            Eat();
        }
    }

    public void Eat()
    {
        if (blackboard.targetContainer is not Blackboard.ContainerData container)
        {
            return;
        }

        blackboard.target = container.position;
        blackboard.move = true;
        if (Physics2D.Raycast(blackboard.entity.transform.position, blackboard.lookDirection, 1f, GameAssets.i.structuresOnly))
        {
            blackboard.preformUse = true;
            blackboard.isUsing = true;
        }
        else
        {
            blackboard.cancelUse = true;
            blackboard.isUsing = false;
        }
    }
}