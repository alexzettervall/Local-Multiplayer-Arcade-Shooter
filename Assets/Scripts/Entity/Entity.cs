using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Entity : MonoBehaviour
{
    // Settings
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected SpriteRenderer sr;

    // Properties
    [SerializeField] protected Entity lastDamager;
    [SerializeField] protected bool isDead;
    [SerializeField] protected int maxHealth;
    [SerializeField] protected int health;
    [SerializeField] protected Item heldItem;
    [SerializeField] protected Transform itemHolder;
    [SerializeField] protected bool reflectBullets = false;
    [SerializeField] protected bool blocksLasers = true;

    // Tags (used for AI right now)
    [SerializeField] protected List<string> tags = new List<string>();

    // Sounds
    [SerializeField] protected Material material;

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

    }
    protected virtual void OnFixedUpdate()
    {

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
