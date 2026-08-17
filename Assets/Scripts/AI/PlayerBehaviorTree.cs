using UnityEngine;
using static PlayerBlackboard;

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
        if (!(blackboard.targetEnemy is PlayerData enemy))
        {
            return;
        }

        blackboard.target = enemy.Position;

        float distance = Vector2.Distance(blackboard.entity.transform.position, enemy.Position);
        
        RaycastHit2D[] hits = Physics2D.LinecastAll(blackboard.entity.transform.position, enemy.Position, GameAssets.i.structuresOnly);
        bool hasLineOfSight = hits.Length == 0;
        
        bool inRange = distance <= blackboard.attackRange;
        Item heldItem = blackboard.entity.GetItem();
        bool hasGun = heldItem is Gun;
        
        Vector2 directionToTarget = (enemy.Position - (Vector2)blackboard.entity.transform.position).normalized;
        blackboard.lookDirection = Vector2.Lerp(blackboard.lookDirection.normalized, directionToTarget, blackboard.settings.movementSmoothingResponsiveness * Time.deltaTime);
        float rbRotation = blackboard.entity.GetRigidbody().rotation;
        bool lookingAt = Vector2.Dot(directionToTarget, blackboard.lookDirection) > 0.99f;
        

        blackboard.move = !hasLineOfSight;

        blackboard.use = hasLineOfSight && lookingAt;
        
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
        blackboard.use = false;

        if (blackboard.targetLoot is not ItemData loot) return;
        if (blackboard.entity is not Player player) return;

        blackboard.move = true;
        blackboard.target = loot.Position;

        Item item = GameMan.Instance.GetClosestItemInRange(player.transform.position, player.GetPickUpRadius(), player.GetItem());
        if (item == null) return;
        if (!item.GetTags().Contains("deadly weapon")) return;

        blackboard.interact = true;
        blackboard.isDirty = true;
        blackboard.move = false;
    }
}
