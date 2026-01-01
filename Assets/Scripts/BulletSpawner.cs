using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BulletSpawner
{
    public static void ShootBullet(int amount, GameObject bulletPrefab, Vector2 firePoint, float minVel, float maxVel, float spread, float rotation, int damage, Entity shooter)
    {
        Transform level = GameObject.FindObjectOfType<Level>().transform;
        for (int i = 0; i < amount; i++)
        {
            // Create physical bullet and add velocity
            GameObject bulletObj = GameObject.Instantiate(bulletPrefab, firePoint, Quaternion.identity, level);
            Rigidbody2D bulletRb = bulletObj.GetComponent<Rigidbody2D>();
            float velocity = Random.Range(minVel, maxVel);
            float angle = rotation + Random.Range(-spread, spread);
            float angleRad = (angle + 90) * Mathf.Deg2Rad;
            bulletRb.rotation = angle;
            bulletRb.AddForce(new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) * velocity, ForceMode2D.Impulse);

            // Apply data
            Bullet bullet = bulletObj.GetComponent<Bullet>();
            if (bullet != null)
            {
                bullet.SetDamage(damage);
                bullet.SetShooter(shooter);
                bullet.CheckForCollisions(firePoint, true);
            }

            // If frag grenade start cooking
            FragGrenade fragGrenade = bulletObj.GetComponent<FragGrenade>();
            if (fragGrenade != null)
            {
                fragGrenade.StartCooking(shooter);
            }
        }
    }
}
