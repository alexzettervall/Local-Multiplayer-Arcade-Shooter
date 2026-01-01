using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class Player : LivingEntity
{
    public int playerID { get; private set; }
    [SerializeField] private float moveSpeed;
    [SerializeField] private GameObject gunSprite;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Image healthCircle;
    [SerializeField] private Color color;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform rightHand;
    [SerializeField] private Transform leftHand;
    [SerializeField] private RadialAmmoDisplay ammoDisplay;
    [SerializeField] private float punchRadius = 0.4f;
    [SerializeField] private float punchDistance = 0.5f;
    [SerializeField] private int punchDamage = 15;
    [SerializeField] private float punchDelay = 0.33f;
    [SerializeField] private float stepDelay = 0.3f;
    [SerializeField] private LayerMask poisonGas;
    [SerializeField] private CircleCollider2D col;
    [SerializeField] private SpriteRenderer leftHandSr;
    [SerializeField] private SpriteRenderer rightHandSr;
    public bool isStatic = false;
    private float punchTimer;
    private Level level;

    private Vector2 movement;
    private Vector2 direction;
    private bool interact;
    private bool drop;
    private bool use;
    private bool unUse;
    private bool isUsing;
    private float lastFootStepTime;

    private bool inGas = false;
    private float gasTimer = 0f;
  
    private PlayerUI playerUI;
    public void InitializePlayer(int playerID)
    {
        level = FindObjectOfType<Level>();
        this.playerID = playerID;
        GameMan.PlayerData playerData = GameMan.Instance.GetPlayer(playerID);

        if (playerData != null)
        {
           color = playerData.PlayerColor;
           sr.sprite = playerData.Skin;
           leftHandSr.sprite = playerData.HandSkin.leftHand;
           rightHandSr.sprite = playerData.HandSkin.rightHand;
        }
        playerUI = new PlayerUI(this);
    } 

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
    public override void Damage(int damage, Entity damager, DamageSource damageSource)
    {
        // Return if the game hasnt started yet
        Level level = GameObject.FindObjectOfType<Level>();
        if (level == null || !level.IsStarted()) {
            return;
        }
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
                //GameObject bloodResidue = Instantiate(GameAssets.i.bloodResidue, transform.position, Quaternion.identity, transform.parent);
                //Randomizer randomizer = bloodResidue.GetComponent<Randomizer>();
                //randomizer.SetMaxOffSet(new Vector2(spread, spread));
            }
        }
        base.Damage(damage, damager, damageSource);
    }
    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (isDead) { return; }
        if (isStatic) { return; }
        playerUI.UpdateUI();
        punchTimer -= Time.deltaTime;
        GasLogic();
    }
    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        if (isDead) { return; }
        if (isStatic) { return; }
        if (HasStatusEffect(StatusEffectType.Frozen)) { return; }

        if (drop)
        {
            drop = false;
            DropItem(false);
        }
        if (isUsing || unUse)
        {
            if (heldItem != null)
            {
                UseItem(use, unUse);
            }
            else
            {
                Punch();
            }
            use = false;
            unUse = false;
        }
        if (interact)
        {
            interact = false;
            Interact();
        }
        if (level.IsStarted())
        {
            Move(movement);
        }
        
        Rotate(direction);
        
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
        Collider2D col_ = Physics2D.OverlapCircle(transform.position, col.radius, poisonGas);
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
    public void Punch()
    {
        if (punchTimer > 0)
        {
            return;
        }
        punchTimer = punchDelay;
        if (Random.Range(0, 2) == 1)
        {
            animator.SetTrigger("Punch Right");
        }
        else
        {
            animator.SetTrigger("Punch Left");
        }
        
        AttackMelee(punchDamage, Sound.Punch, punchRadius, punchDistance);
    }
    public void AttackMelee(float damage, Sound sound, float attackRadius, float attackDistance)
    {
        AudioMan.PlaySound(sound);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position + (Vector3)GetDirection() * attackDistance, attackRadius);
        foreach (Collider2D collider in colliders)
        {
            Entity entity = collider.GetComponent<Entity>();
            if (entity == null) { continue; }
            if (entity == this) { continue; }
            entity.Damage(punchDamage, this, DamageSource.Melee);
        }
    }
    private Vector2 GetDirection()
    {
        float rotRad = Mathf.Deg2Rad * (rb.rotation + 90f);
        return new Vector2(Mathf.Cos(rotRad), Mathf.Sin(rotRad));
    }
    public void Interact()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.7f, 1 << 6);

        Collider2D closestCollider = null;
        float closestDist = float.MaxValue;

        foreach (Collider2D collider in colliders)
        {
            if (collider.GetComponent<Item>() == heldItem)
            {
                continue;
            }
            float dist = Vector2.Distance(collider.transform.position, transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestCollider = collider;
            }
        }

        if (closestCollider != null)
        {
            PickUpItem(closestCollider.GetComponent<Item>());
        }
    }
    public void SetGunSprite(Sprite sprite)
    {
        gunSprite.GetComponent<SpriteRenderer>().sprite = sprite;
    }
    public void SetGunColor(Color color)
    {
        gunSprite.GetComponent<SpriteRenderer>().color = color;
    }
    public Transform GetFirePoint()
    {
        return firePoint;
    }
    public float GetAngle()
    {
        return rb.rotation;
    }
    public Image GetHealthCircle()
    {
        return healthCircle;
    }
    public Color GetColor()
    {
        return color;
    }
    public Animator GetAnimator()
    {
        return animator;
    }
    public Transform GetRightHand()
    {
        return rightHand;
    }
    public Transform GetLeftHand()
    {
        return leftHand;
    }
    public float GetMoveSpeed()
    {
        return moveSpeed;
    }
    public RadialAmmoDisplay GetAmmoDisplay()
    {
        return ammoDisplay;
    }
    public float GetDPS() {
        Item item = GetItem();
        if (item == null) {
            return punchDamage / punchDelay;
        }
        return Mathf.Max(punchDamage / punchDelay, item.GetDPS());
    }
    protected override void Kill()
    {
        if (heldItem != null)
        {
            Item item = heldItem;
            DropItem(true);
            Rigidbody2D itemRb = item.GetRigidbody();
            itemRb.angularVelocity = 1000f;
            itemRb.velocity = (item.transform.up * Random.Range(-1f, 1f) + item.transform.right * Random.Range(-1f, 1f)).normalized;
        }
        base.Kill();
        FindObjectOfType<Level>().OnPlayerDeath(this);
    }

    public override void PickUpItem(Item item)
    {
        base.PickUpItem(item);
        animator.SetTrigger("Pick Up");
        animator.SetBool("Holding Item", true);
    }
    public override void OnDropItem(bool forced)
    {
        base.OnDropItem(forced);
        if (!forced)
        {
            animator.SetTrigger("Pick Up");
        }
        animator.SetBool("Holding Item", false);
    }
}
