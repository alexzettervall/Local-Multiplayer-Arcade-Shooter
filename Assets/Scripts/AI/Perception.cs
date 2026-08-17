using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Perception<TBlackboard> where TBlackboard : Blackboard
{
    protected TBlackboard blackboard;

    private float perceptionTimer = 0f;

    public Perception(TBlackboard blackboard) {
        this.blackboard = blackboard;
    }

    public void Update() {
        perceptionTimer -= Time.deltaTime;
        if (perceptionTimer <= 0)
        {
            perceptionTimer = blackboard.settings.perceptionPeriod + Random.Range(-blackboard.settings.jitter, blackboard.settings.jitter);
            UpdatePerception();
        }
    }

    protected abstract void UpdatePerception();
}
