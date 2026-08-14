using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Egg : Item
{
    [SerializeField] private float timeUntilHatch;
    private bool hatched = false;

    protected override void OnStart()
    {
        base.OnStart();
        SetCircleColor(GameAssets.i.itemCircleUseless);
        tags.Add("throwable");
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        timeUntilHatch -= Time.deltaTime;
        if (timeUntilHatch <= 0)
        {
            Hatch();
        }
    }

    public void Hatch()
    {
        if (hatched) return;
        hatched = true;
        Instantiate(GameAssets.i.chickenPrefab, transform.position, quaternion.identity, transform.parent);
        Kill();
    }
}
