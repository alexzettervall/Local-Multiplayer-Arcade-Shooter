using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C4Remote : Item
{
    [SerializeField] private C4 c4;
    protected override void OnStart() {
        base.OnStart();
        tags.Add("deadly weapon");
    }
    public override void Use(bool use, bool unUse)
    {
        base.Use(use, unUse);
        bool playSound = false;
        if (use)
        {
            playSound = true;
        }
        if (c4 != null)
        {
            playSound = true;
            c4.Explode(holder);
            SetCircleColor(GameAssets.i.itemCircleUseless);
            tags.Remove("deadly weapon");
        }
        if (playSound)
        {
            AudioMan.PlaySound(Sound.C4Remote);
        }
    }
}
