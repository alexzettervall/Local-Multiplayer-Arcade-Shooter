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
            RaycastHit2D col = Physics2D.Raycast(currentPos, dir, range, GameAssets.i.structuresOnly);
            length = col.distance + 0.5f;
            if (length == 0)
                length = range;

            DrawBeam(currentPos, dir, Mathf.Min(length, range));
            //Vector2 newDir = Vector2.Reflect(dir, col.normal);
            //if (Mathf.Abs((Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg) - (Mathf.Atan2(newDir.y, newDir.x) * Mathf.Rad2Deg)) < 0.01f)
            //{
            //    break;
            //}

            // Do damage
            RaycastHit2D[] hits = Physics2D.LinecastAll(currentPos, col.point);
            foreach (RaycastHit2D hit in hits)
            {
                Entity entity = hit.transform.gameObject.GetComponent<Entity>();
                if (entity == null) { continue; }
                if (entity == shooter) { continue; }
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
