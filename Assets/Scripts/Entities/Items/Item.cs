using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : Entity
{
    [SerializeField] protected Animation holdAnimation;
    [SerializeField] protected Sound pickUp;
    [SerializeField] protected string groundLayer;
    [SerializeField] protected string heldLayer;
    [SerializeField] protected float size = 0.8f;
    [SerializeField] protected HoldPosition holdPosition;
    [SerializeField] protected Vector2 holdOffset;
    [SerializeField] protected bool throwable = false;
    [SerializeField] protected float throwStrength = 10f;
    [SerializeField] protected float throwTorque = 100f;
    protected Entity holder;
    protected GameObject circle;
    protected bool heldBack = false;
    protected Entity thrower;
    protected bool thrown = false;

    protected override void OnStart()
    {
        base.OnStart();

        UpdateCircle();
    }
    public virtual void Use(bool use, bool unUse)
    {
        if (throwable)
        {
            if (use || !heldBack)
            {
                PullBack();
            }
            if (unUse)
            {
                Throw();
            }
        }
    }
    public virtual bool IsHeld()
    {
        return holder != null;
    }
    public virtual void PickUp(Entity holder)
    {
        this.holder = holder;
        HideCircle();
        sr.sortingLayerName = heldLayer;
        if (holder is Player)
        {
            Player player = (Player)holder;
            Animator animator = player.GetAnimator();
        }
        AudioMan.PlaySound(pickUp);
        if (holder is Player)
        {
            Player player = (Player)holder;
            if (holdPosition == HoldPosition.RightHand)
            {
                transform.parent = player.GetRightHand();
            }
            else if (holdPosition == HoldPosition.LeftHand)
            {
                transform.parent = player.GetLeftHand();
            }
            transform.localPosition = holdOffset;
        }
    }
    public virtual void OnDropped()
    {
        heldBack = false;
        holder = null;
        ShowCircle();
        sr.sortingLayerName = groundLayer;
    }


    public void PullBack()
    {
        heldBack = true;
        SetAnimatorTrigger("Pull");
        OnPullBack();
    }
    public void Throw()
    {
        heldBack = false;
        OnThrow();
        holder.DropItem(true);
        rb.centerOfMass = Vector2.zero;
        rb.velocity = transform.up * throwStrength;
        rb.angularVelocity = throwTorque;
        SetAnimatorTrigger("Throw");
    }

    public virtual void OnPullBack()
    {

    }
    public virtual void OnThrow()
    {
        thrower = holder;
        thrown = true;
    }

    public Entity GetHolder()
    {
        return holder;
    }
    private Animator GetAnimator()
    {
        if (holder is Player)
        {
            Player player = (Player)holder;
            return player.GetAnimator();
        }
        return null;
    }
    protected void SetAnimatorBool(string name, bool value)
    {
        Animator animator = GetAnimator();
        if (animator == null)
        {
            return;
        }
        animator.SetBool(name, value);
    }
    protected void SetAnimatorTrigger(string name)
    {
        Animator animator = GetAnimator();
        if (animator == null)
        {
            return;
        }
        animator.SetTrigger(name);
    }
    public void SetThrower(Entity thrower)
    {
        this.thrower = thrower;
    }
    public override int Damage(int damage, Entity damager, DamageSource damageSource)
    {
        return 0;
    }
    public void InitCircle()
    {
        if (circle != null)
        {
            return;
        }
        circle = new GameObject("Circle");
        circle.transform.parent = transform;
        circle.transform.localPosition = rb.centerOfMass;
        circle.transform.localScale = new Vector3(size, size, 1f);
        SpriteRenderer sr = circle.AddComponent<SpriteRenderer>();
        sr.sortingLayerName = "Item Circle";
        sr.sprite = GameAssets.i.circle;
        sr.color = GameAssets.i.itemCircleNormal;
    }
    public void UpdateCircle()
    {
        if (circle == null)
        {
            InitCircle();
        }
        circle.transform.localPosition = rb.centerOfMass;
        circle.transform.localScale = new Vector3(size, size, 1f);
    }
    public void HideCircle()
    {
        if (circle == null)
        {
            InitCircle();
        }
        circle.SetActive(false);
    }
    public void ShowCircle()
    {
        if (circle == null)
        {
            InitCircle();
        }
        circle.SetActive(true);
    }
    public void SetCircleColor(Color color)
    {
        if (circle == null)
        {
            InitCircle();
        }
        circle.GetComponent<SpriteRenderer>().color = color;
    }
    public virtual float GetDamageLeft() {
        return 0f;
    }
    public virtual float GetDPS() {
        return 0f;
    }
}
public enum HoldPosition
{
    RightHand = 0,
    LeftHand = 1,
    Middle = 2
}