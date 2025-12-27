using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeGrenade : Grenade
{
    [SerializeField] private ParticleSystem smoke;
    [SerializeField] private float time;
    private float timer;
    private bool activated = false;

    protected override void OnStart() {
        base.OnStart();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (activated)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                smoke.transform.parent = FindObjectOfType<Level>().transform;
                if (holder != null)
                {
                    holder.DropItem(true);
                }
                smoke.Stop(false, ParticleSystemStopBehavior.StopEmitting);
                Kill();
            }
        }
    }
    public override void OnExplosion()
    {
        AudioMan.PlaySound(Sound.GasStart);
        smoke.gameObject.SetActive(true);
        activated = true;
        timer = time;
    }
    public override void OnPullBack()
    {
        base.OnPullBack();
        SetCircleColor(GameAssets.i.itemCircleUseless);
    }
}
