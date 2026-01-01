using UnityEngine;

public class StatusEffect
{
    public StatusEffectType statusEffectType;
    public Entity inflictor;
    public float duration;
    float timer = 0;

    public StatusEffect(StatusEffectType statusEffectType, float duration, Entity inflictor)
    {
        this.statusEffectType = statusEffectType;
        this.duration = duration;
        this.inflictor = inflictor;
    }

    public void Update(Entity entity)
    {
        if (statusEffectType == StatusEffectType.Fire)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = 0.4f; // Magic number (1 second between fire ticks)
                entity.FireTick(inflictor);
            }
        }
    }
}