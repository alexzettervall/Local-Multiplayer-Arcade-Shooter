using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Perception<TBlackboard> where TBlackboard : Blackboard
{
    protected TBlackboard blackboard;

    public Perception(TBlackboard blackboard) {
        this.blackboard = blackboard;
    }

    public void Update() {
        OnUpdate();
    }

    public virtual void OnUpdate()
    {
        
    }
}
