using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : Bullet
{
    public GameObject spritePrefab;
    public int bounces;
    public float range;
    public bool protective;

    private void Start()
    {
        Shoot();
    }
    public void Shoot()
    {
        Vector2 currentPos = transform.position;
        Vector2 dir = transform.up;
        for (int i = 0; i <= bounces; i++)
        {
            float length = 0f;
            
            RaycastHit2D[] hits = Physics2D.RaycastAll(currentPos, dir, range, GameAssets.i.structuresOnly);
            
            Entity entityHit = null;
            Vector2 hitPoint = Vector2.zero;
            

            foreach (RaycastHit2D hit in hits)
            {
                Entity entity = hit.transform.gameObject.GetComponent<Entity>();
                if (entity != null && !entity.BlocksLasers()) { continue; }
                entityHit = entity;
                hitPoint = hit.point;
                length = hit.distance + 0.5f;
                break;
            }
            if (length == 0)
            {
                length = range;
            }

            DrawBeam(currentPos, dir, Mathf.Min(length, range));
            

            // Do damage
            if (entityHit != null)
            {
                if (entityHit == shooter) { continue; }
                entityHit.Damage(damage, shooter, DamageSource.Bullet);
            }

            hits = Physics2D.LinecastAll(currentPos, hitPoint + (hitPoint - currentPos).normalized * 0.1f);
            foreach (RaycastHit2D hit in hits)
            {
                Entity entity = hit.transform.gameObject.GetComponent<Entity>();
                if (entity == null) { continue; }
                if (entity == shooter) { continue; }
                if (entity == entityHit) { continue; }
                entity.Damage(damage, shooter, DamageSource.Bullet);
            }

            //dir = newDir;
        }
    }
    public Vector2 DrawBeam(Vector2 pos, Vector2 dir, float dist)
    {
        Vector2 finalPos = pos + dir * dist;
        GameObject laser = Instantiate(spritePrefab, (pos + finalPos) / 2, Quaternion.identity, transform);
        laser.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        SpriteRenderer laserSprite = laser.GetComponent<SpriteRenderer>();
        laserSprite.size = new Vector2(dist, 0.5f);
        return finalPos;
    }
    public override void CheckForCollisions(Vector2 position, bool ignoreShooter = false)
    {
        return;
    }
    public override void CollideWithEntity(Entity entity)
    {
        return;
    }
}
