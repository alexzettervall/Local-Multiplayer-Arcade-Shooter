using System.Collections.Generic;
using UnityEngine;
using static ChickenBlackboard;

public class ChickenPerception : Perception<ChickenBlackboard>
{
    public ChickenPerception(ChickenBlackboard blackboard) : base(blackboard)
    {

    }

    protected override void UpdatePerception()
    {
        UpdateStructures();
    }

    
    public void UpdateStructures()
    {
        List<StructureData> structureDatas = new List<StructureData>();

        Structure[] structures = GameObject.FindObjectsOfType<Structure>();

        foreach (Structure structure in structures)
        {
            // Ignore if it's in the gas
            if (Physics2D.OverlapCircle(structure.transform.position, 0.5f, GameAssets.i.poisonGasLayer))
            {
                continue;
            }

            structureDatas.Add(new StructureData(structure.transform.position));
        }

        blackboard.structureDatas = structureDatas;
    }
}