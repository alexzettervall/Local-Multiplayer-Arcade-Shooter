using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPG : Gun
{
    public Rocket rocket;

    protected override void OnStart() {
        base.OnStart();
        damage = rocket.GetDamage();
    }

    public override void Shoot(bool pressed) {
        if (rocket == null || rocket.launched) {
            if (pressed) {
                AudioMan.PlaySound(Sound.GunClick);
            }
            return;
        }
        rocket.SetShooter(GetHolder());
        rocket.Launch();
        tags.Remove("deadly weapon");
        ammo = 0;
        SetCircleColor(GameAssets.i.itemCircleUseless);
        AudioMan.PlaySound(Sound.ShootRPG);
    }
}
