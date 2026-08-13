public abstract class BehaviorTree
{
    protected Blackboard blackboard;
    
    public BehaviorTree(Blackboard blackboard) {
        this.blackboard = blackboard;
    }

    public void Update() {
        OnUpdate();
    }

    public virtual void OnUpdate()
    {
        
    }
}