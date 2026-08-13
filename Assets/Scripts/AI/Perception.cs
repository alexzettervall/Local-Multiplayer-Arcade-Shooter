using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Perception
{
    protected Blackboard blackboard;

    public Perception(Blackboard blackboard) {
        this.blackboard = blackboard;
    }

    public void Update() {
        OnUpdate();
    }

    public virtual void OnUpdate()
    {
        
    }
}
