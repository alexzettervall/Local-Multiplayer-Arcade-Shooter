using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Chicken : LivingEntity
{
    private BotAI ai;
    [SerializeField] private int peckDamage;
    [SerializeField] private float peckRadius;
    [SerializeField] private float peckDistance;
    [SerializeField] private float peckDelay;
    [SerializeField] private Animator animator;

    private float peckTimer;

    protected override void OnStart()
    {
        base.OnStart();

        ai = new ChickenBotAI();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        peckTimer -= Time.deltaTime;
    }

    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        ai.SetEntity(this);
        ai.Update();
        Move(movement);
        Rotate(direction);

        if (isUsing)
        {
            Peck();
        }
    }

    public void Peck()
    {
        if (peckTimer > 0)
        {
            return;
        }
        peckTimer = peckDelay;
        animator.SetTrigger("Peck");
        
        AttackMelee(peckDamage, Sound.Punch, peckRadius, peckDistance);
    }
}