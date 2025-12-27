using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Item
{
    [SerializeField] private Sound shootSound;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] protected int ammo;
    [SerializeField] private int maxAmmo;
    [SerializeField] private bool auto;
    [SerializeField] protected int damage;
    [SerializeField] private float fireRate;
    [SerializeField] private float spread;
    [SerializeField] private int amount;
    [SerializeField] private float minVel;
    [SerializeField] private float maxVel;
    private float shootDelay;

    public int GetAmmo()
    {
        return ammo;
    }
    public int GetMaxAmmo()
    {
        return maxAmmo;
    }
    public int GetDamge() {
        return damage;
    }
    public float GetFireRate() {
        return fireRate;
    }
    public override float GetDPS() {
        return ammo > 0 ? GetDamge() * GetFireRate() * amount : 0f;
    }
    public override float GetDamageLeft() {
        return amount * GetDamge() * ammo;
    }


    protected override void OnStart() {
        base.OnStart();
        tags.Add("deadly weapon");
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (shootDelay > 0)
        {
            shootDelay -= Time.deltaTime;
        }
    }
    public override void Use(bool use, bool unUse)
    {
        Shoot(use);
        if (ammo <= 0) {
            GetHolder().DropItem(true);
            
        }
    }
    public override void PickUp(Entity holder)
    {
        base.PickUp(holder);
        SetAnimatorBool("Holding Gun", true);
    }
    public override void OnDropped()
    {
        SetAnimatorBool("Holding Gun", false);
        if (ammo < 1)
        {
            Destroy(gameObject);
        }
        base.OnDropped();
    }
    public virtual void Shoot(bool pressed)
    {
        if (shootDelay > 0)
        {
            return;
        }
        if (ammo <= 0)
        {
            if (pressed)
            {
                AudioMan.PlaySound(Sound.GunClick);
            }
            return;
        }
        if (!pressed && !auto)
        {
            return;
        }
        int rounds = 0;
        while (shootDelay <= 0)
        {
            shootDelay += 1f / fireRate;
            rounds++;
        }
        for (int i = 0; i < rounds; i++)
        { 
            ammo--;
            BulletSpawner.ShootBullet(amount, bulletPrefab, GetFirePoint(), minVel, maxVel, spread, rb.rotation, damage, holder);
            if (ammo <= 0)
            {
                AudioMan.PlaySound(Sound.GunClick);
                SetCircleColor(GameAssets.i.itemCircleUseless);
                tags.Remove("deadly weapon");
                break;
            }
        }
        AudioMan.PlaySound(shootSound);
    }
    public Vector2 GetFirePoint()
    {
        Vector2 dir = firePoint.position - transform.position;
        RaycastHit2D[] raycastHits = Physics2D.RaycastAll(transform.position, dir, Vector2.Distance(transform.position, firePoint.position), GameAssets.i.structuresOnly | GameAssets.i.playersOnly);
        foreach (RaycastHit2D raycastHit in raycastHits) {
            if (raycastHit.collider == GetHolder().GetComponent<Collider2D>()) {
                continue;
            }
            if (raycastHit.collider != null)
            {
                return raycastHit.point;
            }
        }
        
        return firePoint.position;
    }
}
