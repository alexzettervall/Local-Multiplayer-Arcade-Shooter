using UnityEngine;
using UnityEngine.UI;

public class Player : LivingEntity
{
    public int playerID { get; private set; }
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
    [SerializeField] private float pickUpRadius = 0.7f;
    [SerializeField] private SpriteRenderer leftHandSr;
    [SerializeField] private SpriteRenderer rightHandSr;
    public bool isStatic = false;
    private float punchTimer;
    private Level level;
  
    private PlayerUI playerUI;
    public void InitializePlayer(int playerID)
    {
        level = FindObjectOfType<Level>();
        this.playerID = playerID;
        GameMan.PlayerData playerData = GameMan.Instance.GetPlayer(playerID);

        if (playerData != null)
        {
           color = playerData.PlayerColor;
           sr.sprite = playerData.Outfit.skin;
           leftHandSr.sprite = playerData.Outfit.hands.leftHand;
           rightHandSr.sprite = playerData.Outfit.hands.rightHand;
        }
        playerUI = new PlayerUI(this);
    } 

    
    public override int Damage(int damage, Entity damager, DamageSource damageSource)
    {
        // Return if the game hasnt started yet
        Level level = GameObject.FindObjectOfType<Level>();
        if (level == null || !level.IsStarted()) {
            return 0;
        }
        return base.Damage(damage, damager, damageSource);
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
    public void Interact()
    {
        Item item = GameMan.Instance.GetClosestItemInRange(transform.position, pickUpRadius, heldItem);

        if (item != null)
        {
            PickUpItem(item);
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
    public RadialAmmoDisplay GetAmmoDisplay()
    {
        return ammoDisplay;
    }
    public override float GetDPS()
    {
        Item item = GetItem();
        if (item == null) {
            return punchDamage / punchDelay;
        }
        return Mathf.Max(punchDamage / punchDelay, item.GetDPS());
    }
    public float GetPickUpRadius()
    {
        return pickUpRadius;
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
