public abstract class BotAI<TBlackboard> where TBlackboard : Blackboard
{
    protected TBlackboard blackboard;
    Perception<TBlackboard> perception;
    UtilityScorer<TBlackboard> utilityScorer;
    BehaviorTree<TBlackboard> behaviorTree;
    NavigationAgent navigationAgent;

    public BotAI(AISettings aiSettings) {
        blackboard = CreateBlackboard();
        blackboard.settings = aiSettings;
        perception = CreatePerception();
        utilityScorer = CreateUtilityScorer();
        behaviorTree = CreateBehaviorTree();
        navigationAgent = new NavigationAgent(blackboard);
    }

    public abstract TBlackboard CreateBlackboard();
    public abstract Perception<TBlackboard> CreatePerception();
    public abstract UtilityScorer<TBlackboard> CreateUtilityScorer();
    public abstract BehaviorTree<TBlackboard> CreateBehaviorTree();

    public void SetEntity(LivingEntity entity)
    {
        blackboard.entity = entity;
    }

    public void Update() {
        // Return if entity doesn't exist
        if (blackboard.entity == null) {
            return;
        }

        // Update components
        perception.Update();
        utilityScorer.Update();
        behaviorTree.Update();
        navigationAgent.Update();

        // inject inputs
        blackboard.entity.OnMove(blackboard.movement);
        blackboard.entity.OnRotate(blackboard.lookDirection, null);
        blackboard.entity.OnUse(blackboard.preformUse, blackboard.cancelUse);
        if (blackboard.preformUse) {
            blackboard.isUsing = true;
        }
        if (blackboard.cancelUse) {
            blackboard.isUsing = false;
        }
        // Set flags to false because they are triggers
        blackboard.preformUse = false;
        blackboard.cancelUse = false;

        blackboard.entity.OnInteract(blackboard.interact);
        blackboard.interact = false;

        blackboard.entity.OnDrop(blackboard.drop);
        blackboard.drop = false;
    }

    public TBlackboard GetBlackboard()
    {
        return blackboard;
    }

    public void DrawGizmos() {
        utilityScorer.DrawGizmos();
        navigationAgent.DrawGizmos();
    }
}
