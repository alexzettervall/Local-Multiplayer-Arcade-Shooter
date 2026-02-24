using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : Bullet
{
    public bool launched = false;
    [SerializeField] protected float acceleration = 200f;
    [SerializeField] protected float initialSpeed = 10f;
    [SerializeField] protected float radius = 2f;
    [SerializeField] protected int shrapnel;

    protected override void OnAwake() {
        base.OnAwake();
        rb.simulated = false;
    }

    public override void CollideWithEntity(Entity entity) {
        if (!launched) return;
        Explode();
    }

    public override void UpdateLifeTimer() {
        return;
    }

    public void Explode() {
        GameMan.Instance.GetLevel().CreateExplosion(transform.position, damage, radius, shooter, shrapnel);
        Kill();
    }

    public void Launch() {
        if (launched) return;
        launched = true;
        rb.simulated = true;
        transform.parent = FindObjectOfType<Level>().transform;
        rb.AddForce(transform.up * initialSpeed, ForceMode2D.Impulse);
    }

    protected override void OnFixedUpdate() {  
        base.OnFixedUpdate();

        if (launched) {
            rb.AddForce(transform.up * acceleration * Time.fixedDeltaTime, ForceMode2D.Impulse);
        }
    }
}
