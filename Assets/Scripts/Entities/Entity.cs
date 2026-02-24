using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Entity : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected SpriteRenderer sr;

    [Header("Properties")]
    [SerializeField] protected Entity lastDamager;
    [SerializeField] protected bool isDead;
    [SerializeField] protected int maxHealth;
    [SerializeField] protected int health;
    [SerializeField] protected Item heldItem;
    [SerializeField] protected Transform itemHolder;
    [SerializeField] protected bool reflectBullets = false;
    [SerializeField] protected bool blocksLasers = true;
    [SerializeField] protected bool canBeFrozen = false;

    [Header("Tags")] // Used by ai
    [SerializeField] protected List<string> tags = new List<string>();

    [Header("Sounds")]
    [SerializeField] protected Material material;

    [Header("Status")]
    [SerializeField] protected Dictionary<StatusEffectType, StatusEffect> statusEffects = new Dictionary<StatusEffectType, StatusEffect>();


    private GameObject fire;
    private GameObject iceCube;


    // Unity Functions
    private void Awake() {
        OnAwake();
    }
    private void Start()
    {
        OnStart();   
    }
    private void Update()
    {
        OnUpdate();
    }
    private void FixedUpdate()
    {
        OnFixedUpdate();
    }



    // Overidable Functions
    public virtual void Damage(int damage, Entity damager, DamageSource damageSource)
    {
        if (material != null)
        {
            Sound hitSound = Sound.None;
            if (damageSource == DamageSource.Bullet)
            {
                hitSound = material.bulletHit;
            }
            else if (damageSource == DamageSource.Melee)
            {
                hitSound = material.punchHit;
            }
            else if (damageSource == DamageSource.Explosion)
            {
                hitSound = material.explosionHit;
            }
            else if (damageSource == DamageSource.Fire)
            {
                hitSound = material.fireHit;
            }
            else if (damageSource == DamageSource.Gas)
            {
                hitSound = material.gasHit;
            }
            AudioMan.PlaySound(hitSound);
        }
        // Dont do checks if damage is 0
        if (damage > 0)
        {
            lastDamager = damager;
            health -= damage;
            if (health <= 0)
            {
                Kill();
            }
        }
    }
    protected virtual void OnAwake() {
        
    }
    protected virtual void OnStart()
    {

    }
    protected virtual void OnUpdate()
    {
        UpdateStatusEffects();
    }
    protected virtual void OnFixedUpdate()
    {

    }
    protected virtual void UpdateStatusEffects()
    {
        List<StatusEffectType> toRemove = new List<StatusEffectType>();
        foreach (StatusEffect statusEffect in statusEffects.Values)
        {
            statusEffect.duration -= Time.deltaTime;
            if (statusEffect.duration <= 0)
            {
                toRemove.Add(statusEffect.statusEffectType);
            } 
            else
            {
                statusEffect.Update(this);
            }
        }
        foreach (StatusEffectType statusEffectType in toRemove)
        {
            RemoveStatusEffect(statusEffectType);
        }
    }

    // Status effect logic
    public virtual void FireTick(Entity inflictor)
    {
        Damage(10, inflictor, DamageSource.Fire); // Magic number 5 damage from fire
    }

    public virtual void ApplyStatusEffect(StatusEffect statusEffect)
    {
        if (statusEffects.ContainsKey(statusEffect.statusEffectType))
        {
            // If exists take max duration
            statusEffects[statusEffect.statusEffectType].duration = Mathf.Max(statusEffect.duration, statusEffects[statusEffect.statusEffectType].duration);
        }
        else
        {
            // If doesn't exist add it to effects
            statusEffects.Add(statusEffect.statusEffectType, statusEffect);
            
            // Visuals
            if (statusEffect.statusEffectType == StatusEffectType.Fire)
            {
                fire = Instantiate(GameAssets.i.fireIconPrefab, transform.position, Quaternion.identity);
                fire.GetComponent<Follow>().target = transform;
            }
            else if (statusEffect.statusEffectType == StatusEffectType.Frozen && canBeFrozen)
            {
                iceCube = Instantiate(GameAssets.i.iceCubeIconPrefab, transform.position, Quaternion.identity);
                iceCube.GetComponent<Follow>().target = transform;
            }
        }
    }
    public virtual void RemoveStatusEffect(StatusEffectType statusEffectType)
    {
        statusEffects.Remove(statusEffectType);
        
        // Visuals
        if (statusEffectType == StatusEffectType.Fire)
        {
            Destroy(fire);
        }
        else if (statusEffectType == StatusEffectType.Frozen)
        {
            Destroy(iceCube);
        }
    }

    public virtual bool HasStatusEffect(StatusEffectType statusEffectType)
    {
        return statusEffects.ContainsKey(statusEffectType);
    }
    
    protected virtual void Kill()
    {
        isDead = true;
        Destroy(gameObject);
    }
    // Item functions
    public virtual void PickUpItem(Item item)
    {
        if (heldItem != null)
        {
            DropItem(false);
        }
        Entity holder = item.GetHolder();
        if (holder != null)
        {
            holder.DropItem(true);
        }
        heldItem = item;
        heldItem.transform.parent = itemHolder;
        heldItem.transform.localPosition = Vector3.zero;
        heldItem.transform.localRotation = Quaternion.identity;
        heldItem.rb.bodyType = RigidbodyType2D.Kinematic;
        heldItem.PickUp(this);
    }
    public virtual void DropItem(bool forced)
    {
        if (heldItem == null)
        {
            return;
        }
        heldItem.transform.parent = transform.parent;
        heldItem.rb.bodyType = RigidbodyType2D.Dynamic;
        if (!forced)
        {
            heldItem.rb.velocity = transform.up * 2f + transform.right * Random.Range(-1f, 1f);
            heldItem.rb.angularVelocity = Random.Range(-1f, 1f) * 50f;
        }
        heldItem.OnDropped();
        heldItem = null;
        OnDropItem(forced);
    }
    public virtual void OnDropItem(bool forced)
    {

    }
    public virtual void UseItem(bool use, bool unUse) // Use() is called when ever button is held down. "use" will be true when button was pressed this frame.
    {
        if (heldItem == null)
        {
            return;
        }
        heldItem.Use(use, unUse);
    }
    


    // Getters and Setters
    public int GetHealth()
    {
        return health;
    }
    public int GetMaxHealth()
    {
        return maxHealth;
    }
    public Entity GetLastDamager()
    {
        return lastDamager;
    }
    public Rigidbody2D GetRigidbody()
    {
        return rb;
    }
    public Item GetItem()
    {
        return heldItem;
    }
    public bool IsDead()
    {
        return isDead;
    }
    public Material GetMaterial()
    {
        return material;
    }
    public bool HasTag(string tag) {
        return tags.Contains(tag);
    }
    public bool ReflectsBullets()
    {
        return reflectBullets;
    }
    public bool BlocksLasers()
    {
        return blocksLasers;
    }
}
