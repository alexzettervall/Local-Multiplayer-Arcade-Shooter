using UnityEngine;
using UnityEngine.InputSystem;

public abstract class LivingEntity : Entity
{
    protected Vector2 movement;
    protected Vector2 direction;
    protected bool interact;
    protected bool drop;
    protected bool use;
    protected bool unUse;
    protected bool isUsing;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float stepDelay = 0.3f;
    [SerializeField] private CircleCollider2D col;

    private float lastFootStepTime;
    private bool inGas = false;
    private float gasTimer = 0f;

    public void OnMove(Vector2 movement)
    {
        this.movement = movement;
    }
    public void OnRotate(Vector2 direction, InputDevice device)
    {
        Vector2 input = direction;
        if (device is Mouse)
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(input);
            input = pos - (Vector2)transform.position;
        }
        else
        {
            if (input.magnitude < 0.1f)
            {
                return;
            }
        }
        this.direction = input;
    }
    public void OnInteract(bool triggered)
    {
        interact = triggered;
    }
    public void OnDrop(bool triggered)
    {
        drop = triggered;
    }
    public void OnUse(bool performed, bool canceled)
    {
        if (performed)
        {
            use = true;
            isUsing = true;
        }
        else if (canceled)
        {
            unUse = true;
            isUsing = false;
        }
    }
    
    public override int Damage(int damage, Entity damager, DamageSource damageSource)
    {
        // Emit blood particles
        float change = damage;
        if (health - damage < 0)
        {
            change += (health - damage);
        }
        if (damage > 0)
        {
            float spread = Mathf.Lerp(0.2f, 1.5f, damage / 100f);
            for (int i = 0; i < Mathf.CeilToInt(change/2f); i++)
            {
                Destroy(Instantiate(GameAssets.i.bloodSplatter, transform.position, Quaternion.identity), 5f);
            }
        }
        return base.Damage(damage, damager, damageSource);
    }

    public void Rotate(Vector2 direction)
    {
        if (HasStatusEffect(StatusEffectType.Frozen)) { return; }
        rb.rotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
    }
    public void Move(Vector2 movement)
    {
        if (HasStatusEffect(StatusEffectType.Frozen)) { return; }
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
            
        if (movement.magnitude > 0.1f)
        {
            if (Time.time - lastFootStepTime >= stepDelay)
            {
                Physics2D.queriesHitTriggers = true;
                Collider2D[] cols = Physics2D.OverlapPointAll(transform.position, GameAssets.i.structuresOnly);
                Physics2D.queriesHitTriggers = false;
                Material material = null;
                foreach (Collider2D col in cols)
                {
                    if (col == null)
                    {
                        continue;
                    }
                    Structure structure = col.GetComponent<Structure>();
                    Material mat = structure.GetMaterial();
                    if (material == null || mat.priority > material.priority)
                    {
                        material = mat;
                    }
                }
                if (material != null)
                {
                    AudioMan.PlaySound(material.footstep);
                    lastFootStepTime = Time.time;
                }
            }
        }
    }
    protected void GasLogic()
    {
        Collider2D col_ = Physics2D.OverlapCircle(transform.position, col.radius, GameAssets.i.poisonGasLayer);
        inGas = !(col_ == null);
        if (inGas)
        {
            gasTimer += Time.deltaTime;
        }
        else
        {
            gasTimer = 0;
        }
        if (gasTimer > GameMan.Instance.gasTickTime)
        {
            gasTimer = 0;
            Damage(GameMan.Instance.gasDamage, null, DamageSource.Gas);
        }
    }
    public int AttackMelee(int damage, Sound sound, float attackRadius, float attackDistance)
    {
        int damageDelt = 0;
        AudioMan.PlaySound(sound);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position + (Vector3)GetDirection() * attackDistance, attackRadius);
        foreach (Collider2D collider in colliders)
        {
            Entity entity = collider.GetComponent<Entity>();
            if (entity == null) { continue; }
            if (entity == this) { continue; }
            damageDelt += entity.Damage(damage, this, DamageSource.Melee);
        }
        return damageDelt;
    }
    
    private Vector2 GetDirection()
    {
        float rotRad = Mathf.Deg2Rad * (rb.rotation + 90f);
        return new Vector2(Mathf.Cos(rotRad), Mathf.Sin(rotRad));
    }

    public float GetMoveSpeed()
    {
        return moveSpeed;
    }

    public virtual float GetDPS() {
        /*
            Get the max dps this entity can currently deal.
        */
        return 0f;
    }
}
