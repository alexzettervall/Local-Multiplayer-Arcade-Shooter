using System.Collections.Generic;
using UnityEngine;

public class ChickenBlackboard : Blackboard
{
    public Entity agroTarget;
    public List<StructureData> structureDatas;
    public StructureData? targetStructure;

    public struct StructureData : IPerceivedEntity
    {
        public StructureData(Vector2 position)
        {
            Position = position;
        }

        public Vector2 Position { get; }
    }
}