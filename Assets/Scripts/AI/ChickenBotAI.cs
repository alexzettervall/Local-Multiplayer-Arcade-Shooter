public class ChickenBotAI : BotAI<ChickenBlackboard>
{
    public ChickenBotAI() : base(GameAssets.i.chickenAISettings) {
        
    }

    public override ChickenBlackboard CreateBlackboard()
    {
        return new ChickenBlackboard();
    }

    public override Perception<ChickenBlackboard> CreatePerception()
    {
        return new ChickenPerception(blackboard);
    }

    public override UtilityScorer<ChickenBlackboard> CreateUtilityScorer()
    {
        return new ChickenUtilityScorer(blackboard);
    }

    public override BehaviorTree<ChickenBlackboard> CreateBehaviorTree()
    {
        return new ChickenBehaviorTree(blackboard);
    }
}