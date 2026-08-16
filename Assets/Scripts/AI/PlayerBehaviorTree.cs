using UnityEngine;

public class PlayerBehaviorTree : BehaviorTree<PlayerBlackboard>
{
    public PlayerBehaviorTree(PlayerBlackboard blackboard) : base(blackboard)
    {
        
    }

    public override void OnUpdate() {
        if (blackboard.goal == "Attack") {
            Attack();
        }
        else if (blackboard.goal == "Loot") {
            Loot();
        }
        else {
            blackboard.move = false;
        }
    }

    public void Attack() {
        // Find target
        if (!(blackboard.targetEnemy is Blackboard.PlayerData enemy))
        {
            return;
        }

        blackboard.target = enemy.position;

        float distance = Vector2.Distance(blackboard.entity.transform.position, enemy.position);
        
        RaycastHit2D[] hits = Physics2D.LinecastAll(blackboard.entity.transform.position, enemy.position, GameAssets.i.structuresOnly);
        bool hasLineOfSight = hits.Length == 0;
        
        bool inRange = distance <= blackboard.attackRange;
        Item heldItem = blackboard.entity.GetItem();
        bool hasGun = heldItem is Gun;
        
        Vector2 directionToTarget = (enemy.position - (Vector2)blackboard.entity.transform.position).normalized;
        blackboard.lookDirection = Vector2.Lerp(blackboard.lookDirection.normalized, directionToTarget, blackboard.settings.movementSmoothingResponsiveness * Time.deltaTime);
        float rbRotation = blackboard.entity.GetRigidbody().rotation;
        bool lookingAt = Vector2.Dot(directionToTarget, blackboard.lookDirection) > 0.99f;
        

        blackboard.move = !hasLineOfSight;

        if (hasLineOfSight && lookingAt) {
            if (!blackboard.isUsing) {
                blackboard.preformUse = true;
            }
        }
        else {
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
        if (!(blackboard.targetLoot is Blackboard.ItemData targetLoot))
        {
            return;
        }

        blackboard.target = targetLoot.position;
        blackboard.move = true;
        float distance = Vector2.Distance(blackboard.entity.transform.position, targetLoot.position);
        if (distance < 0.25f) {
            blackboard.interact = true;
            blackboard.isDirty = true;
        }
    }
}
