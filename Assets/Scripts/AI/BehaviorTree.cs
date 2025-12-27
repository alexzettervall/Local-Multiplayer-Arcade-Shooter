using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorTree
{
    Blackboard blackboard;
    
    public BehaviorTree(Blackboard blackboard) {
        this.blackboard = blackboard;
    }

    public void Update() {
        if (blackboard.goal == "Attack") {
            Attack();
        }
        else if (blackboard.goal == "Loot") {
            Loot();
        }
        else {
            blackboard.move = false;
        }


        // Bug fix
        if (blackboard.goal != "Attack" && blackboard.isUsing) {
            blackboard.cancelUse = true;
        }
    }

    public void Attack() {
        // Find target
        Player closestEnemy = blackboard.targetEnemy;
        if (closestEnemy == null) {
            return;
        }
        blackboard.target = new AITarget { provider = new TransformTarget { transform = closestEnemy.transform } };

        float distance = Vector2.Distance(blackboard.player.transform.position, closestEnemy.transform.position);
        bool wantToShoot = false;
        bool hasLineOfSight = true;
        if (Physics2D.Linecast(blackboard.player.transform.position, blackboard.target.GetPosition(), GameAssets.i.structuresOnly)) {
            hasLineOfSight = false;
        }
        
        bool inRange = distance <= blackboard.attackRange;
        bool outOfThreshold = distance > blackboard.attackRange + 0.1f;
        Item heldItem = blackboard.player.GetItem();
        bool hasGun = (heldItem is Gun);

        if (inRange && hasLineOfSight) {
            wantToShoot = true;
        }
        else if (outOfThreshold || !hasLineOfSight) {
            wantToShoot = false;
        }

        if (wantToShoot) {
            blackboard.move = !hasGun;
            Vector2 direction = Vector2.Lerp(blackboard.lookDirection.normalized, (blackboard.target.GetPosition() - (Vector2)blackboard.player.transform.position).normalized, GameAssets.i.AISettings.movementSmoothingResponsiveness * Time.deltaTime);
            float rbRotation = blackboard.player.GetRigidbody().rotation;
            bool lookingAt = Vector2.Dot(direction, new Vector2(Mathf.Cos(rbRotation), Mathf.Sin(rbRotation))) > 0.99f;

            blackboard.lookDirection = direction;
            if (!blackboard.isUsing && (lookingAt || distance < 1f)) {
                blackboard.preformUse = true;
            }
        }
        else {
            // Tell navigator to goto
            blackboard.move = true;

            if (blackboard.isUsing) {
                blackboard.cancelUse = true;
            }
        }
        
        // Drop gun if empty
        if (hasGun) {
            Gun gun = (Gun) heldItem;
            blackboard.attackRange = 10000f;
            if (gun.GetAmmo() <= 0) {
                blackboard.drop = true;
            }
        }
        else if (heldItem is Grenade)
        {
            blackboard.attackRange = 1000f;
        }
        else {
            blackboard.attackRange = 1.29f;
        }
    }

    public void Loot() {
        // Need to verify that target is a item
        Item item = blackboard.targetLoot;
        if (item == null) {
            blackboard.target = null;
            blackboard.move = false;
            return;
        }
        blackboard.target = new AITarget { provider = new TransformTarget { transform = item.transform } };
        blackboard.move = true;
        float distance = Vector2.Distance(blackboard.player.transform.position, blackboard.target.GetPosition());
        if (distance < 0.5f) {
            blackboard.interact = true;
            blackboard.target = null;
            blackboard.isDirty = true;
        }
    }

    public Item GetClosestUseableItem()
    {
        var sourcePosition = blackboard.player.transform.position;
        Item closestItem = null;
        float closestDistanceSqr = float.MaxValue;

        foreach (var itemData in blackboard.itemDatas)
        {
            var item = itemData.item;
            if (item == null) continue;
            if (item.IsHeld()) continue; 
            if (!item.HasTag("deadly weapon")) {
                continue;
            }

            float distanceSqr = (item.transform.position - sourcePosition).sqrMagnitude;
            if (distanceSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distanceSqr;
                closestItem = item;
            }
        }

        return closestItem;
    }


    
}
