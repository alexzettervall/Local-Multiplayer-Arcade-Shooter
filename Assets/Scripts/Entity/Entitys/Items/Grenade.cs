using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Grenade : Item
{
    [SerializeField] private Sprite pinOut;
    protected Entity thrower;
    protected bool cooking = false;
    protected bool exploded = false;
    [SerializeField] private float explosionTimer = 2f;
    
    protected override void OnStart() {
        base.OnStart();
        tags.Add("deadly weapon");
        tags.Add("throwable");
    }

    protected override void OnUpdate()
    {
        if (cooking && !exploded)
        {
            explosionTimer -= Time.deltaTime;
            if (explosionTimer <= 0)
            {
                Explode();
            }
        }
    }
    public override void OnPullBack()
    {
        base.OnPullBack();
        AudioMan.PlaySound(Sound.GrenadePull);
        StartCooking(holder);
    }
    public virtual void StartCooking(Entity thrower)
    {
        this.thrower = thrower;
        if (pinOut != null)
        {
            GetComponent<SpriteRenderer>().sprite = pinOut;
        }
        cooking = true;
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
        if (exploded) { return; }
        exploded = true;
        OnExplosion();
    }
    public virtual void OnExplosion()
    {

    }
}
