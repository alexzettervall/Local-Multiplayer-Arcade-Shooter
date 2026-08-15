public abstract class BehaviorTree<TBlackboard> where TBlackboard : Blackboard
{
    protected TBlackboard blackboard;
    
    protected BehaviorTree(TBlackboard blackboard) {
        this.blackboard = blackboard;
    }

    public void Update() {
        OnUpdate();
    }

    public virtual void OnUpdate()
    {
        
    }
}