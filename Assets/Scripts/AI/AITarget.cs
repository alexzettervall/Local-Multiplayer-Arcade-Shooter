using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetProvider
{
    Vector2 GetPosition();
}

public class TransformTarget : ITargetProvider
{
    public Transform transform;
    public Vector2 GetPosition() => transform == null ? Vector2.zero : transform.position;
}

public class StaticTarget : ITargetProvider
{
    public Vector2 position;
    public Vector2 GetPosition() => position;
}

public class AITarget
{
    public ITargetProvider provider;
    public bool useInstantUpdate = false;

    public Vector2 GetPosition()
    {
        return provider.GetPosition();
    }
}
