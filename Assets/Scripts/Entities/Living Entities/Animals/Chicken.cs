using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Chicken : LivingEntity
{
    private BotAI<ChickenBlackboard> ai;
    [SerializeField] private int peckDamage;
    [SerializeField] private float peckRadius;
    [SerializeField] private float peckDistance;
    [SerializeField] private float peckDelay;
    [SerializeField] private int damageToLayEgg;
    [SerializeField] private float eggVelocity;
    [SerializeField] private float eggAngularVelocity;

    [SerializeField] private float agroMoveSpeedMultiplier;
    [SerializeField] private int agroPeckDamage;
    [SerializeField] private float agroPeckDistance;
    [SerializeField] private float agroPeckDelay;
    [SerializeField] private float agroPeckRadius;
    [SerializeField] private float chainAgroRadius;
    [SerializeField] private bool overrideCurrentAgro;

    [SerializeField] private Animator animator;

    private float peckTimer;
    private int damageCounter;
    private bool isAgro = false;

    protected override void OnAwake()
    {
        base.OnAwake();
        ai = new ChickenBotAI();
    }
    
    protected override void OnUpdate()
    {
        base.OnUpdate();
        peckTimer -= Time.deltaTime;
        if (isAgro)
        {
            if (ai.GetBlackboard().agroTarget == null)
            {
                UnAgro();
            }
        }
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

    public override int Damage(int damage, Entity damager, DamageSource damageSource)
    {
        if (damager is LivingEntity attacker && damager is not Chicken)
        {
            Agro(attacker, true);
        }
        return base.Damage(damage, damager, damageSource);
    }

    public void Peck()
    {
        if (peckTimer > 0)
        {
            return;
        }
        peckTimer = isAgro ? agroPeckDelay : peckDelay;
        animator.SetTrigger("Peck");
        
        if (isAgro)
        {
            damageCounter += AttackMelee(agroPeckDamage, Sound.Punch, agroPeckRadius, agroPeckDistance);
        }
        else
        {
            damageCounter += AttackMelee(peckDamage, Sound.Punch, peckRadius, peckDistance);
        }

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

    public void Agro(LivingEntity target, bool chainAgro = false)
    {
        var blackboard = ai.GetBlackboard();
        blackboard.agroTarget = target;
        blackboard.isDirty = true;
        SetMoveSpeedMultiplier(agroMoveSpeedMultiplier);
        animator.SetBool("IsAgro", true);
        isAgro = true;

        if (chainAgro)
        {
            foreach (Chicken chicken in FindObjectsOfType<Chicken>())
            {
                if (chicken == this)
                    continue;

                float distance = Vector2.Distance(chicken.transform.position, transform.position);

                if (distance <= chainAgroRadius && (!chicken.isAgro || chicken.overrideCurrentAgro))
                {
                    chicken.Agro(target, false);
                }
            }
        }
    }

    public void UnAgro()
    {
        SetMoveSpeedMultiplier(1f);
        animator.SetBool("IsAgro", false);
        isAgro = false;
    }
}