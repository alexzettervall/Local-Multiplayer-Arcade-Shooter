using UnityEngine;

public class ChickenBehaviorTree : BehaviorTree<ChickenBlackboard>
{
    public ChickenBehaviorTree(ChickenBlackboard blackboard) : base(blackboard)
    {
        
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (blackboard.goal == "Eat")
        {
            Eat();
        }
        else if (blackboard.goal == "Attack")
        {
            Attack();
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
        blackboard.use = Physics2D.Raycast(blackboard.entity.transform.position, blackboard.lookDirection, 1f, GameAssets.i.structuresOnly);
    }

    public void Attack()
    {
        if (blackboard.agroTarget == null)
        {
            return;
        }

        blackboard.target = blackboard.agroTarget.transform.position;

        if (blackboard.target is not Vector2 target)
        {
            return;
        }

        blackboard.move = true;
        blackboard.use = Vector2.Distance(target, blackboard.entity.transform.position) < 1f;
    }
}