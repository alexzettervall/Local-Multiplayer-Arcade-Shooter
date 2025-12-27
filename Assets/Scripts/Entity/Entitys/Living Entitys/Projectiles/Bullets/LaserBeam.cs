using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : Bullet
{
    public override void CheckForCollisions(Vector2 position, bool ignoreShooter = false)
    {
        return;
    }
    public override void CollideWithEntity(Entity entity)
    {
        return;
    }
    public void CheckForCols()
    {
        /*if (isDead)
        {
            return;
        }
        Collider2D[] colliders = new Collider2D[50]; // Maximum 10 results
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.useTriggers = true;  // Include triggers in the check
        GetComponent<BoxCollider2D>().OverlapCollider(contactFilter, colliders);

        foreach (Collider2D collider in colliders)
        {
            if (collider == col)
            {
                continue;
            }
            if (collider.GetComponent<Entity>() == null)
            {
                continue;
            }
            if ((collider.GetComponent<Entity>() == shooter))
            {
                continue;
            }

            Entity entity = collider.GetComponent<Entity>();
            entity.Damage(damage, shooter, DamageSource.Bullet);
        }*/
    }
}
