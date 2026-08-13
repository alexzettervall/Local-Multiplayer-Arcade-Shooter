using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor.Playables;
using Unity.VisualScripting;
using System.Linq;




#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class UtilityScorer
{
    protected Blackboard blackboard;

    float utilityTimer = 0f;

    public Dictionary<String, float> utilities = new Dictionary<string, float>();

    public UtilityScorer(Blackboard blackboard) {
        this.blackboard = blackboard;
    }

    public void Update() {
        utilityTimer -= Time.deltaTime;
        if (utilityTimer <= 0 || blackboard.isDirty) {
            CalculateUtility();
            SetGoal();
            utilityTimer = blackboard.settings.utilityRecalcPeriod;
        }
    }

    public abstract void CalculateUtility();

    public void SetGoal() {
        string goal = utilities.OrderByDescending(x => x.Value).First().Key;
        // See if goal changes
        if (blackboard.goal != goal) {
            blackboard.target = null;
        }
        blackboard.goal = goal;
    }

    public void DrawGizmos() {

        if (blackboard.entity == null) {
            return;
        }
        // Set gizmo position
        Vector3 position = blackboard.entity.transform.position + Vector3.up * 2f;


        // Draw the text label
        GUIStyle style = new GUIStyle();
        style.normal.textColor = new Color(1f, 1f, 1f);
        style.fontStyle = FontStyle.Bold;
        style.fontSize = 20;

        #if UNITY_EDITOR
            Vector3 labelPosition = position;
            foreach (String utility in utilities.Keys)
            {
                float utilityValue = utilities[utility];
                Handles.Label(labelPosition, utility + " " + utilityValue.ToString(), style);
                labelPosition += new Vector3(0, 0.5f);
            }
        #endif
    }
}
