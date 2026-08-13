public abstract class BotAI
{
    protected Blackboard blackboard;
    Perception perception;
    UtilityScorer utilityScorer;
    BehaviorTree behaviorTree;
    NavigationAgent navigationAgent;

    public BotAI(AISettings aiSettings) {
        blackboard = new Blackboard();
        blackboard.settings = aiSettings;
        perception = CreatePerception();
        utilityScorer = CreateUtilityScorer();
        behaviorTree = CreateBehaviorTree();
        navigationAgent = new NavigationAgent(blackboard);
    }

    public abstract Perception CreatePerception();
    public abstract UtilityScorer CreateUtilityScorer();
    public abstract BehaviorTree CreateBehaviorTree();

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

    public void DrawGizmos() {
        utilityScorer.DrawGizmos();
        navigationAgent.DrawGizmos();
    }
}
