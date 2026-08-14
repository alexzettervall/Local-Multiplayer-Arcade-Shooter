using Unity.Mathematics;
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
    [SerializeField] private int damageToLayEgg;
    [SerializeField] private float eggVelocity;
    [SerializeField] private float eggAngularVelocity;
    [SerializeField] private Animator animator;

    private float peckTimer;
    private int damageCounter;

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
        if (controller == null)
        {
            ai.SetEntity(this);
            ai.Update();
        }
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
        
        damageCounter += AttackMelee(peckDamage, Sound.Punch, peckRadius, peckDistance);
        if (damageCounter > damageToLayEgg)
        {
            damageCounter = 0;
            LayEgg();
        }
    }

    public void LayEgg()
    {
        GameObject eggObj = Instantiate(GameAssets.i.eggPrefab, transform.position, quaternion.identity, transform.parent);
        Egg egg = eggObj.GetComponent<Egg>();
        egg.GetRigidbody().velocity = -direction * eggVelocity;
        egg.GetRigidbody().angularVelocity = UnityEngine.Random.Range(-eggAngularVelocity, eggAngularVelocity);
    }
}