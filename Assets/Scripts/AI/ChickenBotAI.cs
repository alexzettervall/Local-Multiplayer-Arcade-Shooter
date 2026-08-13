public class ChickenBotAI : BotAI
{
    public ChickenBotAI() : base(GameAssets.i.chickenAISettings) {
        
    }

    public override Perception CreatePerception()
    {
        return new GeneralPerception(blackboard);
    }

    public override UtilityScorer CreateUtilityScorer()
    {
        return new ChickenUtilityScorer(blackboard);
    }

    public override BehaviorTree CreateBehaviorTree()
    {
        return new ChickenBehaviorTree(blackboard);
    }
}