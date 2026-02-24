using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallBullet : Bullet
{
    [SerializeField] private bool overrideTrailColor = true;
    [SerializeField] private TrailRenderer trailRenderer;

    protected override void OnStart()
    {
        base.OnStart();
        if (overrideTrailColor)
        {
            trailRenderer.colorGradient = GameAssets.i.bulletGradient;
        }
    }
}
