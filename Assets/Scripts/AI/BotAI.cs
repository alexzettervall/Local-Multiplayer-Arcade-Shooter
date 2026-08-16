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
        blackboard.Update();
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
