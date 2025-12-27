using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Grenade : Item
{
    [SerializeField] private Sprite pinOut;
    protected Entity thrower;
    private bool cooking = false;
    private bool exploded = false;
    private float timer = 3f;
    
    protected override void OnStart() {
        base.OnStart();
        tags.Add("deadly weapon");
        tags.Add("throwable");
    }

    protected override void OnUpdate()
    {
        if (cooking && !exploded)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                Explode();
                exploded = true;
            }
        }
    }
    public override void OnPullBack()
    {
        base.OnPullBack();
        if (pinOut != null)
        {
            GetComponent<SpriteRenderer>().sprite = pinOut;
        }
        cooking = true;
        thrower = holder;
        AudioMan.PlaySound(Sound.GrenadePull);
        tags.Remove("deadly weapon");
        tags.Add("dangerous");
    }
    public override void OnThrow()
    {
        base.OnThrow();
        thrower = holder;
        AudioMan.PlaySound(Sound.GrenadeThrow);
    }
    public virtual void Explode()
    { 
        OnExplosion();
    }
    public virtual void OnExplosion()
    {

    }
}
